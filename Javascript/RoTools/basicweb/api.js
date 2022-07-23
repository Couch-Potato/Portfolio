import { json } from "body-parser";
import express from "express"
import { HSocket, SocketManager } from "./modules/jay-proxy-lib/Socket.js";

const manager = new SocketManager()

manager.AddSocket(new HSocket("123"));

manager.GetSocket("123").OnMessage((data)=>{
    console.log(data);
});

const app = express();

app.get("/rx", async (req,res)=>{
    var data = await manager.GetSocket(req.query.socket).Poll();
    res.send(JSON.stringify(data));
});

app.get("/tx", async(req,res)=>{
    var data = await manager.GetSocket(req.query.socket).Post({username:"hi"});
    res.send("ok.");
})

setTimeout(()=>{
    manager.GetSocket("123").Publish({user:"hello_world"})
}, 3000)

app.listen(8081);