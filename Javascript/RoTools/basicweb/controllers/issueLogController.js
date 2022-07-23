import { ControllerInstance, Message } from "./controller.js";
import {playerLog, serverLog} from "../data/models/logs.js";
import issue from "../data/models/issues.js";
export class IssueHandler extends ControllerInstance {
    /**
     * @param {Message} message 
     */
    async Handle(message) {
        issue.reportIssue(message.Data);
    }
}
export class LogHandler extends ControllerInstance {
    /**
     * @param {Message} message 
     */
    async Handle(message) {
        if (message.Data.isPlayer) {
            var rblx = await message.Bus.GetServerRecord();
            playerLog.create({
                server: message.Bus.ServerId, 
                server_roblox: rblx.roblox_id,
                message: message.Data.message,
                stacktrace: message.Data.stacktrace,
                object_data: message.Data.object_data,
                type: message.Data.type,
                player_id: message.Data.player_id, 
                player_username: rblx.resolvePlayerUsername(message.Data.player_id),
                date:new Date()
            })
        }else {
            serverLog.create({
                server: message.Bus.ServerId,
                message: message.Data.message,
                stacktrace: message.Data.stacktrace,
                object_data: message.Data.object_data,
                type: message.Data.type,
                date: new Date()
            })
        }
    }
}