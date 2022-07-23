import mongoose from "mongoose";

var pipe = new mongoose.Schema({
    server: { type: mongoose.Schema.Types.ObjectId, ref: "Server" },
    owner: { type: mongoose.Schema.Types.ObjectId, ref: "User" },
    stdin: String,
    stdout: String,
});

pipe.statics.getPipe = async function (server, owner) {
    return await this.findOne({
        server:server,
        owner:owner
    });
}

pipe.methods.writeToPipe = async function(message) {
    var bus = await (await mongoose.model("Server").findById(this.server)).getBus(); 
    await bus.write({
        channel:"terminal_pipe",
        data: {
            pipe:this._id,
            message:message
        }
    })
    // this.stdin += message;
    // await this.save();
}

pipe.methods.readFromPipe = async function() {
    var x = await mongoose.model("Pipe").findById(this._id);
    var outf = x.stdout;
    x.stdout = "";
    await x.save();
    return outf;
}

global.PIPEHANDLERS = {}

pipe.statics.createPipe = async function(server, owner) {
    var p = await this.create({
        server:server,
        owner:owner,
        stdin:"",
        stdout:""
    });
    var bus = await (await mongoose.model("Server").findById(server)).getBus(); 
    var handler = bus.attach(async ()=>{
        // We want to parse data one by one.
        var newData = await bus.glance();
        var clout = []
        for (var i in newData) {
            var x = newData[i];
            if (x.channel = "terminal_pipe") {
                if (p._id.equals(x.data.pipe)){
                    var px = await mongoose.model("Pipe").findById(p._id);
                    px.stdout += x.data.message;
                    await px.save();
                    clout.push(i);
                }
            }
        }
        // Clear any pipe messages.
        for (var x of clout) {
            await bus.clearOne(x);
        }
    })
    global.PIPEHANDLERS[String(p._id)] = handler;
    return p;
}


/**
 * Watches for when a property changes
 * @param {String} fieldName 
 * @param {Function} cb 
 */
pipe.methods.watch = function (fieldName, cb) {
    var this_id = this._id;
    mongoose.model("Pipe").watch().on("change", (data) => {
        if (data.operationType == "update") {
            if (data.documentKey._id.equals(this_id)) {
                if (data.updateDescription.updatedFields.hasOwnProperty(fieldName)) {
                    cb(data.updateDescription.updatedFields[fieldName])
                }
            }
        }
    });
}

pipe.methods.detach = function(){
    global.PIPEHANDLERS[String(this._id)].detach();
    delete global.PIPEHANDLERS[String(this._id)]
}
export default mongoose.model("Pipe", pipe);