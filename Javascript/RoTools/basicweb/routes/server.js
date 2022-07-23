import { DummyRequest } from "../modules/DummyRequest.js";
import express from "express"
import { DataTable, TableRow } from "../components/Table.js";
import { SearchCard } from "../components/SearchCard.js";
import server from "../data/models/server.js";
import dateFormat from "dateformat"
import { Card } from "../components/Card.js";
import { Button, ButtonStyles, LinkButton } from "../components/Button.js";
import { Alert } from "../components/Alert.js";
import { Badge } from "../components/Badge.js";
import { LogViewer } from "../components/LogViewer.js";
import { playerLog, serverLog } from "../data/models/logs.js"
import { Colors } from "../components/Theme.js";
import { Terminal } from "../components/Terminal.js";
import { Row } from "../components/Row.js";
import pipes from "../data/models/pipes.js"
import { Client } from "../modules/auth_client.js";
import messagebus from "../data/models/messagebus.js";
import { BanDialog, KickDialog } from "../components/Dialog.js";


var app = express.Router()

app.get('/',
    /**
     * 
     * @param {DummyRequest} req 
     * @param {express.Response} res 
     */
    async (req, res) => {
        req.Client.Dashboard.AddElement(new SearchCard("Lookup Server", 12));

        var serverTable = new DataTable(["Roblox Id", "Players", "Start Time", "Actions"]);

        var serverResults = []
        if (req.query.q) {
            // Roblox server
            if (req.query.q.includes("-")) {
                serverResults = [await server.findByRoblox(req.query.q)]
            } else {
                serverResults = [await server.findServerWithPlayer(req.query.q)]
            }
        } else {
            serverResults = await server.getActiveServersMinor();
        }

        for (var sv of serverResults) {
            if (sv){
                serverTable.AddRow(new TableRow([sv.roblox_id, 
                    `${sv.players.length}/${sv.max_players}`, 
                dateFormat(sv.last_updated, "DDDD mmm dd yyyy MM:hh TT"), 
                new LinkButton("View Server", "/server/" + sv._id)]))
            }
        }

        var card = new Card("Server List", 12);
        card.AddElement(serverTable);

        req.Client.Dashboard.PageTitle = "Server List"
        req.Client.Dashboard.AddElement(card);
        res.send(req.Client.Dashboard.Render());
    })

app.get('/:id', 
/**
   *
   * @param {DummyRequest} req
   * @param {express.Response} res
   */
async function (req, res) {
    var item = await server.findById(req.params.id);
    if (item) {
        if (req.query.executeMacro && req.query.origin) {
            if (req.query.origin == req.Client.UserId) {
                var bus = await item.getBus();
                bus.write({
                    channel:"macro",
                    data:{
                        macro_id: req.query.executeMacro
                    }
                })
            }
            return res.redirect(`/server/${req.params.id}`);
        }
        var card = new Card(item.roblox_id, 12);
        var shutdown_button = new LinkButton();
        shutdown_button.Text = "Shutdown";
        shutdown_button.Color = Colors.Danger;
        shutdown_button.URL = `/server/${req.params.id}?executeMacro=builtin.shutdown&origin=${req.Client.UserId}`
        // var e_button = new LinkButton();
        // e_button.Text = "Force Expire";
        // e_button.Color = Colors.Danger;
        // e_button.URL = `/server/${req.params.id}?executeMacro=builtin.forceexpire&origin=${req.Client.UserId}`
        // shutdown_button.Style = ButtonStyles.Outlined;

        var logs_button = new LinkButton("View Logs", `/logs?q=${req.params.id}&type=server`);

        card.AddElement(shutdown_button);
        card.AddElement(" ");
        card.AddElement(logs_button);
        // card.AddElement(" ");
        // card.AddElement(e_button);

        var players_dt = new DataTable(["Username", "UserId", "Actions"]);
        var events_dt = new DataTable(["Flags", "Type", "Time", "Actions"]);

        // Add ban modal

        for (var player of item.players) {
            players_dt.AddRow(new TableRow([player.username, player.user_id, new Row([new BanDialog(player.user_id, req.originalUrl), "&nbsp;", new KickDialog(player.user_id, req.originalUrl), "&nbsp", new LinkButton("View", `/players?q=${player.user_id}`)])]));
        }
        // Logs disabled for mod panel.
        // var logs = await serverLog.getLogsForServer(item._id, 100);
        // for (var log of logs) {
        //     var lv = new LogViewer(log.type, log.stacktrace, log.object_data, log.message, "LOG", "primary");
        //     lv.ModalButton.Text = "View Log";
        //     events_dt.AddRow(new TableRow([
        //         new Badge("primary", "LOG"),
        //         log.type,
        //         dateFormat(log.date, "DDDD mmm dd yyyy MM:hh TT"),
        //         lv
        //     ]));
        // }
        card.AddElement("<hr/><h5>Macros</h5>")
        for (var macro of item.macros) {
            var _button = new LinkButton();
            _button.Text =  macro.title;
            _button.Color = macro.color;
            _button.Style = ButtonStyles.Inverse;
            _button.URL = `/server/${req.params.id}?executeMacro=${macro.action}&origin=${req.Client.UserId}`
            card.AddElement(_button);
        }


        var player_Card = new Card("Players", 6);
        var terminalCard = new Card("Terminal", 6);
        var data_row = new Row();
        data_row.AddElement(player_Card);
        data_row.AddElement(terminalCard);

        terminalCard.AddElement(new Terminal(req.params.id));
        player_Card.AddElement(players_dt);

        req.Client.Dashboard.AddElement(card);
        req.Client.Dashboard.AddElement(data_row);

    }else {
        var alert = new Alert("The server you requested to view is not found.", "alert-circle");
        alert.Color = "danger";
        req.Client.Dashboard.AddElement(alert);
    }
    req.Client.Dashboard.PageTitle = "View Server";
    res.send(req.Client.Dashboard.Render());
})

var websocket = {};

var qf = 1

websocket["/pipes"] = async (ws, req)=>{
    if (!req.headers.cookie){
        ws.send("Unauthorized.");
        ws.close();
        return;
    }
    var token = ""
    if (req.headers.cookie.includes("; ")) {
        var hrs = req.headers.cookie.split("; ");
        for (var hpar of hrs) {
            var key = hpar.split("=")[0];
            var val = hpar.split("=")[1];
            if (key == "token")
                token = val;
        }
    }else {
        token = req.headers.cookie.split('=')[1];
    }
    var Cli = new Client();
    await Cli.AuthToken(token);
    if (!Cli.Authenticated) {
        ws.send("Unauthorized.");
        ws.close();
        return;
    }
    var svx = await server.findById(req.id);
    if (!svx.isActive())
    {
        ws.send("Could not connect to server. It is offline.");
        ws.close();
        return;
    }
    var pip = await pipes.createPipe(req.id, Cli.Record._id);
    ws.send("Connected to server: " + req.id);
    ws.on("message", async (data)=>{
        await pip.writeToPipe(data);
    });
    pip.watch("stdout",async (data)=>{
        await pip.readFromPipe();
        if (data != "")
        {
            ws.send(data)
        }
    })
    ws.on("close", ()=>{
        pip.delete();
    })
};

export let socket = websocket;


export default app;