import express from "express"
import api from "../data/models/api.js";
import moderation from "../data/models/moderation.js";
import server from "../data/models/server.js";
import {playerLog, serverLog} from "../data/models/logs.js";
var app = express.Router();

app.use(async (req,res,next)=>{
    const user_id = req.headers.client;
    const token = req.headers.token;
    if (user_id && token) {
        var client = await api.authenticate(user_id, token);
        if (client) {
            req.isAuthenticated = true;
            req.client = client;
            res.sjson = (objs) => {
                if (!objs.status)
                    objs.status = 200;
                res.send(JSON.stringify(objs))
            }
            try {
                next();
            }catch {
                res.send(JSON.stringify({
                    status:500,
                    message:"An error occured when trying to process your request."
                })).status(500)
            }
            
        }else {
            return res.send(JSON.stringify({
                status:401,
                message:"Authentication failed."
            })).status(401);
        }
        
    }else {
        return res.send(JSON.stringify({
            status:401,
            message:"No authentication."
        })).status(401);
    }
});

app.get("/info", (req,res)=>{
    res.send(JSON.stringify({
        client:req.headers.client,
        username:req.client.creator.username,
        server_time:new Date(),
        version:{
            major:1,
            minor:0,
            patch:0
        }
    }));
});

app.get("/servers", async (req,res)=>{
    var servers = await server.getServerIds();
    return res.send(JSON.stringify(servers));
});

app.get("/servers/:id", async (req,res)=>{
    if (req.params.id.includes("-"))
    {
        // is a roblox server
        var svr = await server.findByRoblox(req.params.id);
        res.sjson(svr);
    }else {
        try {
            var svr = await server.findById(req.params.id);
            if (!svr) {
                res.sjson({
                    status: 404,
                    message: "Server not found."
                });
            }
            res.sjson(svr);
        }catch {
            res.sjson({
                status:404, 
                message:"Server not found."
            });
        }
        
    }
});

// app.get("/players",(req,res)=>{
//     // do not setup, will be disabled 
//     
// });

app.get("/players/:playerid" , async (req,res)=>{
    var mod = await moderation.findByRoblox(req.params.id);
    if (!mod) {
        return res.sjson({
            status: 404,
            message: "Player not found."
        });
    }
    res.sjson(mod);
});

app.get("/logs", async (req,res)=>{
    const limit = req.query.limit ? req.query.limit : 100
    const player  = req.query.player;
    const server = req.query.server;
    var logs = [];
    if (player) {
        logs = await playerLog.getLogsForPlayer(player, limit);
    }else if (server) {
        logs = await serverLog.getLogsForServer(server, limit);
    }else {
        logs = await serverLog.getLogs(limit);
    }
    res.sjson(logs);
})

app.get("/logs/:type", async (req,res)=>{
    const limit = req.query.limit ? req.query.limit : 100
    const player  = req.query.player;
    const server = req.query.server;
    const logType = req.params.type;

    var logs = [];
    if (player) {
        logs = await playerLog.getLogsForPlayer(player, limit, logType);
    }else if (server) {
        logs = await serverLog.getLogsForServer(server, limit, logType);
    }else {
        logs = await serverLog.getLogs(limit, logType);
    }
    res.sjson(logs);
});

export default app;