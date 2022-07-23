import express from "express"
import { Card, CardFooter } from "../components/Card.js";
import { Tab, TabPage } from "../components/Tab.js";
import { DataTable, TableRow } from "../components/Table.js";
import api from "../data/models/api.js";
import { DummyRequest } from "../modules/DummyRequest.js";
import dateFormat from "dateformat";
import { ButtonStyles, LinkButton } from "../components/Button.js";
import { Modal, ModalSize } from "../components/Modal.js";
import { Row } from "../components/Row.js";
import { Colors } from "../components/Theme.js";
import {Alert} from "../components/Alert.js";
import { WSClient, WSConnection } from "../modules/wshook.js";
var app = express.Router();

app.use(async (req,res,next)=>{
    if (req.Client.Record.role == "user") return res.redirect("/");
    next();
})

/**
 * @type {Array<{message:string, color:string}>}
 */
const messageTable = [
    {
        message:"Key has been deleted successfully.", color:"success",
    },{
        message:"The specified API key cannot be found", color:"danger",
    },{
        message: "Key has been created successfully.", color: "success",
    },
    {
        message: "Your game key has been reset.", color: "warning",
    }

]   

app.get("/",
    /**
   *
   * @param {DummyRequest} req
   * @param {express.Response} res
   */
    async (req, res) => {
        var settingsCard = new Card("Settings", 12);
        var tab = new Tab();

        /**
         * API KEYS TAB
         */
        var dataTable = new DataTable(["Client Id", "Creator", "Creation Date", "Actions"]);
        var keys = await api.getAll();

        for (var key of keys) {
            var dmdl = new Modal("Delete Key");
            dmdl.ModalButton.Color = Colors.Danger;
            dmdl.ModalButton.Style = ButtonStyles.Inverse;
            dmdl.IsForm = true;
            dmdl.FormUrl = "/settings/dk/" + key._id
            dmdl.ModalButton.FixedWidth = false
            dmdl.AddElements([
                "<h3>Warning</h3>",
                "Deleting this key will result in any apps associated with it to stop working. Only do this if you sure about what you are doing."
            ])
            dataTable.AddRow(new TableRow([
                key.client_id,
                key.creator.username,
                dateFormat(key.date, "mm/dd/yy"),
                new Row([new LinkButton("Download Key", "/settings/key/" + key._id, true), "&nbsp;", dmdl])
            ]))
        }

        var createModal = new Modal("Create Key");
        createModal.IsForm = true;
        createModal.FormUrl = "/settings/create_key"
        createModal.Body = "<h2>You are creating an API Key.</h2> This will give whoever has access to this key access to your games. They can run terminal commands, view server data and logs, and game sockets."
        createModal.ModalButton.Style = ButtonStyles.Inverse;
        createModal.ModalButton.FixedWidth = false;
        var creatModalRow = new Row();
        creatModalRow.AddElement(createModal);
        dataTable.Footer = creatModalRow;
        tab.AddTab(new TabPage("APIKeys", [dataTable]));


        /**
         * GAME KEYS
         */

        var keycard = new Card("Game Key", 11);
        var krm = new Modal("Reset Key");
        krm.AddElements([
            "<h3>Warning</h3>",
            "Resetting your key will terminate all running RoTools instances, and you will have to shut down all servers and paste in the new key for them to reconnect. You should only reset your key if it is compromised."
        ])
        krm.ModalButton.Color = Colors.Danger;
        krm.ModalButton.Style = ButtonStyles.Inverse;
        krm.IsForm = true;
        krm.ModalButton.FixedWidth = false
        krm.FormUrl = "/settings/reset_gk"
        keycard.AddElements([
            "Your game key contains the secret that will allow your ROBLOX game to connect to RoTools. For this account, you may only have one game key attached. If your game key is compromised, it is advised that you reset it.",
        ])
        keycard.Footer = new CardFooter([
            new Row([
                new LinkButton("Download Key", "/settings/game_key/", true),
                "&nbsp;",
                krm
            ])
        ])

        tab.AddTab(new TabPage("GameKeys", [keycard]))
        settingsCard.AddElement(tab);
        if (req.query.m) {
            var idx = Number(req.query.m);
            if (idx >= 0 && idx < messageTable.length)
            {
                var alrt = new Alert(messageTable[idx].message, "alert-circle")
                alrt.Color = messageTable[idx].color
                req.Client.Dashboard.AddElement(alrt)
            }
        }
        req.Client.Dashboard.PageTitle = "Settings";
        req.Client.Dashboard.AddElement(settingsCard);
        res.send(req.Client.Dashboard.Render());
    });
