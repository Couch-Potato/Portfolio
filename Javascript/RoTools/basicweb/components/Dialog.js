import dateFormat from "dateformat";
import moderation from "../data/models/moderation.js";
import serverx from "../data/models/server.js";
import { Modal, ModalSize } from "./Modal.js";
import { Colors } from "./Theme.js";

export class Dialog extends Modal {
    Type="default"
    constructor(title, action, referral){
        super(title);
        this.IsForm = true;
        this.FormUrl = action
        this.SetReferral(referral)
    }
    /**
     * @type {{Color:"primary" | "danger" | "warning" | "success", Text:string, Icon:string}[]}
     */
    Results = [];
    GetResult(id){
        if (this.Results.length > id){
            return this.Results[id]
        }
    }
    async Handle(req,res){

    }
    SetReferral(url){
        this.FormUrl+="?refer=" + url;
    }
}

export class BanDialog extends Dialog {
    Type = "ban"
    constructor(userid,refer) {
        super("Ban", "/actions/ban/"+userid, refer);
        this.ModalButton.Color = Colors.Danger
        this.AddElements([
            `<div class="form-group"><label>Reason</label><br/> <textarea class="form-control form-control-lg" name="reason" rows="2" spellcheck="true"></textarea> </div>`,
            `<div class="form-group"><label>Time (In Minutes)</label><br/><input type="text" name="time" class="form-control form-control-lg" placeholder="Time"> </div>`,
        ])
    }
    Results = [
        {
            Color:"success",
            Text:"User has been banned successfully.",
            Icon:"alert-circle"
        }
    ]
    /**
     * 
     * @param {Number} userid Roblox Userid 
     * @param {String} reason The reason
     * @param {Number} time The time in minutes
     * @param {String} moderator Moderator name
     */
    async Handle(userid, req) {
        var us = await moderation.findByRoblox(userid);
        var reason = req.body.reason;
        var time = req.body.time;
        var moderator = req.Client.UserId;

        await us.ban(time, reason, moderator)

        var nd = new Date();
        nd.setMinutes(nd.getMinutes() + time);
        var server = await serverx.findServerWithPlayerId(userid);
        if (server) {
            var bus = await server.getBus();
            bus.write({
                channel:"kick",
                data:{
                    userid:userid,
                    reason: `You were banned by ${req.Client.Username} for ${reason}. Your ban will expire on ${dateFormat(nd, "mm/dd/yyyy hh:MM TT")}.`
                }
            })
        }
        
        return 0;
    }
} 
export class KickDialog extends Dialog {
    Type = "kick"
    constructor(userid, refer) {
        super("Kick", "/actions/kick/" + userid, refer);
        this.ModalButton.Color = Colors.Warning
        this.AddElements([
            `<div class="form-group"><label>Reason</label><br/> <textarea class="form-control form-control-lg" name="reason" rows="2" spellcheck="true"></textarea> </div>`,
        ])
    }
    Results = [
        {
            Color: "success",
            Text: "User has been kicked successfully.",
            Icon: "alert-circle"
        }
    ]
    /**
     * 
     * @param {Number} userid Roblox Userid 
     * @param {String} reason The reason
     * @param {Number} time The time in minutes
     * @param {String} moderator Moderator name
     */
    async Handle(userid, req) {
        var reason = req.body.reason;
        var moderator = req.Client.UserId;
        var us = await moderation.findByRoblox(userid);
        if (us) {
            await us.kick(reason, moderator)
        }
        

        var server = await serverx.findServerWithPlayerId(userid);
        if (server) {
            var bus = await server.getBus();
            bus.write({
                channel: "kick",
                data: {
                    userid: userid,
                    reason: `You were kicked by ${req.Client.Username} for ${reason}.`
                }
            })
        }

        return 0;
    }
}
export class WarnDialog extends Dialog {
    Type = "warn"
    constructor(userid, refer) {
        super("Warn", "/actions/warn/" + userid, refer);
        this.ModalButton.Color = Colors.Warning
        this.AddElements([
            `<div class="form-group"><label>Reason</label><br/> <textarea class="form-control form-control-lg" name="reason" rows="2" spellcheck="true"></textarea> </div>`,
        ])
    }
    Results = [
        {
            Color: "success",
            Text: "User has been warned successfully.",
            Icon: "alert-circle"
        }
    ]
    /**
     * 
     * @param {Number} userid Roblox Userid 
     * @param {String} reason The reason
     * @param {Number} time The time in minutes
     * @param {String} moderator Moderator name
     */
    async Handle(userid, req) {
        var reason = req.body.reason;
        var moderator = req.Client.UserId;
        var us = await moderation.findByRoblox(userid);
        await us.warn(reason, moderator)

        var server = await serverx.findServerWithPlayerId(userid);
        if (server) {
            var bus = await server.getBus();
            bus.write({
                channel: "warn",
                data: {
                    userid: userid,
                    reason: `You were warned by ${req.Client.Username} for ${reason}.`
                }
            })
        }

        return 0;
    }
}
export class CreateUserDialog extends Dialog {
    constructor() {
        super("Create User", "/users/create", "/users");
        this.AddElements([
            `<div class="form-group"><label>Username</label><input type="text" class="form-control form-control-lg" name="username" placeholder="Roblox Username"> </div>`,
            `<div class="form-group"><label>Email Address</label><input type="text" class="form-control form-control-lg" name="email" placeholder="Email"> </div>`,
            `<div class="form-group">
                      <label> User Type </label>
                      <select class="form-control form-control-lg" name="type">
                        <option value="user">Regular User</option>
                        <option value="admin">Administrator</option>
                      </select>
                    </div>`
        ])
    }
}