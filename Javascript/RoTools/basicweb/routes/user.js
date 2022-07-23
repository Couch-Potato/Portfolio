import express from "express"
import { Card, CardFooter } from "../components/Card.js";
import { Modal } from "../components/Modal.js";
import { DataTable, TableRow } from "../components/Table.js";
import { DummyRequest } from "../modules/DummyRequest.js";
import userData from "../data/models/user.js";
import { LinkButton } from "../components/Button.js";
import { Colors } from "../components/Theme.js";
import { AvatarRenderer } from "../components/AvatarRenderer.js";
import { Badge } from "../components/Badge.js";
import { Row } from "../components/Row.js";
import { CreateUserDialog} from "../components/Dialog.js";
import bodyParser from "body-parser";
import noblox from "noblox.js";
import axios from "axios";
import user from "../data/models/user.js";
import { RandomString } from "../util.js";
import { Alert } from "../components/Alert.js";
import { MailTo } from "../modules/mail.js";
import { CreateOnBoard } from "../modules/auth_client.js";
var app = express.Router();
var parser = bodyParser.urlencoded();

app.use(async (req, res, next) => {
    if (req.Client.Record.role == "user") return res.redirect("/");
    next();
})


app.get("/", 
    /**
       *
       * @param {DummyRequest} req
       * @param {express.Response} res
       */
async(req,res)=>{
    
    if (req.query.userCreate) {
        var alrt = new Alert("User account has been created.", "alert-circle");
        alrt.Color = "success"
        req.Client.Dashboard.AddElement(alrt);
    }
    if (req.query.userFail) {
        if (req.query.userFail == "invald") {
            var alrt = new Alert("Account type is invalid.", "alert-circle");
            alrt.Color = "danger"
            req.Client.Dashboard.AddElement(alrt);
        }
        if (req.query.userFail == "email") {
            var alrt = new Alert("Account email is invalid.", "alert-circle");
            alrt.Color = "danger"
            req.Client.Dashboard.AddElement(alrt);
        }
        if (req.query.userFail == "duplicate") {
            var alrt = new Alert("That account already exists.", "alert-circle");
            alrt.Color = "danger"
            req.Client.Dashboard.AddElement(alrt);
        }
    }
    var card = new Card("Users", 12);

    var userTable = new DataTable(["Avatar", "Username","Role","Status","Actions"])

    var users = await userData.find({});
    for (var user of users) {
        if (user.deleted) continue;
        var disableButton = new LinkButton("Disable", "/users/disable/" + user._id);
        disableButton.Color = Colors.Warning;

        var enable = new LinkButton("Enable", "/users/enable/" + user._id);
        enable.Color = Colors.Success;

        var deleteButton = new LinkButton("Delete", "/users/delete/" + user._id);
        deleteButton.Color = Colors.Danger;
        userTable.AddRow(new TableRow([
            new AvatarRenderer(user.pfp),
            user.username,
            user.role,
            user.disabled?new Badge("danger", "DISABELD") : new Badge("primary", "ACTIVE"),
            user.role == "root" || user.role == "owner" ? new Row([]) : new Row(user.disabled ? [deleteButton, "&nbsp;", enable] : [deleteButton, "&nbsp;", disableButton])
        ]))
    }

    card.AddElement(userTable);
    var createUserModal = new CreateUserDialog();
    card.Footer = new CardFooter([new Row([createUserModal])]);
    req.Client.Dashboard.AddElement(card);
    req.Client.Dashboard.PageTitle = "Users"
    res.send(req.Client.Dashboard.Render());
});

app.get("/disable/:id", async(req,res)=>{
    var r = await user.findById(req.params.id);
    r.disabled = true;
    await r.save();
    return res.redirect("/users");
})

app.get("/enable/:id", async (req, res) => {
    var r = await user.findById(req.params.id);
    r.disabled = false;
    await r.save();
    return res.redirect("/users");
})
app.get("/delete/:id", async (req, res) => {
    var r = await user.findById(req.params.id);
    r.deleted = true;
    await r.save();
    return res.redirect("/users");
})
app.post("/create",parser,  async (req,res)=>{
    const newUsername = req.body.username;
    const newEmail = req.body.email;
    const type = req.body.type;

    if (type != "user" && type!="admin") {
        return res.redirect("/users?userFail=invalid")
    }
    if (!newEmail.includes("@")) {
        return res.redirect("/users?userFail=email")
    }

    var userId = await noblox.getIdFromUsername(newUsername);
    var tb = await axios.get(`https://www.roblox.com/headshot-thumbnail/json?userId=${userId}&width=180&height=180`)
    const pfp = tb.data.Url;

    var username = await user.findOne({username:newUsername});
    if (username) {
        if (username.deleted) {
            username.creation_date = new Date();
            username.role = type
            username.email = newEmail
            username.pfp = pfp
            username.deleted = false;
            delete username.password
            await username.save();
            var token = CreateOnBoard(username._id)
            await MailTo(newUsername, newEmail, token, req.Client.Username);
            return res.redirect("/users?userCreate=true");
        }else {
            return res.redirect("/users?userFail=duplicate")
        }
    }

    var id = await user.create({
        username:newUsername,
        creation_date: new Date(),
        role:type,
        email:newEmail,
        pfp:pfp
    })
    var token = CreateOnBoard(id._id)
    await MailTo(newUsername,newEmail, token, req.Client.Username);

    res.redirect("/users?userCreate=true")
})

export default app;