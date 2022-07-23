import mongoose from "mongoose";

export class ControllerInstance {
    /**
     * @type {String}
     */
    Channel;

    /**
     * @type {MessageBus}
     */
    Bus;

    constructor(channel, bus){
        this.Channel = channel
        this.Bus = new MessageBus(bus);
    }

    /**
     * Handles an incoming message from a bus
     * @param {Message} message 
     */
    Handle(message){}

    static GetController(channel, messageBus){
        return new this(channel, messageBus);
    }
}
export class MessageBus {
    /**
     * Reads data from a bus.
     */
    async Read() {
        return await this.#busStream.read();
    }
    /**
     * Writes data to a bus
     * @param {any} data data
     */
    async Write(data){
        return await this.#busStream.write(data);
    }
    /**
     * Glances at a bus
     */
    async Glance(){
        return await this.#busStream.glance();
    }
    /**
     * Removes an item from the bus.
     * @param {number} id The item to clear from the bus channel
     */
    async ClearOne(id){
        return await this.#busStream.clearOne();
    }
    async GetServerRecord(){
        return await mongoose.model("Server").findById(this.ServerId);
    }
    /**
     * Writes data to the bus channel.
     * @param {string} channel The channel
     * @param {any} data The data
     */
    async Send(channel, data) {
        return await this.Write({
            channel:channel, 
            data:data
        })
    }
    /**
     * @type {mongoose.ObjectId}
     */
    ServerId;
    
    /**
     * @type {mongoose.ObjectId}
     */
    MessageBusId;
    /**
     * @type {mongoose.Document}
     */
    #innerbus;
    #busStream;
    constructor(bus){
        this.#innerbus = bus;
        this.ServerId = bus.server;
        this.MessageBusId = bus._id;
        this.#busStream = bus.getBusStream();
    }
}

export class Message {
    /**
     * @type {MessageBus}
     */
    Bus;


    Data;

    /**
     * @type {string}
     */
    Channel;

    Send(data, channel = this.Channel) {
        this.Bus.Send(channel, data);
    }

    constructor(channel, data, bus) {
        this.Channel = channel;
        this.Data = data;
        this.Bus = bus;
    }
}