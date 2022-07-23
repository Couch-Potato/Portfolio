"use strict";
import mongoose from "mongoose"
import express from "express"
import routes from "./routes/index.js"
import dotenv from "dotenv"
import middleware from "./middleware.js"
import cookie_parser from "cookie-parser"
import http from "http";
import WebSocket from "ws";
import ws_routes from "./routes/websocket.js";
import { WSClient, WSConnection } from "./modules/wshook.js";
import { restoreBusOwnership } from "./data/models/messagebus.js";
import fs from "fs";
dotenv.config();
var lwAuthRoutes = [];

dotenv.config();

var app = express()

app.use(cookie_parser())
/**
 * Makes a lightweight set of routes for the purpose of reviewing auth data.
 */
for (const x of routes) {
  lwAuthRoutes.push({
    endpoint: x.endpoint,
    authentication: x.authentication
  });
}
/**
 * Handles putting in the routing data so we can parse it later on in our auth system.
 */
app.use((req,res,next)=>{
  req.endpoints = lwAuthRoutes;
  next();
})
app.use(middleware);

function getConnectionString()
{
    if (process.env.IS_DEBUG) {
      return `mongodb://${process.env.DBUSER ? process.env.DBUSER : ``}${process.env.DBPASS ? ":"+process.env.DBPASS+"@" : ``}${process.env.DBHOST}/${process.env.DBNAME}?authSource=admin`
    }else {
      return `mongodb://${process.env.DBUSER ? process.env.DBUSER : ``}${process.env.DBPASS ? ":" + process.env.DBPASS + "@" : ``}${process.env.DBHOST}/${process.env.DBNAME}?replicaSet=rs0&authSource=admin`
    }
}

async function attemptReconnect(){
  try {
    console.log("[RECONNECT] Reconnecting to server.")
    var connection = await WSClient.Connect(process.env.CONNECTION);
    await connection.Query("authenticate", {
      privateKey: process.env.PRIVATE_KEY,
      id: process.env.DBNAME.replace("cli-", ""),
      i_hostname: process.env.HOSTNAME
    });
    global.CONFIGSOCKET = connection;
    console.log("[RECONNECT] Connection regained.")
  }catch {
    console.log("[RECONNECT] Connection failed... retrying")
    setTimeout(attemptReconnect, 3000);
  }
}

async function authenticate(){
  if (process.env.CONNECTION) {
    console.log("Connecting to controller...");
    /**
     * @type {WSConnection}
     */
    var connection = await WSClient.Connect(process.env.CONNECTION);
    await connection.Query("authenticate", {
      privateKey: process.env.PRIVATE_KEY,
      id: process.env.DBNAME.replace("cli-",""),
      i_hostname:process.env.HOSTNAME
    });
    console.log("Got configuration file...");
    var configData = await connection.Query("get_config");
    global.GAMEKEY = configData.gameKey;
    global.PRIVATEKEY = process.env.PRIVATE_KEY;
    global.IS_CONTROLLER = true;
    global.CONTROLLER = process.env.CONNECTION
    global.CONFIGSOCKET = connection;
    global.HOSTSET = configData.hostname;
    global.INSTANCE_ID = process.env.HOSTNAME + ".cli-" + configData._id;
    restoreBusOwnership();
    connection.OnClose(()=>{
      attemptReconnect();
    })
    console.log("Connected to controller and got configuration.");
  } else {
    global.PRIVATEKEY = '123';
    global.GAMEKEY = "123"
    global.HOSTSET = "localhost:8080"
  }
  fs.readFile("version.txt", (err, data)=>{
    global.RTVERSION = data.toString();
  })
}

authenticate();

mongoose.connect(getConnectionString(), {useNewUrlParser: true, useUnifiedTopology: true});
const db = mongoose.connection;
db.on('error', console.error.bind(console, 'connection error:'));
db.once('open', function() {
  console.log('connected to db');
});

for (const x of routes)
{
    app.use(x.endpoint, x.module)
}

app.use('/assets', express.static('assets', {
  immutable: true, maxAge: 3600000}))



const server = http.createServer(app)


const wss = new WebSocket.Server({server});
wss.on("connection", /**
 * 
 * @param {WebSocket} ws 
 */
(ws, req)=>{
  for (var i in ws_routes) {
    if (("*" + req.url).includes("*" + i)){
      for (var x in ws_routes[i]){
        var nUrl = ("*" + req.url).replace("*" + i, "");
        if (("*" + nUrl).includes("*" + x)){
          var idx = ("*" + nUrl).replace("*" + x +"/", "");
          req.id = idx;
          ws_routes[i][x](ws,req);
        }
      }
    }
  }
})
server.listen(8080)


console.log('listening on port 8080');







