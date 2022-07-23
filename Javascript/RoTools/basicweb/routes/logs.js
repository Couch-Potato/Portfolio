import express from "express"
import { Row } from "../components/Row.js";
import { playerLog, serverLog } from "../data/models/logs.js";
import server from "../data/models/server.js";
import { DummyRequest } from "../modules/DummyRequest.js";
import { DataTable, TableRow } from "../components/Table.js";
import { LogViewer } from "../components/LogViewer.js";
import { Card } from "../components/Card.js";
import { Badge } from "../components/Badge.js"
import dateFormat from "dateformat";
import noblox from "noblox.js";
import { SearchCard } from "../components/SearchCard.js";
import moderation from "../data/models/moderation.js";
import { LinkButton } from "../components/Button.js";
var app = express.Router()

app.get("/",
    /**
       *
       * @param {DummyRequest} req
       * @param {express.Response} res
       */
    async (req, res) => {
        req.Client.Dashboard.AddElement(new SearchCard("Lookup Server Or Client", 12));
        if (!req.query.q) {
            req.Client.Dashboard.PageTitle = "Lookup Logs";
            return res.send(req.Client.Dashboard.Render());
        }
        var isServer = req.query.q.includes("-") || req.query.type == "server";
        var main_row = new Row();

        if (isServer) {
            var serverObj;
            if (req.query.type) {
                if (req.query.type == "server") {
                    serverObj = await server.findById(req.query.q);
                } else {
                    serverObj = await server.findByRoblox(req.query.q);
                }
            } else {
                serverObj = await server.findByRoblox(req.query.q);
            }

            var serverLogs = await serverLog.getLogsForServer(serverObj._id, 100);
            var playerLogs = await playerLog.getLogsForServer(serverObj._id, 100);

            var sLogData = new DataTable(["Type", "Date", "Source", "View"]);
            var pLogData = new DataTable(["Player", "Type", "Date", "Source", "View", "Player Logs"]);

            for (var log of serverLogs) {
                var stackView = new LogViewer(
                    log.type,
                    log.stacktrace,
                    log.object_data,
                    log.message,
                    "LOG",
                    "primary"
                );
                stackView.ModalButton.Text = "View Log";
                sLogData.AddRow(new TableRow([
                    log.type,
                    dateFormat(log.date, "mm/dd/yyyy MM:hh TT"),
                    stackView.StackView.Children[0].PackageName,
                    stackView
                ]));
            }
            for (var log of playerLogs) {
                var stackView = new LogViewer(
                    log.type,
                    log.stacktrace,
                    log.object_data,
                    log.message,
                    "LOG",
                    "primary"
                );
                stackView.ModalButton.Text = "View Log";
                pLogData.AddRow(new TableRow([
                    log.player_username,
                    log.type,
                    dateFormat(log.date, "mm/dd/yyyy MM:hh TT"),
                    stackView.StackView.Children[0].PackageName,
                    stackView.Render(), new LinkButton("View Player Logs", "/logs?q=" + log.player_id + "&type=player").Render()
                ]));
            }

            var pLogDataCard = new Card("Player Logs", 7);
            var sLogDataCard = new Card("Server Logs", 5);

            sLogDataCard.AddElement(sLogData);
            pLogDataCard.AddElement(pLogData);

            main_row.AddElement(sLogDataCard);
            main_row.AddElement(pLogDataCard);
        } else {
            var roblox_id;
            if (req.query.type) {
                roblox_id = req.query.q;
            } else {
                roblox_id = await noblox.getIdFromUsername(req.query.q);
            }

            var player_data = await moderation.findByRoblox(roblox_id);
            var sLogData = new DataTable(["Server", "Type", "Date", "View"]);
            var recentData = new DataTable(["Flags", "Server Id", "View"]);

            if (player_data) {
                var logs = await player_data.getLogs();
                var recentServers = player_data.recent_servers;
                var serverWithPlayer = await server.findServerWithPlayerId(roblox_id);
                for (var i of recentServers) {
                    var x = await server.findById(i);

                    var active = x.isActive();
                    var isPlayerServer = false;
                    if (serverWithPlayer) {
                        isPlayerServer = serverWithPlayer._id.equals(x._id);
                    }
                   


                    var badgeData = "";
                    var buttonData = new LinkButton("View Logs", `/logs?q=${i}&type=server`).Render();
                    if (active)
                        badgeData += new Badge("primary", "ONLINE").Render();
                    else {
                        badgeData += new Badge("danger", "OFFLINE").Render();
                    }
                    if (isPlayerServer)
                        badgeData += " " + new Badge("success", "IN GAME").Render();

                    if (active)
                        buttonData += " " + new LinkButton("View Server", "/server/" + i).Render();

                    recentData.AddRow(new TableRow([
                        badgeData,
                        x.roblox_id,
                        buttonData
                    ]));
                }

                for (var log of logs) {
                    var stackView = new LogViewer(
                        log.type,
                        log.stacktrace,
                        log.object_data,
                        log.message,
                        "LOG",
                        "primary"
                    );
                    stackView.ModalButton.Text = "View Log";
                    sLogData.AddRow(new TableRow([
                        log.player_username,
                        log.type,
                        dateFormat(x.date, "mm/dd/yyyy MM:hh TT"),
                        stackView
                    ]));
                }

                var pLogDataCard = new Card("Player Logs", 7);
                var sLogDataCard = new Card("Recent Servers", 5);

                sLogDataCard.AddElement(recentData);
                pLogDataCard.AddElement(sLogData);
                main_row.AddElement(pLogDataCard);
                main_row.AddElement(sLogDataCard);




            }

        }
        req.Client.Dashboard.PageTitle = "Logs";
        req.Client.Dashboard.AddElement(main_row);
        res.send(req.Client.Dashboard.Render());
    });

export default app;