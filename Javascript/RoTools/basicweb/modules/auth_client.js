import jwt from "jsonwebtoken"
import fs from "fs"
import user from "../data/models/user.js"
import { DashboardPage } from "../components/DashboardPage.js"
import { MenuHolder, SingletonMenu, UserMenu } from "../components/MenuItem.js"
import { Roles } from "./roleset.js"
import { WSConnection } from "./wshook.js"
import { Dialog } from "../components/Dialog.js"
import {Dialogs} from "../routes/action.js";
import {Alert} from "../components/Alert.js";
import passwordValidator from "password-validator";
import bcrypt from "bcrypt";
var privateKey = fs.readFileSync("keys/private.key")
var publicKey = fs.readFileSync("keys/public.key")

export class Client {
    Authenticated = false;
    Record;
    UserId;
    Username;
    /**
     * @type {DashboardPage}
     */
    Dashboard = new DashboardPage("Placeholder");
    #request;
    #response;
    constructor(req, res) {
        this.#request = req
        this.#response = res
    }

    /**
     * Gets if a client can read a certain permission.
     * @param {"Servers" | "Logs" | "Issues"} permission The permission type to read
     * @returns {Boolean} whether they can read or not.
     */
    CanRead(permission) {
        return this.Record.getRoleAtIndex(Roles[permission]).includes("r");
    }
    /**
     * Gets if a client can write a certain permission.
     * @param {"Servers" | "Logs" | "Issues"} permission The permission type to read
     * @returns {Boolean} whether they can write or not.
     */
    CanWrite(permission) {
        return this.Record.getRoleAtIndex(Roles[permission]).includes("rw");
    }

    async AuthToken(token) {
        if (token) {
            try {
                var authobj = jwt.verify(token, privateKey)
                if (authobj) {
                    this.Record = await user.findById(authobj.user_id);
                    if (this.Record) {
                        this.Authenticated = true;
                        this.UserId = this.Record._id;
                        this.Username = this.Record.username;

                    }

                }
            } catch { }

            if (!this.Record) return;

            this.Dashboard = new DashboardPage("Placeholder");
            this.Dashboard.Username = this.Username;
            this.Dashboard.ProfilePicture = this.Record.pfp;

            if (this.#request) {
                for (var qp in this.#request.query) {
                    for (var dialog of Dialogs) {
                        if (qp == dialog.Type) {
                            var r = dialog.GetResult(this.#request.query[qp]);
                            if (r) {
                                var aler = new Alert(r.Text, r.Icon);
                                aler.Color = r.Color;
                                this.Dashboard.AddElement(aler);
                            }
                        }
                    }
                }
            }

            


            // Moderation
            var mod = new MenuHolder("Administration", "gavel");
            mod.Populate([
                { Name: "View Server", URL: "/server" },
                { Name: "Lookup User", URL: "/players" }
            ]);

            var logs = new SingletonMenu("Logs", "file-outline", "/logs");
            var issues = new SingletonMenu("Issues", "alert-circle-outline", "/issues");

            var dash = new SingletonMenu("Dashboard", "speedometer", "/");

            var manage = new MenuHolder("Management", "settings");
            manage.Populate([
                { Name: "Users", URL: "/users" },
                { Name: "Settings", URL: "/settings" }
            ]);

            this.Dashboard.MenuItems = [dash, mod, logs, issues]
            if (this.Record.role !="user") {
                var keys = new MenuHolder("Keys", "settings");
                keys.Populate([
                    {
                        Name: "API Keys", URL:"/settings/api"
                    }, {
                        Name:"Game Keys", URL : "/settings/game"
                    }
                ])
                var users = new SingletonMenu("Users", "settings", "/users");
                this.Dashboard.MenuItems.push(keys);
                this.Dashboard.MenuItems.push(users);
            }

           

            this.Dashboard.UserMenuItems = [
                new UserMenu("Sign out", "")
            ]

            // Do authentication stuff here.
        }
    }
}

export async function CreateRoot() {
    var root = await user.findOne({ username: "Root" });
    if (!root) {
        root = await user.create({
            username: "Root",
            password: "34743777217A25432A46294A404E635266556A5386E3272357538782F413F4428472B4B6150645367566B5970337336763979244226452948404D635165546857", // It is impossibe for a hash to ever generate this
            creation_date: new Date(),
            pfp: "https://tr.rbxcdn.com/c3ee609e91804ee2f15c6375355a381a/150/150/AvatarHeadshot/Png",
            role: "root"
        });
    }
    return jwt.sign({ user_id: root._id, date: new Date() }, privateKey)
}

export function CreateOnBoard(id){
    return jwt.sign({ ob_id: id, date: new Date() }, privateKey)
}
export async function VerifyOnBoard(token){
    var id = jwt.verify(token, privateKey).ob_id;
    var x=  await user.findById(id);
    if (!x.password) return x;
    else return false; 
}
export async function GetUserRoleFromToken(token) {
    var authobj = jwt.verify(token, privateKey)
    if (authobj) {
        var x = await user.findById(authobj.user_id);
        return x.role;
    }
}
export function ValidatePasswordStrength(pw){
    var schema = new passwordValidator();
    schema
        .is().min(8)                                    // Minimum length 8
        .is().max(100)                                  // Maximum length 100
        .has().uppercase()                              // Must have uppercase letters
        .has().lowercase()                              // Must have lowercase letters
        .has().digits(1)                                // Must have at least 2 digits
        .has().not().spaces()                           // Should not have spaces
        .is().not().oneOf(['Passw0rd', 'Password123']); // Blacklist these values
    var validation = schema.validate(pw);
    if (validation){
        return {
            valid:true
        }
    }
    else {
        var reasons = "";
        for (var reason of schema.validate(pw,{list:true})){
            switch (reason) {
                case "min":
                    reasons += "The password is too small. <br/>"; break;
                case "max":
                    reasons += "The password is too large. <br/>"; break;
                case "digits":
                    reasons += "The password requires at least one digit. <br/>"; break;
                case "uppercase":
                    reasons += "The password requires at least one uppercase character. <br/>"; break;
                case "lowercase":
                    reasons += "The password requires at least one lowercase character. <br/>"; break;
            }
        }
        return {
            valid:false,
            reason:reasons
        }
    }
}

export async function Login(username, pass){
    var userx = await user.findOne({username:username});
    if (!userx) return false;
    var isValid =  await bcrypt.compare(pass, userx.password);
    if (!isValid) return false;
    if (userx.disabled || userx.deleted) return false;
    return jwt.sign({ user_id: userx._id, date: new Date() }, privateKey)
}

export async function HashPassword(pass) {
    return await bcrypt.hash(pass, 10);
}

async function HandleStartup() {
    var root = await CreateRoot();
    console.log("Root Account:  " + `http://localhost:8080/onboarding?token=${root}`)
    if (global.CONFIGSOCKET) {
        /**
         * @type {WSConnection}
         */
        const cx = global.CONFIGSOCKET;
        cx.Send({
            header: "root_cfg",
            token: root
        })
    }
}
HandleStartup();


