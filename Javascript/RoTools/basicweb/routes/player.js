import { DummyRequest } from "../modules/DummyRequest.js";
import express from "express"
import noblox from "noblox.js";
import { Alert } from "../components/Alert.js";
import { DataTable, Table, TableRow } from "../components/Table.js";
import { Row } from "../components/Row.js";
import { Button, LinkButton } from "../components/Button.js";
import server from "../data/models/server.js";
import { Badge } from "../components/Badge.js";
import { Card } from "../components/Card.js";
import { Colors } from "../components/Theme.js";
import dateFormat from "dateformat";
import { Modal, ModalSize } from "../components/Modal.js";
import {PlayerSearchCard} from "../components/SearchCard.js";
import moderation from "../data/models/moderation.js";
import { BanDialog,KickDialog,WarnDialog } from "../components/Dialog.js";
var app = express.Router()

app.get('/', 
/**
   *
   * @param {DummyRequest} req
   * @param {express.Response} res
   */
async function  (req, res) {
    req.Client.Dashboard.AddElement(new PlayerSearchCard("Lookup User", 12));
    if (!req.query.q) {
        req.Client.Dashboard.PageTitle = "View User";
        return res.send(req.Client.Dashboard.Render());
    }

    var player_record;

    if (isNaN(req.query.q)){
        var player_id = await noblox.getIdFromUsername(req.query.q);
        
    }else {
        player_id = req.query.q;
    }
    if (player_id) {
        var record = await moderation.findByRoblox(player_id);
        if (record) {
            player_record = record;
        } else {
            var alert = new Alert("Player did not have any moderation records or playtime on file, so we generated a record for it.", "alert-circle");
            alert.Color = "primary";
            req.Client.Dashboard.AddElement(alert);

            player_record = await moderation.create({
                user_id: player_id
            });
        }
    }

    var playerDataTable = new Table(["",""]);
    //playerDataTable.AddRow(new TableRow(["Database", new LinkButton("View Player Data", "/data/player/" + player_id)])) NOT IN THIS RELEASE YET!
    playerDataTable.AddRow(new TableRow(["Logs", new LinkButton("View Player Logs", `/logs?type=player&q=${player_id}`)]))

    var playerServer = await server.findServerWithPlayerId(player_id);
    if (playerServer) {
        var banButton = new Button();
        banButton.Text = "Kick";
        banButton.Color = Colors.Warning;
        playerDataTable.AddRow(new TableRow([
            "Server", 
            new Row([new LinkButton("View Server", "/server/" + playerServer._id), "&nbsp;", new KickDialog(player_id, req.originalUrl)])
    ]));
    }else {
        playerDataTable.AddRow(new TableRow(["Server", "Not In Server."]));
    }
    var badge = new Badge("primary", "OFFLINE");

    if (playerServer) 
        badge = new Badge("success", "IN GAME");

    if (player_record.banned) {
        if (player_record.ban_til_date > new Date()){
            badge = new Badge("danger", "BANNED");
        }
    }

    playerDataTable.AddRow(new TableRow(["Status", badge]));

    var banButton = new Button();
    banButton.Text = "Ban";
    banButton.Color = Colors.Danger;

    var warnButton = new Button();
    warnButton.Text = "Warn";
    warnButton.Color = Colors.Warning;

    playerDataTable.AddRow(new TableRow(["Actions", new Row([new BanDialog(player_id, req.originalUrl), "&nbsp;", new WarnDialog(player_id, req.originalUrl)])]))

    var data_card = new Card("Player Data", 6);
    data_card.AddElement(`<h4>${req.query.q} <small>${player_id}</small></h4>`)
    data_card.AddElement(playerDataTable);

    var history = new DataTable(["Action", "Moderator", "Date", "Reason"])

    var plyr_history = await moderation.getModerationHistory(player_id);

    for (var x of plyr_history) {
        var reasonModal = new Modal("Reason");
        reasonModal.AddElement(x.reason);
        reasonModal.ModalButton.Text = "View Reason"
        history.AddRow(new TableRow([
            modTypes[x.type],
            x.moderator.username,
            dateFormat(x.date, "DDDD mmm dd yyyy hh:MM TT"),
            reasonModal
        ]));
    }
    var history_card = new Card("Player History", 6);
    history_card.AddElement(history);

    var rowHolder = new Row();
    rowHolder.AddElement(data_card);
    rowHolder.AddElement(history_card);

    req.Client.Dashboard.AddElement(rowHolder);
    req.Client.Dashboard.PageTitle = "View User";
    return res.send(req.Client.Dashboard.Render());
})


const modTypes = {
    0: new Badge("warning", "KICK"),
    1:new Badge("danger", "BAN"),
    2: new Badge("warning", "WARNING")
}

export default app;