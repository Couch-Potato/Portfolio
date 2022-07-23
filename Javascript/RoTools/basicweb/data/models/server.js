import mongoose from "mongoose";
import { RandomString } from "../../util.js";
import messagebus from "../models/messagebus.js"
import moderation from "./moderation.js";

const customMacro = new mongoose.Schema({
    action:String,
    color:String,
    title:String,
    description:String,
});

const playerList = new mongoose.Schema({
    user_id:Number,
    username:String,
});

var schema = new mongoose.Schema({
    roblox_id:String,
    players:[playerList],
    current_players:Number,
    max_players:Number,
    macros:[customMacro],
    last_updated:Date,
    message_bus:{type: mongoose.Schema.Types.ObjectId, ref:"MessageBus"},
    inner_heartbeat:Date,
    server_token:String
});

/**
 * Get a list of all servers
 */
schema.statics.getServers = async function () {
    return await this.find({});
}

schema.statics.getServerIds = async function(){
    return await this.find({}).select("_id roblox_id");
}

/**
 * Get a single server
 * @param {Number} id Server id
 */
schema.statics.findByRoblox = async function (id) {
    return await this.findOne({ roblox_id: id });
}


/**
 * Get player list from specific server
 * @param {Number} id Server id
 */
schema.statics.getPlayers = async function (id) {
    return await this.find({ roblox_id: id }, 'players').exec();
}

schema.statics.findServerWithPlayer = async function (player) {
    return await this.findOne({ "players.username": player, inner_heartbeat: { $gte: AddMinutesToDate(new Date(), -5).toISOString()}});
}
schema.statics.findServerWithPlayerId = async function (player) {
    return await this.findOne({ "players.user_id": player, inner_heartbeat: { $gte: AddMinutesToDate(new Date(), -5).toISOString() } });
}

schema.statics.playerJoin = async function(username, user_id, server) {
    var svr = await this.findById(server);
    for (var py of svr.players) {
        if (py.user_id == user_id) return;
    }
    svr.players.push({
        username:username,
        user_id:user_id
    });
    await svr.save();
    await moderation.addServerToPlayer(server, user_id);
}

schema.methods.isActive = function(){
    if (!this.inner_heartbeat) return false;
    var hbXpire = AddMinutesToDate(this.inner_heartbeat, 5);
    return hbXpire.getTime() > new Date().getTime()
}

schema.statics.getActiveServers = async function(){
    return await this.find({
        inner_heartbeat: {$gte:AddMinutesToDate(new Date(), -5).toISOString()}
    }).exec();
}
schema.statics.getActiveServersMinor = async function(){
    return await this.find({
        inner_heartbeat: { $gte: AddMinutesToDate(new Date(), -5).toISOString() }
    }).select("roblox_id _id max_players players last_updated")
}

schema.statics.anyActiveServers = async function(){
    var sv = await this.find({
        inner_heartbeat: { $gte: AddMinutesToDate(new Date(), -5).toISOString() }
    }).select("_id").limit(1).exec();
    return sv.length > 0;
}

schema.statics.getActiveServersAmt = async function(){
    var sv = await this.find({
        inner_heartbeat: { $gte: AddMinutesToDate(new Date(), -5).toISOString() }
    }).select("_id").exec();
    return sv.length ? sv.length : 0;
}

schema.methods.doHeartbeat = async function(){
    this.inner_heartbeat = new Date();
    await this.save();
}

function AddMinutesToDate(date, minutes) {
    return new Date(date.getTime() + minutes * 60000);
}
schema.statics.playerLeave = async function(user_id, server) {
    var svr = await this.findById(server);
    var idx = -1;
    for (var i in svr.players) {
        var pI = svr.players[i]
        if (pI.user_id == user_id)
            idx = i
    }
    if (idx > -1) {
        svr.players.splice(idx, 1);
        await svr.save();
    }
}

/**
 * Get macro list from specific server
 * @param {Number} id Server id
 */
schema.statics.getMacros = async function (id) {
    return this.find({ roblox_id: id }, 'macros').exec();
}

/**
 * Creates a new macro
 * @param {String} action The action the macro will do
 * @param {String} color The color
 * @param {String} title The title of the macro
 * @param {String} description The description of the macro
 */
schema.methods.createMacro = async function (action, color, title, description) {
    this.macros.push({
        action:action,
        color:color,
        title:title,
        description:description,
    });
    await this.save();
}

schema.methods.getBus = async function() {
    return await mongoose.model("MessageBus").getBus(this.message_bus);
}
schema.methods.getBusForRoblox = async function() {
    return await mongoose.model("MessageBus").getRobloxBus(this.message_bus);
}

schema.statics.createServer = async function(uuid, max_players, macros) {
    var token = RandomString(32);
    var serverdoc = await this.create({
        roblox_id:uuid, 
        players:[],
        current_players:0,
        max_players:max_players,
        macros:macros,
        last_updated:new Date(),
        inner_heartbeat:new Date(),
        server_token:token
    });
    var message_bus = await messagebus.createBus(serverdoc._id);
    return {
        token:token, 
        id: serverdoc._id
    };
}

schema.methods.resolvePlayerUsername = async function(userid){
    for (var p of this.players) {
        if (p.user_id == userid)
            return p.username;
    }
}

schema.methods.deleteBus = async function(){
    var bus =  await mongoose.model("MessageBus").findById(this.message_bus);
    delete global.BUSHANDLERS[String(this.message_bus)];
    await bus.delete();

}

export default mongoose.model("Server", schema);