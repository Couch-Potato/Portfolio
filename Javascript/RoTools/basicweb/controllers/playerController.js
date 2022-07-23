import { ControllerInstance, Message } from "./controller.js";
import serverx from "../data/models/server.js";
export class PlayerJoinHandler extends ControllerInstance {
    /**
     * @param {Message} message 
     */
    async Handle(message) {
        serverx.playerJoin(message.Data.username, message.Data.userid, this.Bus.ServerId);
    }
}
export class PlayerLeaveHandler extends ControllerInstance {
    /**
     * @param {Message} message 
     */
    async Handle(message) {
        serverx.playerLeave(message.Data.userid, this.Bus.ServerId);
    }
}
export class Heartbeat extends ControllerInstance {
    /**
     * @param {Message} message
     */
    async Handle(message) {
        var server = await message.Bus.GetServerRecord();
        server.doHeartbeat();
    }
}