import express from 'express';
import * as http from 'http';
import WebSocket from 'ws';

export interface Listener {
    /**
     * The interface address
     */
    ConnectedInterface: string,
    /**
     * The port
     */
    Port: number
}

export interface EncapsulatedDatasocket {
    Data: any,
    Query?: string,
    QueryId?: string
}

export class WSHook {

    /**
     * The connected interface object
     */
    ConnectionData: Listener;

    WebServer = express();
    WServer = http.createServer(this.WebServer);
    WebSocketServer = new WebSocket.Server({ server: this.WServer });
    /**
     * 
     * @param iface The interface to connect to.
     * @param port The port to listen on.
     */
    constructor(iface: string = "localhost", port: number) {
        this.ConnectionData = {
            ConnectedInterface: iface,
            Port: port
        };

    }

    /**
     * Array of sockets connected to the websocket.
     */
    Connections: Array<WSConnection> = [];

    /**
     * Listens to the websocket
     * @param callback Callback to return to after the listener
     */
    Listen(callback?: () => void) {
        this.WServer.listen(this.ConnectionData.Port, () => {
            this.WebSocketServer.on("connection", (ws: WebSocket) => {
                var connection = new WSConnection(ws)
                this.Connections.push(connection);
                ws.on("close", () => {
                    let connectionId: number = -1;
                    for (var i in this.Connections) {
                        if (this.Connections[i] == connection)
                            connectionId = Number(i);
                    }
                    if (connectionId) {
                        this.Connections.splice(connectionId, 1);
                    }
                });
            })
            if (callback)
                callback()
        });
    }

    OnConnection(callback: (connection: WSConnection) => void) {
        this.WebSocketServer.on("connection", (ws: WebSocket) => {
            callback(new WSConnection(ws));
        });
    }
}

export class WSClient {
    /**
     * Connects to a given websocket server running our data encapsulation
     * @param hostname Hostname of the server
     */
    static Connect(hostname: string): WSConnection {
        return new WSConnection(new WebSocket(hostname));
    }
}

/**
 * A websocket connection class that has more stuff tacked onto it.
 */
export class WSConnection {
    ws: WebSocket;
    constructor(ws: WebSocket) {
        this.ws = ws;
    }

    /**
     * On the closing of the connection
     * @param callback The callback
     */
    OnClose(callback: () => void) {
        this.ws.on("close", () => callback);
    }

    /**
     * Sends data through the connection
     * @param data Data to send
     */
    Send(data: any) {
        let sdata: EncapsulatedDatasocket = { Data: data };
        let stringified: string = JSON.stringify(sdata);
        this.ws.send(stringified);
    }

    /**
     * Handles a message
     * @param callback Handler for the message
     */
    OnMessage(callback: (data: any) => void) {
        this.ws.on("message", (innerdata: any) => {
            let xdata = <EncapsulatedDatasocket>JSON.parse(innerdata);
            if (!xdata.Query)
                callback(xdata.Data);
        });
    }

    /**
     * Queries the other end and returns a promise containing the other end's result
     * @param query The query type to run
     * @param data The data to send
     */
    async Query(query: string, data: any): Promise<any> {
        let send: EncapsulatedDatasocket = {
            Query: query,
            Data: data,
            QueryId: makeid(6)
        }
        return new Promise((resolve, reject) => {
            this.ws.send(JSON.stringify(send));
            this.ws.on("message", (inner: any) => {
                let xData = <EncapsulatedDatasocket>JSON.parse(inner);
                if (xData.Query == query && xData.QueryId == send.QueryId)
                    resolve(xData.Data);
            });
        })
    }

    /**
     * Handles an incoming query
     * @param query The query type
     * @param callback The callback handler of the query, whatever is returned in the callback will be sent to the other end.
     */
    OnQuery(query: string, callback: (data: any, res:QuerySys) => any) {
        this.ws.on("message", (inner: any) => {
            let xData = <EncapsulatedDatasocket>JSON.parse(inner);
            if (xData.Query == query) {
                if (xData.QueryId) {
                    callback(xData.Data, new QuerySys(this.ws, xData.Query, xData.QueryId));
                }
            }
        });
    }

    /**
     * Due to unique instances being made and stored in the WSHook.Connections table and the OnConnection handler, this tests for if an instance equals it
     * @param other The other connection to evaluates
     */
    equals(other: WSConnection): boolean {
        return other.ws == this.ws;
    }
}

export class QuerySys {
    socket:WebSocket;
    queryString:string;
    queryId:string;
    constructor(ws:WebSocket, qs:string, qi: string) {
        this.socket = ws;
        this.queryString = qs;
        this.queryId = qi;
    }

    Reply(data:any){
        let ds: EncapsulatedDatasocket = {
            Data: data,
            Query: this.queryString,
            QueryId: this.queryId
        }
        this.socket.send(JSON.stringify(ds));
    }
}

function makeid(length: number) {
    var result = '';
    var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    var charactersLength = characters.length;
    for (var i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    var time = new Date();
    return result + time.getMilliseconds();
}