app.get("/api",
    /**
   *
   * @param {DummyRequest} req
   * @param {express.Response} res
   */
    async (req, res) => {
        var settingsCard = new Card("Settings", 12);
        var tab = new Tab();

        /**
         * API KEYS TAB
         */
        var dataTable = new DataTable(["Client Id", "Creator", "Creation Date", "Actions"]);
        var keys = await api.getAll();

        for (var key of keys) {
            var dmdl = new Modal("Delete Key");
            dmdl.ModalButton.Color = Colors.Danger;
            dmdl.ModalButton.Style = ButtonStyles.Inverse;
            dmdl.IsForm = true;
            dmdl.FormUrl = "/settings/dk/" + key._id
            dmdl.ModalButton.FixedWidth = false
            dmdl.AddElements([
                "<h3>Warning</h3>",
                "Deleting this key will result in any apps associated with it to stop working. Only do this if you sure about what you are doing."
            ])
            dataTable.AddRow(new TableRow([
                key.client_id,
                key.creator.username,
                dateFormat(key.date, "mm/dd/yy"),
                new Row([new LinkButton("Download Key", "/settings/key/" + key._id, true), "&nbsp;", dmdl])
            ]))
        }

        var createModal = new Modal("Create Key");
        createModal.IsForm = true;
        createModal.FormUrl = "/settings/create_key"
        createModal.Body = "<h2>You are creating an API Key.</h2> This will give whoever has access to this key access to your games. They can run terminal commands, view server data and logs, and game sockets."
        createModal.ModalButton.Style = ButtonStyles.Inverse;
        createModal.ModalButton.FixedWidth = false;
        var creatModalRow = new Row();
        creatModalRow.AddElement(createModal);
        dataTable.Footer = creatModalRow;
        settingsCard.AddElement(dataTable);
        if (req.query.m) {
            var idx = Number(req.query.m);
            if (idx >= 0 && idx < messageTable.length) {
                var alrt = new Alert(messageTable[idx].message, "alert-circle")
                alrt.Color = messageTable[idx].color
                req.Client.Dashboard.AddElement(alrt)
            }
        }
        req.Client.Dashboard.PageTitle = "Settings";
        req.Client.Dashboard.AddElement(settingsCard);
        res.send(req.Client.Dashboard.Render());
    });
app.get("/game",
    /**
   *
   * @param {DummyRequest} req
   * @param {express.Response} res
   */
    async (req, res) => {
        var settingsCard = new Card("Settings", 12);
        var tab = new Tab();

        /**
         * GAME KEYS
         */

        var keycard = new Card("Game Key", 11);
        var krm = new Modal("Reset Key");
        krm.AddElements([
            "<h3>Warning</h3>",
            "Resetting your key will terminate all running RoTools instances, and you will have to shut down all servers and paste in the new key for them to reconnect. You should only reset your key if it is compromised."
        ])
        krm.ModalButton.Color = Colors.Danger;
        krm.ModalButton.Style = ButtonStyles.Inverse;
        krm.IsForm = true;
        krm.ModalButton.FixedWidth = false
        krm.FormUrl = "/settings/reset_gk"
        keycard.AddElements([
            "Your game key contains the secret that will allow your ROBLOX game to connect to RoTools. For this account, you may only have one game key attached. If your game key is compromised, it is advised that you reset it.",
        ])
        keycard.Footer = new CardFooter([
            new Row([
                new LinkButton("Download Key", "/settings/game_key/", true),
                "&nbsp;",
                krm
            ])
        ])
        settingsCard.AddElement(keycard);
        if (req.query.m) {
            var idx = Number(req.query.m);
            if (idx >= 0 && idx < messageTable.length) {
                var alrt = new Alert(messageTable[idx].message, "alert-circle")
                alrt.Color = messageTable[idx].color
                req.Client.Dashboard.AddElement(alrt)
            }
        }
        req.Client.Dashboard.PageTitle = "Settings";
        req.Client.Dashboard.AddElement(settingsCard);
        res.send(req.Client.Dashboard.Render());
    });
app.post("/dk/:id",
    /**
       *
       * @param {DummyRequest} req
       * @param {express.Response} res
       */
    async (req, res) => {
        if (req.params.id)
        {
            await api.deleteKey(req.params.id);
            res.redirect("/settings?m=0")
        }
    })
app.get("/key/:id",
    /**
       *
       * @param {DummyRequest} req
       * @param {express.Response} res
       */
    async (req, res) => {
        res.setHeader('Content-disposition', 'attachment; filename=' + "api.json");
        res.setHeader('Content-type', "application/octet-stream");
        var item = await api.findById(req.params.id)
        if (!item) {
            return res.redirect("/settings?m=1")
        }
        res.send(JSON.stringify({
            client: item.client_id,
            token: item.token,
            host: global.HOSTSET
        }));
    })
app.post("/create_key",
    /**
       *
       * @param {DummyRequest} req
       * @param {express.Response} res
       */
    async (req, res) => {
        await api.generateKey(req.Client.UserId);
        return res.redirect("/settings?m=2")
    })
app.post("/reset_gk",
    /**
       *
       * @param {DummyRequest} req
       * @param {express.Response} res
       */
    async (req, res) => {
        if (global.CONFIGSOCKET) {
            /**
             * @type {WSConnection}
             */
            var connection = global.CONFIGSOCKET;
            var newKey = await connection.Query("reset_key");
            global.GAMEKEY = newKey;
        }
        return res.redirect("/settings?m=3")
    })
app.get("/game_key",
    /**
       *
       * @param {DummyRequest} req
       * @param {express.Response} res
       */
    async (req, res) => {
        res.setHeader('Content-disposition', 'attachment; filename=' + "rotools.lua");
        res.setHeader('Content-type', "application/octet-stream");
        res.send(`local RoToolsAuth = {}
RoToolsAuth.Token = "${global.GAMEKEY}"
RoToolsAuth.Host = "${global.HOSTSET}"
return RoToolsAuth`);
    })

export default app;