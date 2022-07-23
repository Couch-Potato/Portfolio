import mongoose from "mongoose";
import hash from "object-hash";

const issue = new mongoose.Schema({
    latest_server: { type: mongoose.Schema.Types.ObjectId, ref: "Server" },
    message: String,
    stacktrace: String,
    object_data: Object,
    type: String,
    begin_date: Date,
    latest_date: Date,
    hash: String,
    amount:Number,
    ignored:Boolean
})

/**
 * 
 * @param {{latest_server, message, stacktrace, object_data, type}} data The data to report.
 */
issue.statics.reportIssue = async function(data){
    var hd = {
        message: data.message,
        stacktrace: data.stacktrace,
        type: data.type,
    }
    var server = data.latest_server
    var hsh = hash(hd);
    var issu = await mongoose.model("Issue").findOne({hash:hsh});
    if (issu) {
        issu.latest_date = new Date();
        issu.amount++;
        issu.latest_server = (await mongoose.model("Server").findOne({roblox_id:server}))._id;
        await issu.save();
    }else {
        var serverId = (await mongoose.model("Server").findOne({ roblox_id: server }))._id;
        await this.create({
            latest_server:serverId,
            message:data.message,
            stacktrace:data.stacktrace,
            object_data:data.object_data,
            type:data.type,
            begin_date:new Date(),
            latest_date: new Date(),
            hash: hsh,
            amount: 1
        })
    }
}
issue.statics.getActiveIssueAmt = async function(){
    return (await this.find({ignored:false}).select("_id").exec()).length
}

export default mongoose.model("Issue", issue);