import RoToolsInstance,{IRoToolsInstance} from "./data/serverdata.js";
import { WSConnection, WSHook } from "./modules/wshook.js";
import mongoose from "mongoose";
import { RandomString } from "./modules/util.js";
import { KubernetesClient } from "./modules/kube_config.js";
import { V1EnvVar } from "@kubernetes/client-node";
import standard_env from "./standard_env.js";
import connection from "./data/connection.js";
import {env, loadEnv} from "./env.js";
import { DataConfigurator } from "./modules/data_config.js";
const configurator_listener = new WSHook("localhost", 8081);
const post_listener = new WSHook("localhost", 8082);



export interface IInstance {
    Hostname:string,
    Connection:WSConnection
}

export interface IConfigData {
    hostname:string
}
loadEnv();
connection();

var rootListeners:{[id:string]:(token:string)=>void} = {}

var Instances:{[id:string]:{[hostanme:string]:IInstance}} = {}

function prepareConfigGroup() :V1EnvVar[]{
    var envars: V1EnvVar[] = []
    for (var i in standard_env) {
        envars.push({
            name:i,
            value:standard_env[i]
        })
    }
    return envars;
}

configurator_listener.OnConnection((connection:WSConnection)=>{
    var connection_id:mongoose.Types.ObjectId;
    var i_hostname:string;
    connection.OnQuery("authenticate", async (data, qs)=>{
        var exist = await RoToolsInstance.findOne({
            _id: new mongoose.Types.ObjectId(data.id),
            privateKey: data.privateKey,
        });
        if (exist) {
            connection_id = new mongoose.Types.ObjectId(data.id);
            i_hostname = data.i_hostname;
            if (Instances[String(data.id)]){
                Instances[String(data.id)][i_hostname] = {
                    Hostname: i_hostname,
                    Connection: connection
                }
            }else {
                Instances[String(data.id)] = {}
                Instances[String(data.id)][i_hostname] = {
                    Hostname: i_hostname,
                    Connection: connection
                }
            }
            
        }
        qs.Reply({});
    })
    connection.OnQuery("get_config", async (data, qs)=>{
        var dx = await RoToolsInstance.findById(connection_id);
        qs.Reply(dx);
    })
    connection.OnQuery("reset_key", async (data, qs)=>{
        var str = RandomString(128);
        var inst = await RoToolsInstance.findById(connection_id);
        if (inst) {
            inst.gameKey = str;
            await inst.save();
        }
        qs.Reply(str);
    })
    connection.OnQuery("get-instances", async(data,qs)=>{
        var instTable = [];
        if (Instances[String(connection_id)]) {
            for (var i in Instances[String(connection_id)]) {
                instTable.push(i);
            }
            return qs.Reply(instTable);
        }
        return qs.Reply([]);
    })
    connection.OnMessage(async (data:any)=>{
        if (data.header == "root_cfg"){
            var inst = await RoToolsInstance.findById(connection_id);
            if (rootListeners[String(connection_id)]) {
                rootListeners[String(connection_id)](data.token);
                delete rootListeners[String(connection_id)];
            }
        }
    })
    connection.OnClose(()=>{
        delete Instances[String(connection_id)][i_hostname];
    })
})

post_listener.OnConnection((connection: WSConnection)=>{
    var isSocketAuthenticated = false;
    connection.OnMessage(async (data)=>{
        if (data.header == "configure_server") {
            var gKey = RandomString(128);
            var sKey = RandomString(256);
            var dbUser = RandomString(16);
            var dbPass = RandomString(32);
            var inst = await RoToolsInstance.create({
                hostname: data.hostname,
                gameKey:gKey,
                privateKey:sKey,
                owner:"123",
                online:false,
                dbUser:dbUser
            });
            await DataConfigurator.CreateDatabase("cli-" + inst._id, dbUser, dbPass);
            var ns = await KubernetesClient.CreateNamespace("cli-" + inst._id);
            Instances[String(inst._id)] = {};
            rootListeners[String(inst._id)] = (token:string) =>{
                connection.Send({
                    id: inst._id,
                    status: "RootCreated",
                    token: token
                })
            }
            await ns.Deploy({
                selector:"game-instance",
                image:"jaym244/rotools-web",
                port:8080,
                outsidePort:80,
                replicas:1,
                hostname:inst.hostname, 
                name:"game-instance-deployment",
                envParams: prepareConfigGroup().concat([{
                    name:"PRIVATE_KEY",
                    value:sKey
                },{
                    name:"DBUSER",
                    value:dbUser,
                },{
                    name:"DBPASS",
                    value:dbPass
                }, {
                    name:"DBNAME",
                    value: "cli-" + inst._id
                },{
                    name:"DBHOST",
                    value:env.DB_HOSTNAME
                },{
                    name:"IS_DEBUG",
                    value:String(env.IS_DEBUG)
                },{
                    name:"REPLICA_SET",
                    value:env.REPLICA_SET
                },{
                    name:"CONNECTION",
                    value:env.CONTROLLER_CONNECTION
                },{
                    name:"NODE_ENV",
                    value:"production"
                }
            ])
            })
            connection.Send({
                id: inst._id,
                status: "InstanceProvisioned"
            })

        }
        if (data.header=="terminate") {
            const terminateId = data.id;
            var instance = await RoToolsInstance.findById(terminateId);
            if (instance) {
                var space = await KubernetesClient.GetNamespace("cli-" + instance._id)
                await space?.Delete();
                await DataConfigurator.DeleteDatabase("cli-" + instance._id, instance.dbUser);
                await instance.delete();
                connection.Send({
                    done: true
                })
            }
        }
        if (data.header == "update") {
            var instances = await RoToolsInstance.find({});
            for (var inst of instances) {
                var n = await KubernetesClient.GetNamespace("cli-" + inst._id);
                await n?.RollingUpdate();
            }
        }
        if (data.header == "set-replicas") {
            var instance = await RoToolsInstance.findById(data.id);
            if (instance) {
                var space = await KubernetesClient.GetNamespace("cli-" + instance._id)
                await space?.SetReplicas(data.replicas);
                connection.Send({
                    done: true
                })
            }
        }
    });
});

process.on('SIGTERM', () => {
    console.info('Initiating graceful shutdown...');
    for (var i in Instances) {
        for (var x in Instances[i]) {
            Instances[i][x].Connection.ws.close();
        }
    }
});

post_listener.Listen(()=>{
    configurator_listener.Listen(()=>{
        console.log("Online");
    })
})
console.log("V1.1.3");