import k8s, { HttpError, V1EnvVar } from "@kubernetes/client-node";
import {env, loadEnv} from "../env.js";
const kc = new k8s.KubeConfig();
loadEnv();
if (env.IS_DEBUG) {
    console.log("Loading from default...");
    kc.loadFromDefault();
}else {
    kc.loadFromCluster();
    test();
    console.log("Loaded from cluster.");
}
async function test(){
    try {
        await k8sApi.readNamespace("default");
        return true;
    }catch (ex:any) {
        var err:HttpError = <HttpError>ex;
        console.log("KUBE Test failed: " + err.message);
        return false
    }
}

const k8sApi = kc.makeApiClient(k8s.CoreV1Api);
const netwapi = kc.makeApiClient(k8s.NetworkingV1Api);
const appapi = kc.makeApiClient(k8s.AppsV1Api) 
export interface IDeploymentSpec {
    image:string,
    replicas:number,
    selector:string,
    port:number,
    name:string,
    outsidePort:number, 
    envParams: Array<V1EnvVar>,
    hostname:string
}

export class Namespace {
    Name:string;
    IsAvailable = false;
    constructor(nsName:string) {
        this.Name = nsName;
    }
    async Delete(){
        if (this.IsAvailable) {
            return await k8sApi.deleteNamespace(this.Name);
        }
    }
    async Read(){
        var ns = await k8sApi.readNamespace(this.Name);
        if (ns.body)
        {
            this.IsAvailable = true;
            return true;
        }
        return false;
    }
    async Create(){
        var can = await test();
        if (!can) return;
        this.IsAvailable = true;
        return await k8sApi.createNamespace({
            metadata: {
                name: this.Name
            }
        })
    }
    async RollingUpdate() {
        try {
            return await appapi.patchNamespacedDeployment("game-instance-deployment", this.Name, {
                spec: {
                    template: {
                        metadata: {
                            labels: {
                                "deploy.k8s.app.rotools.net": String(new Date().getTime())
                            }
                        }
                    }
                }
            }, undefined, undefined, undefined, undefined, {
                headers:{
                    "Content-Type":"application/strategic-merge-patch+json"
                }
            })
        }catch(ex) {
            console.log(ex);
        }
        
    }
    async SetReplicas(num:number){
        try {
            return await appapi.patchNamespacedDeployment("game-instance-deployment", this.Name, {
                spec: {
                    replicas: String(num)
                }
            }, undefined, undefined, undefined, undefined, {
                headers: {
                    "Content-Type": "application/strategic-merge-patch+json"
                }
            })
        } catch (ex) {
            console.log(ex);
        }
        
    }
    async CloneSecretFromDefault(secretName: string){
        // var res = await k8sApi.readNamespacedSecret("default", secretName);
        // res.body.data?.
        await k8sApi.createNamespacedSecret(this.Name, {
            metadata: {
                name:secretName
            },
            data: {
                ".dockerconfigjson":"eyJhdXRocyI6eyJodHRwczovL2luZGV4LmRvY2tlci5pby92Mi8iOnsiVXNlcm5hbWUiOiJqYXltMjQ0IiwiUGFzc3dvcmQiOiJDb2tlZmllbGQhNjYiLCJFbWFpbCI6ImpheUBub3ZvdnUuY29tIn19fQ=="
            },
            type: "kubernetes.io/dockerconfigjson"
        });
    }
    async Deploy(spec: IDeploymentSpec) {
        await this.CloneSecretFromDefault("regcred");
        var deployment = await appapi.createNamespacedDeployment(this.Name, {
            spec:{
                replicas:spec.replicas,
                template:{
                    spec:{
                        imagePullSecrets:[{
                            name:"regcred"
                        }],
                        containers:[
                            {
                                image:spec.image,
                                name:spec.selector,
                                ports:[
                                    {containerPort:spec.port}
                                ],
                                env:spec.envParams,
                            }
                        ]
                    }, metadata: {
                        labels: {
                            "k8s.app.rotools.net":spec.selector
                        }
                    }
                },
                selector:{
                    matchLabels:{
                        "k8s.app.rotools.net":spec.selector
                    }
                },
            }, metadata: {
                labels:{
                    "k8s.app.rotools.net":spec.selector
                },
                name:spec.name
            }
        })
        var service = await k8sApi.createNamespacedService(this.Name, {
            spec:{
                type:"NodePort",
                selector: {"k8s.app.rotools.net":spec.selector},
                ports:[
                    {
                        protocol:"TCP",
                        port:spec.outsidePort,
                        targetPort:Object(spec.port)
                    }
                ]
            },metadata:{
                name:spec.selector + "-service"
            }
        })
        try {
            var ingress = await netwapi.createNamespacedIngress(this.Name, {
                spec: {
                    tls:[{
                        hosts: [spec.hostname],
                        secretName:`${this.Name}-${spec.selector}-cert`
                    }],
                    rules: [{
                        host: spec.hostname,
                        http: {
                            paths: [{
                                pathType: "Prefix",
                                path: "/",
                                backend: {
                                    service: {
                                        name: spec.selector + "-service",
                                        port: {
                                            number: spec.outsidePort
                                        }
                                    }
                                }
                            }]
                        }
                    }]
                },
                metadata: {
                    name: spec.selector + "-ingress",
                    annotations: {
                        "cert-manager.io/cluster-issuer":"letsencrypt-prod"
                    }
                }
            })
        }catch (exception) {
            console.log(exception);
        }
        

    }
}


export class KubernetesClient {
    static async CreateNamespace(nsName:string):Promise<Namespace> {
        var ns = new Namespace(nsName);
        ns.Create();
        return ns;
    }
    static async GetNamespace(nsName:string):Promise<Namespace | undefined> {
        var ns = new Namespace(nsName);
        await ns.Read();
        if (ns.IsAvailable)
            return ns;
        else
            return;
    }
}

