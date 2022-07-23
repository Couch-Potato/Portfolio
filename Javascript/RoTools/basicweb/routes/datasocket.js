import api from "../data/models/api.js";
import server from "../data/models/server.js";
import { RandomString } from "../util.js";

var websocket = {};
websocket["/data"] = async (ws, req)=>{
    const user_id = req.headers.client;
    const token = req.headers.token;
    if (!token || !user_id)
    {
        ws.send(JSON.stringify({
            status:401,
            message:"Unauthorized; Token or Client headers are required for this connection."
        }));
    }
    var client = await api.authenticate(user_id, token);
    if (!client){
        ws.send(JSON.stringify({
            status:401,
            message:"Unauthorized"
        }));
        ws.close();
    }
}

websocket["/stream"] = async(ws,req)=>{
    const user_id = req.headers.client;
    const token = req.headers.token;
    const serverid = req.id;
    if (!token || !user_id)
    {
        ws.send(JSON.stringify({
            status:401,
            message:"Unauthorized; Token or Client headers are required for this connection."
        }));
    }
    var client = await api.authenticate(user_id, token);
    if (!client){
        ws.send(JSON.stringify({
            status:401,
            message:"Unauthorized"
        }));
        ws.close();
    }

    const socket_id = RandomString(12);

    var svr = await server.findById(serverid);
    var bus = await server.getBus();

    await bus.write({
        channel:"socket_create",
        data:{
            socket:socket_id
        }
    });

    ws.on("message", async (dataString)=>{
        var data = JSON.parse(dataString);
        bus.write({
            channel:"socket",
            data:{
                socket:socket_id,
                data:data
            }
        });
    })

    var busss = bus.attach(async ()=>{
        var newData = await bus.glance();
        var clout = []
        for (var i in newData) {
            var x = newData[i];
            if (x.channel = "socket") {
                if (x.data.socket == socket_id) {
                    ws.send(JSON.stringify(x.data.data));
                    clout.push(i)
                }
            }
        }
        // Clear any pipe messages.
        for (var x of clout) {
            await bus.clearOne(x);
        }
    })

    ws.on("close", function() {
        busss.detach();
    })
}
export default websocket;