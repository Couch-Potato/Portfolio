import mongoose from "mongoose";

var player_log = new mongoose.Schema({
    server: { type: mongoose.Schema.Types.ObjectId, ref: "Server" },
    server_roblox: String,
    message: String,
    stacktrace: String,
    object_data: Object,
    type: String,
    player_id: Number,
    player_username: String,
    date: Date
});

player_log.statics.getLogsForPlayer = async function (player, limit, type = undefined) {
    if (type) {
        var query = this.find({ player_id: player, type: type }).limit(limit).lean().sort({ date: -1 });
        return await query.exec();
    }
    var query = this.find({ player_id: player }).limit(limit).lean().sort({ date: -1 });
    return await query.exec();
}
player_log.statics.getLogsForServer = async function (server, limit) {
    var query = this.find({ server: server }).limit(limit).lean().sort({ date: -1 });
    return await query.exec();
}

export let playerLog = mongoose.model("PlayerLog", player_log);

const server_log = new mongoose.Schema({
    server: { type: mongoose.Schema.Types.ObjectId, ref: "Server" },
    message: String,
    stacktrace: String,
    object_data: Object,
    type: String,
    date: Date
})

server_log.statics.getLogsForServer = async function (server, limit, type = undefined) {
    if (type) {
        var query = this.find({ server: server, type:type }).limit(limit).lean().sort({ date: -1 });
        return await query.exec();
    }
    var query = this.find({ server: server }).limit(Number(limit)).lean().sort({ date: -1 });
    return await query.exec();
}

server_log.statics.getLogs = async function (limit, type = undefined) {
    if (type) {
        var query = this.find({ type: type }).limit(limit).lean().sort({ date: -1 });
        return await query.exec();
    }
    var query = this.find({}).limit(Number(limit)).lean().sort({ date: -1 });
    return await query.exec();
}



export let serverLog = mongoose.model("ServerLog", server_log)