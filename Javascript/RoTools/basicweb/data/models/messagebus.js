import mongoose from "mongoose";
import { PersistentBusController } from "../../controllers/persistentBusController.js";
import { WSConnection } from "../../modules/wshook.js";
import server from "./server.js";
var bus = new mongoose.Schema({
    server: { type: mongoose.Schema.Types.ObjectId, ref: "Server" },
    in: Array,
    out: Array,
    owner: String,
    roblox_id: String,
    last_message_time: Date
});


bus.statics.createBus = async function (serverId) {
    var ownerName = "DEBUG-ALL";
    if (global.CONFIGSOCKET) {
        ownerName = process.env.HOSTNAME;
    }
    var svrm = mongoose.model("Server");
    var svr = await svrm.findById(serverId);
    var bus = await this.create({
        server: serverId,
        in: [],
        out: [],
        owner: ownerName,
        roblox_id: svr.roblox_id,
        last_message_time: new Date()
    });


    svr.message_bus = bus._id;
    await svr.save();
    PersistentBusController.AttachToBus(bus);
    return bus;
}
bus.methods.getServer = async function () {
    return await server.findById(this.server);
}
global.BUSHANDLERS = {}
bus.methods.getBusStream = function () {
    var record = this;
    return {
        read: async () => {
            var x = await mongoose.model("MessageBus").findById(record._id);
            var nOut = x.out;
            x.out = [];
            await x.save();
            return nOut;
        },
        write: async (item) => {
            var x = await mongoose.model("MessageBus").findById(record._id);
            x.in.push(item);
            await x.save();
        },
        attach: (cb) => {
            var id = 0
            if (global.BUSHANDLERS[String(this._id)]) {
                id = global.BUSHANDLERS[String(this._id)].push(cb) - 1;
            } else {
                global.BUSHANDLERS[String(this._id)] = [cb]
            }
            return {
                busId: id,
                detach: function () {
                    global.BUSHANDLERS[String(this._id)].splice(id, 1);
                }
            }
        },
        glance: async () => {
            var x = await mongoose.model("MessageBus").findById(record._id);
            return x.out;
        },
        clearOne: async (index) => {
            var x = await mongoose.model("MessageBus").findById(record._id);
            if (index > -1) {
                x.out.splice(index, 1);
            }
            await x.save();
        }
    }
}
bus.methods.tx = async function (write) {
    var x = await mongoose.model("MessageBus").findById(record._id);
    x.out.push(write);
    x.last_message_time = new Date();
    await x.save();
}
bus.methods.rx = async function () {
    var x = await mongoose.model("MessageBus").findById(record._id);
    var nOut = x.in;
    x.in = [];
    await x.save();
    return nOut;
}

bus.statics.getBus = async function (id) {
    var x = await this.findById(id);
    return x.getBusStream();
}
bus.statics.getRobloxBus = async function (id) {
    return await this.findById(id);
}

bus.statics.readAll = async function () {
    var bus = []
    var messages = await this.find({ in: { $exists: true, $ne: [] } });
    for (var i of messages) {
        bus.push({
            server: i.roblox_id,
            messages: i.in
        });
        i.in = [];
        i.save();
    }
    return bus;
};
bus.statics.writeAll = async function (data) {
    for (var message of data) {
        var bus = await this.findOne({
            roblox_id: message.server
        });
        if (bus) {
            for (var mg of message.messages) {
                bus.out.push(mg);
            }
            bus.save();
        }
    }
}
const busModel = mongoose.model("MessageBus", bus)
/**
 * Restores bus ownership to a given server if a bus happens to become unattached due to a crash.
 */
export async function restoreBusOwnership() {
    /**
     * @type {WSConnection}
     */
    var connection = global.CONFIGSOCKET;
    var busses = await mongoose.model("MessageBus").find({}).select("_id owner").exec();
    var instancesRunning = await connection.Query("get-instances");
    console.log("Getting bus ownership data.")
    for (var bus of busses) {
        var isOwnedByActiveInstance = false;
        for (var instance of instancesRunning) {
            if (bus.owner == instance) {
                isOwnedByActiveInstance = true;
            }
        }
        var sv = await bus.getServer();
        if (!isOwnedByActiveInstance && sv.isActive()) {
            console.log(`[BUS] Taking ownership of bus:` + bus._id);
            var betterBusAttacher = await busModel.findById(bus._id);
            betterBusAttacher.owner = process.env.HOSTNAME;
            await betterBusAttacher.save();
            PersistentBusController.AttachToBus(betterBusAttacher);
        }
    }
}



mongoose.model("MessageBus").watch().on("change", (data) => {
    if (data.operationType == "update") {
        var busA = global.BUSHANDLERS[String(data.documentKey._id)]
        if (data.updateDescription.updatedFields.hasOwnProperty("out")) {
            for (var handler of busA)
                handler()
        }
    }
});
function AddMinutesToDate(date, minutes) {
    return new Date(date.getTime() + minutes * 60000);
}
// Retire old busses
setInterval(async () => {
    var busses = await busModel.find({ last_message_time: { $lte: AddMinutesToDate(new Date(), -5).toISOString() } });
    for (var b of busses) {
        delete global.BUSHANDLERS[String(b._id)];
        await b.delete();
    }
}, 600000)
export default busModel;