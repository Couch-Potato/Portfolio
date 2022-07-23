import express from "express"
import { BanDialog, KickDialog, WarnDialog, Dialog } from "../components/Dialog.js";
import bodyParser from "body-parser";
var parser = bodyParser.urlencoded();
var app = express.Router();
export let Dialogs = [
    new BanDialog(1),
    new KickDialog(1),
    new WarnDialog(1)
]

app.post(`/${Dialogs[0].Type}/:id`, parser, async (req, res) => {
    var i=0
    var result = await Dialogs[i].Handle(req.params.id, req);
    if (req.query.refer) {
        res.redirect(req.query.refer + `${req.query.refer.includes("?") ? `&${Dialogs[i].Type}=${result}` : `?${Dialogs[i].Type}=${result}`}`)
    } else {
        res.redirect(`/${req.url.includes("?") ? `&${Dialogs[i].Type}=${result}` : `?${Dialogs[i].Type}=${result}`}`)
    }
})
app.post(`/${Dialogs[1].Type}/:id`, parser, async (req, res) => {
    var i = 1
    var result = await Dialogs[i].Handle(req.params.id, req);
    if (req.query.refer) {
        res.redirect(req.query.refer + `${req.query.refer.includes("?") ? `&${Dialogs[i].Type}=${result}` : `?${Dialogs[i].Type}=${result}`}`)
    } else {
        res.redirect(`/${req.url.includes("?") ? `&${Dialogs[i].Type}=${result}` : `?${Dialogs[i].Type}=${result}`}`)
    }
})
app.post(`/${Dialogs[2].Type}/:id`, parser, async (req, res) => {
    var i = 2
    var result = await Dialogs[i].Handle(req.params.id, req);
    if (req.query.refer) {
        res.redirect(req.query.refer + `${req.query.refer.includes("?") ? `&${Dialogs[i].Type}=${result}` : `?${Dialogs[i].Type}=${result}`}`)
    } else {
        res.redirect(`/${req.url.includes("?") ? `&${Dialogs[i].Type}=${result}` : `?${Dialogs[i].Type}=${result}`}`)
    }
})
export default app;
