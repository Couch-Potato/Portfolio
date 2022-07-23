import express from "express"
import api from "../data/models/api.js";
import server from "../data/models/server.js";
import bodyParser from "body-parser";
import messagebus from "../data/models/messagebus.js";
const secret_set = "123";
var app = express.Router();

app.use(bodyParser.json());

app.use((req, res, next) => {
    if (!req.headers.secret || req.headers.secret != global.GAMEKEY) {
        return res.send(JSON.stringify({
            status: 401,
            message: "Unauthorized"
        })).status(401);
    }
    next();
});

app.post("/create-server", async (req, res) => {
    const server_uuid = req.body.uuid;
    const server_signer_token = req.body.signer;
    if (!server_uuid) {
        return res.send(JSON.stringify({
            status: 400,
            message: "Malformed request."
        })).status(400);
    }

    const svrs = await server.anyActiveServers();
    if (svrs && !server_signer_token){
        return res.send(JSON.stringify({
            status: 403,
            message: "Games with more than one server online require the signature of other servers to instantiate."
        })).status(403);
    }
    
    const aToken = await server.findOne({
        server_token:server_signer_token
    })
    if (!aToken && svrs) {
        return res.send(JSON.stringify({
            status: 403,
            message: "The signing server is not valid."
        })).status(403);
    }

    const macros = req.body.macros;
    const max = req.body.max_players;
    var tkn = await server.createServer(server_uuid, max, macros);

    return res.send(JSON.stringify({
        status:200,
        token: tkn.token,
        uuid: server_uuid,
        internal_id:tkn.id
    }));
});


var validateServer = async function (req,res,next) {
    const server_uuid = req.query.uuid;
    const server_signer_token = req.query.token;
    if (!server_uuid || !server_signer_token)
    {
        return res.send(JSON.stringify({
            status: 400,
            message: "Malformed request."
        })).status(400);
    }
    const aToken = await server.findOne({
        server_token:server_signer_token,
        roblox_id:server_uuid
    })
    if (!aToken) {
        return res.send(JSON.stringify({
            status: 403,
            message: "The server token is not valid."
        })).status(403);
    }
    next();
}


app.get("/rx",validateServer, async (req,res)=>{
    var messages = await messagebus.readAll();
    return res.send(JSON.stringify({
        status:200,
        data:messages
    }))
});

app.post("/terminate", validateServer, async(req,res)=>{
    const server_uuid = req.query.uuid;
    const server_signer_token = req.query.token;
    const sv = await server.findOne({
        server_token: server_signer_token,
        roblox_id: server_uuid
    })
    if (req.body.studio) {
        sv.inner_heartbeat = new Date(0);
    } else if (sv.inner_heartbeat) {
        sv.inner_heartbeat = new Date(sv.inner_heartbeat.getTime() + -10 * 60000)
    }else {
        sv.inner_heartbeat = new Date(0);
    }

    sv.players = [];
    await sv.save();
    await sv.deleteBus();
    return res.send(JSON.stringify({status:200}))
});

app.post("/tx", validateServer, async (req,res)=>{
    if (!req.body.data)
    {
        return res.send(JSON.stringify({
            status: 400,
            message: "Malformed request."
        })).status(400);
    }
    await messagebus.writeAll(req.body.data);
    res.send(JSON.stringify({
        status:200
    }))
})

export default app;