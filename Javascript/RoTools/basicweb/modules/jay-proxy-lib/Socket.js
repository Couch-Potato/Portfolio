import { RandomString } from "../../util.js";

export class SocketManager {
    #Sockets = {};
    /**
     * Adds a socket to the manager
     * @param {HSocket} socket The socket.
     */
    AddSocket(socket){
        this.#Sockets[socket.ServerId] = socket;
    }
    /**
     * Gets a socket
     * @param {String} id The Socket ID
     * @returns {HSocket}
     */
    GetSocket(id){
        return this.#Sockets[id];
    }
    CloseSocket(id) {
        delete this.#Sockets[id];
    }
}
export class HSocket {
    /**
     * @type {Array<SocketMessage>} 
     */
    OutstandingMessages =[];
    ServerId;


    Handlers = {
        /**
         * @type {Array<Handler>}
         */
        MessageIn : [],
        /**
         * @type {Array<Handler>}
         */
        MessageOut : []
    };

    /**
     * Polls a socket for any data. (Used by API Handler)
     * @returns {Promise<Array<SocketMessage>>}
     */
    async Poll(){
        return new Promise((resolve, reject) =>{
            var handler = new Handler(this, (data)=>{
                resolve(data);
                handler.Dispose();
            }, "in");
            this.Handlers.MessageIn.push(handler);
        })
    }

    /**
     * Posts data to be reviewed by the RoTools client
     * @param {any} data The data
     */
    Post(data) {
        for (var x of this.Handlers.MessageOut) {
            x.Call(data);
        }
    }

    /**
     * Queries the roblox server for data to be returned.
     * @param {any} data The data
     * @returns {Promise<any>} The data
     */
    async Query(data){
        var random = RandomString(15);
        data.query_id = random;
        this.Publish(data);
        return new Promise((resolve)=>{
            var handler = this.OnMessage((data) => {
                if (data.query_id == random) {
                    delete data.query_id;
                    resolve(data);
                    handler.Dispose();
                }
            })
        })
    }

    /**
     * Listens for a message transmistted by the ROBLOX.
     * @param {Function} cb Callback
     * @returns {Handler} The handler made.
     */
    OnMessage(cb){
        var handler = new Handler(this, cb, "out");
        this.Handlers.MessageOut.push(handler);
        return handler;
    }
    /**
     * 
     * @param {any} data 
     */
    Publish(data){
        this.OutstandingMessages.push(data);
        for (var x of this.Handlers.MessageIn)
        {
            x.Call(data);
        }
    }

    constructor(id){
        this.ServerId = id;
    }

}

export class SocketMessage {

}
export class Handler {
    #Inner;
    /**
     * @type {HSocket}
     */
    #Socket;
    /**
     * 
     * @param {HSocket} socket The socket it is parented to 
     * @param {Function} inner The callback
     */
    constructor(socket, inner, type){
        this.#Socket = socket;
        this.#Inner = inner;
        this.HandlerType = type;
    }

    HandlerType;
    /**
     * Disposes of a handler
     */
    Dispose(){
        if (this.HandlerType == "in"){
            for (var i of this.#Socket.Handlers.MessageIn) {
                (this.#Socket.Handlers.MessageIn[i] == this)
                {
                    this.#Socket.Handlers.MessageIn.splice(i, 1);
                }
            }
        }else {
            for (var i of this.#Socket.Handlers.MessageOut) {
                (this.#Socket.Handlers.MessageOut[i] == this)
                {
                    this.#Socket.Handlers.MessageOut.splice(i, 1);
                }
            }
        }
        
    }
    Call(data){
        this.#Inner(data);
    }
}