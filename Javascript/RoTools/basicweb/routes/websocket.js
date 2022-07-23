import {socket} from "./server.js"
import datasocket from "./datasocket.js";
export default {
    "/server": socket,
    "/api":datasocket
}