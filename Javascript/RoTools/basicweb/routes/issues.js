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
import issues from "../data/models/issues.js";
import { Issue, IssueList } from "../components/Issue.js";
var app = express.Router()

app.get("/",
    /**
       *
       * @param {DummyRequest} req
       * @param {express.Response} res
       */
    async (req, res) => {
        var issue = [];
        var issue_list = await issues.find({});
        for (var x of issue_list) {
            if (x.ignored) continue;
            var objx = new Issue(
                x.type,
                x.amount,
                x.stacktrace,
                x.object_data,
                x.message,
                x.latest_server,
                x.begin_date,
                x.end_date,
                x.hash,
                x._id
            );
            issue.push(objx);
        }
        var issueList = new IssueList();
        issueList.AddElements(issue);

        var cardHolder = new Card("Issues", 12);
        if (issue.length > 0) {
            cardHolder.AddElement(issueList);
  
        }else {
            var noIssuesCard = new Card("", 4);
            noIssuesCard.JustifyCenter = true
            noIssuesCard.AddElement("<h1>🥳</h1><h4 class='display-4'>No Issues Found</h4>");
            cardHolder.JustifyCenter = true;
            cardHolder.AddElement(noIssuesCard);
        }
        

        req.Client.Dashboard.AddElement(cardHolder);
        req.Client.Dashboard.PageTitle = "View Issues";
        res.send(req.Client.Dashboard.Render());
    });
    app.get("/ignore/:id", async (req,res)=>{
        var issue = await issues.findById(req.params.id);
        if (issue){
            issue.ignored = true;
            await issue.save();
        }
        return res.redirect("/issues");
    })

export default app;