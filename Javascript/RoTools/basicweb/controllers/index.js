import { ControllerInstance } from "./controller.js";
import { IssueHandler, LogHandler } from "./issueLogController.js";
import { Heartbeat, PlayerJoinHandler, PlayerLeaveHandler } from "./playerController.js";

/**
 * @type {Array<{channel:string, factory:ControllerInstance}>}
 */
export default [
    {
        channel:"example",
        factory:ControllerInstance
    },{
        channel:"player_join",
        factory: PlayerJoinHandler
    }, {
        channel:"player_leave",
        factory: PlayerLeaveHandler
    }, {
        channel:"issue",
        factory: IssueHandler
    }, {
        channel:"log",
        factory: LogHandler
    }, {
        channel:"heartbeat",
        factory:Heartbeat
    }
]