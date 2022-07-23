// import * as ApiClient from 'kubernetes-client';
// const Client = ApiClient.Client1_13;
// const client = new Client({ version: '1.13' });

// export interface IDeploymentSpec {
//     image:string,
//     replicas:number,
//     ports:Array<number>,
//     containerName:string,
//     deploymentName:string
// }

// export class Namespace {
//     Name:string;
//     IsAvailable = false;
//     constructor(nsName:string) {
//         this.Name = nsName;
//     }
//     async Delete(){
//         if (this.IsAvailable) {
//             var ns = await client.api.v1.namespaces(this.Name).delete();
//         }
//     }
//     async Read(){
//         await client.api.v1.namespaces.get()
//         var ns = client.api.v1.namespaces(this.Name)
//         if (ns)
//         {
//             this.IsAvailable = true;
//             return true;
//         }
//         return false;
//     }
//     async Create(){
//         this.IsAvailable = true;
//         await client.api.v1.namespaces.post({
//             kind:"Namespace",
//             metadata:{
//                 name:this.Name
//             }
//         });
//     }
//     async Deploy(spec: IDeploymentSpec) {
//      //   await client.api.v1.namespaces(this.Name).
//     }
//     async CreateIngress(){
//         await client.api.v1.namespaces(this.Name).ingre
//     }
// }


// export class KubernetesClient {
//     static async CreateNamespace(nsName:string):Promise<Namespace> {
//         var ns = new Namespace(nsName);
//         ns.Create();
//         return ns;
//     }
//     static async GetNamespace(nsName:string):Promise<Namespace | undefined> {
//         var ns = new Namespace(nsName);
//         await ns.Read();
//         if (ns.IsAvailable)
//             return ns;
//         else
//             return;
//     }
// }

