import example from "./example.js";
import server from "./server.js";
import logs from "./logs.js";
import issues from "./issues.js";
import players from "./player.js";
import api from "./api.js";
import game from "./game.js";
import settings from "./settings.js";
import users from "./user.js";
import onboarding from "./onboarding.js";
import login from "./login.js";
import actions from "./action.js";
/**
 * @type {Array<{endpoint:String, module:any, authentication:boolean}>}
 */
export default [
   
    {
        endpoint:"/server",
        module: server,
        authentication: true
    },
    {
        endpoint: "/logs",
        module: logs,
        authentication: true
    },
    {
        endpoint: "/issues",
        module: issues,
        authentication: true
    },
    {
        endpoint:"/players",
        module: players,
        authentication: true
    },
    {
        endpoint:"/api",
        module:api,
        authentication:false
    }, 
    {
        endpoint:"/game",
        module:game,
        authentication:false
    },
    {
        endpoint:"/settings",
        module:settings,
        authentication:true
    },
    {
        endpoint:"/users",
        module:users,
        authentication: true
    },
    {
        endpoint:"/onboarding",
        module:onboarding,
        authentication:false   
    }, {
        endpoint:"/auth",
        module:login,
        authentication:false
    }, {
        endpoint:"/actions",
        module:actions,
        authentication:true
    },
    {
        endpoint: "/",
        module: example,
        authentication: true
    },
]