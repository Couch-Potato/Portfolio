import mongoose from "mongoose";

const modHistory = new mongoose.Schema({
    date:Date,
    moderator: {type:mongoose.Schema.Types.ObjectId, ref: "User"},
    /**
     * User the .populate() function to resolve all refs.
     * Ex: a.findOne({xyz}).populate("moderator")
     */
    type : Number,
    reason: String,
})

var schema = new mongoose.Schema({
    user_id:Number,
    recent_servers:Array,
    banned:{type: Boolean, default:false},
    ban_reason: { type: String, default: "" },
    ban_til_date: {type:Date},
    ban_date: { type: Date},
    history:[modHistory]
});
/**
 * Bans a user
 * @param {Number} length Length in ms
 * @param {String} reason The reason of the ban
 * @param {String} moderator The moderator 
 */
schema.methods.ban = async function (length, reason, moderator) { // DO NOT USE ARROW FUNCTIONS See: https://mongoosejs.com/docs/guide.html#methods
    this.history.push({
        moderator: moderator,
        date: new Date(),
        type: 1,
        reason: reason
    });
    this.ban_reason = reason
    var tillDate = new Date();
    tillDate.setMinutes(tillDate.getMinutes() + length);
    this.ban_til_date = tillDate;
    this.ban_date = new Date()
    this.banned = true;
    await this.save();
}

schema.methods.warn = async function(reason, moderator) {
    this.history.push({
        moderator: moderator,
        date: new Date(),
        type: 2,
        reason: reason
    });
    await this.save();
}

schema.methods.kick = async function (reason, moderator) {
    this.history.push({
        moderator: moderator,
        date: new Date(),
        type: 0,
        reason: reason
    });
    await this.save();
}

schema.statics.addServerToPlayer = async function(server, player_id) {
    var precord = await this.findOne({user_id:player_id});
    if (!precord) {
        precord = await this.create({
            userid:player_id,
            history:[],
            recent_servers:[]
        })
    }
    precord.recent_servers.push(server);
    await precord.save();
}

schema.methods.getLogs = async function (length = 1000) {
    var plogs = mongoose.model("PlayerLog");
    return await plogs.getLogsForPlayer(this.user_id, length)
}

schema.statics.getModerationHistory = async function(robloxId) {
    return (await this.findOne({user_id:robloxId}).populate("history.moderator").exec()).history;
}

schema.statics.findByRoblox = async function(robloxId) {
    return await this.findOne({
        user_id:robloxId
    });
}

export default mongoose.model("Moderation", schema);