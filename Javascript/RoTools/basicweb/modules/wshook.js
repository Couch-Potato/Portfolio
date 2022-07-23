/**
 * This is a module auto generated from the controller ts. This implements the caller API.
 */
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import express from 'express';
import * as http from 'http';
import WebSocket from 'ws';
export class WSHook {
    /**
     *
     * @param iface The interface to connect to.
     * @param port The port to listen on.
     */
    constructor(iface = "localhost", port) {
        this.WebServer = express();
        this.WServer = http.createServer(this.WebServer);
        this.WebSocketServer = new WebSocket.Server({ server: this.WServer });
        /**
         * Array of sockets connected to the websocket.
         */
        this.Connections = [];
        this.ConnectionData = {
            ConnectedInterface: iface,
            Port: port
        };
    }
    /**
     * Listens to the websocket
     * @param callback Callback to return to after the listener
     */
    Listen(callback) {
        this.WServer.listen(this.ConnectionData.Port, () => {
            this.WebSocketServer.on("connection", (ws) => {
                var connection = new WSConnection(ws);
                this.Connections.push(connection);
                ws.on("close", () => {
                    let connectionId = -1;
                    for (var i in this.Connections) {
                        if (this.Connections[i] == connection)
                            connectionId = Number(i);
                    }
                    if (connectionId) {
                        this.Connections.splice(connectionId, 1);
                    }
                });
            });
            if (callback)
                callback();
        });
    }
    OnConnection(callback) {
        this.WebSocketServer.on("connection", (ws) => {
            callback(new WSConnection(ws));
        });
    }
}
export class WSClient {
    /**
     * Connects to a given websocket server running our data encapsulation
     * @param hostname Hostname of the server
     */
    static async Connect(hostname) {
        var connection = new WSConnection(new WebSocket(hostname))
        return new Promise((res,rej)=>{
            connection.ws.on("open", () => {
                res(connection)
            })
        })
    }
}
/**
 * A websocket connection class that has more stuff tacked onto it.
 */
export class WSConnection {
    constructor(ws) {
        this.ws = ws;
    }
    /**
     * On the closing of the connection
     * @param callback The callback
     */
    OnClose(callback) {
        this.ws.on("close", () => callback);
    }
    /**
     * Sends data through the connection
     * @param data Data to send
     */
    Send(data) {
        let sdata = { Data: data };
        let stringified = JSON.stringify(sdata);
        this.ws.send(stringified);
    }
    /**
     * Handles a message
     * @param callback Handler for the message
     */
    OnMessage(callback) {
        this.ws.on("message", (innerdata) => {
            let xdata = JSON.parse(innerdata);
            if (!xdata.Query)
                callback(xdata.Data);
        });
    }
    /**
     * Queries the other end and returns a promise containing the other end's result
     * @param query The query type to run
     * @param data The data to send
     */
    Query(query, data) {
        return __awaiter(this, void 0, void 0, function* () {
            let send = {
                Query: query,
                Data: data,
                QueryId: makeid(6)
            };
            return new Promise((resolve, reject) => {
                this.ws.send(JSON.stringify(send));
                this.ws.on("message", (inner) => {
                    let xData = JSON.parse(inner);
                    if (xData.Query == query && xData.QueryId == send.QueryId)
                        resolve(xData.Data);
                });
            });
        });
    }
    /**
     * Handles an incoming query
     * @param query The query type
     * @param callback The callback handler of the query, whatever is returned in the callback will be sent to the other end.
     */
    OnQuery(query, callback) {
        this.ws.on("message", (inner) => {
            let xData = JSON.parse(inner);
            if (xData.Query == query) {
                let returnValue = callback(xData.Data);
                let ds = {
                    Data: returnValue,
                    Query: query,
                    QueryId: xData.QueryId
                };
                this.ws.send(JSON.stringify(ds));
            }
        });
    }
    /**
     * Due to unique instances being made and stored in the WSHook.Connections table and the OnConnection handler, this tests for if an instance equals it
     * @param other The other connection to evaluates
     */
    equals(other) {
        return other.ws == this.ws;
    }
}
function makeid(length) {
    var result = '';
    var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    var charactersLength = characters.length;
    for (var i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    var time = new Date();
    return result + time.getMilliseconds();
}
