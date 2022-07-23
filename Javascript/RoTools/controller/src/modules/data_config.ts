import mongodb,{MongoClient} from "mongodb"
import {env, getConnectionString} from "../env.js";

export class DataConfigurator{
    static async CreateDatabase(name:string, username:string, password:string):Promise<void> {
        return new Promise((resolve, reject)=>{
            mongodb.MongoClient.connect(getConnectionString(name), (err, client) => {
                if (err) return reject(err);
                var db = client.db(name);
                var adminDb = db.admin();

                adminDb.addUser(username, password, {
                    roles:[{
                        role: "readWrite",
                        db:name,
                    }],
                    fsync:false
                }, (errx,res)=>{
                    if (errx) return reject(errx);
                    resolve();
                });
            })
        })
        
    }
    static async DeleteDatabase(name:string, dbUser:string):Promise<void> {
        return new Promise((resolve,reject)=>{
            mongodb.MongoClient.connect(getConnectionString(name), async (err,client)=>{
                if (err) return reject(err);
                var db = client.db(name);
                var adminDb = db.admin();
                adminDb.removeUser(dbUser, async (errx,resx)=>{
                    if (errx) return reject(errx);
                    await db.dropDatabase();
                    resolve();
                })
            });
        })
    }
}