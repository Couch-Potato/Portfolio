import express from "express"
var app = express.Router()
import {DashboardPage} from "../components/DashboardPage.js"
import { TableRow, Table, DataTable } from "../components/Table.js";
import {Card} from "../components/Card.js"
import {Row} from "../components/Row.js"
import { Button } from "../components/Button.js";
import {Timeline, TimelineEntry } from "../components/Timeline.js";
import { Modal } from "../components/Modal.js";
import { IconBadge } from "../components/Badge.js";
import { Colors } from "../components/Theme.js";
import { Flamegraph } from "../components/Flamegraph.js";
import serverx from "../data/models/server.js";
import moderation from "../data/models/moderation.js"
import {playerLog, serverLog} from "../data/models/logs.js"
import { MarkdownView } from "../components/MarkdownView.js";
import { StackTrace } from "../components/StackTrace.js";
import { ObjectViewer } from "../components/ObjectViewer.js";
import { LogViewer } from "../components/LogViewer.js";
import issues from "../data/models/issues.js";
import { Issue } from "../components/Issue.js";
import { DummyRequest } from "../modules/DummyRequest.js";
import { LoginPage } from "../components/Login.js";
import server from "../data/models/server.js";
import { Statistic } from "../components/Statistic.js";

var cached = {
    logs:0,
    server:0,
    issue:0
}
var cacheExpiryTime = new Date();
async function getCachedStats(){
    if (cacheExpiryTime < new Date()){
        var logsAmt = await serverLog.find({}).select("_id");
        var serverAmt = await server.getActiveServersAmt();
        var issueamt = await issues.getActiveIssueAmt();
        cacheExpiryTime = new Date() + (5*60*1000);
        cached = {
            logs:logsAmt.length,
            server: serverAmt,
            issue: issueamt,
        }
        return cached;
    }
    else return cached;
}

// respond with "hello world" when a GET request is made to the homepage
app.get('/', 
/**
 * @param {DummyRequest} req 
 * @param {*} res 
 */
async function (req, res) {
    req.Client.Dashboard.AddElement(`
    <h1 class="display-1">Welcome, ${req.Client.Username}.</h1>
    `)
    var stats = await getCachedStats();

    req.Client.Dashboard.AddElement(new Row([
        new Statistic("success", "Servers", "Servers Currently Online", stats.server),
        new Statistic("primary", "Logs", "Logs Currently Reported", stats.logs),
        new Statistic("danger", "Issues", "Issues Currently Reported", stats.issue)
    ]));

    req.Client.Dashboard.AddElement(`
    <h1 class="display-3">You aren't reporting any statistics right now.</h1>
    `)

    req.Client.Dashboard.PageTitle="";
    res.send(req.Client.Dashboard.Render());
})


export default app
