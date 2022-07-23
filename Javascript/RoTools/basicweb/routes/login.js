import express from "express"
import { AuthPage } from "../components/AuthPage.js";
import bodyParser from "body-parser";
import { Login } from "../modules/auth_client.js";
import { Alert } from "../components/Alert.js";

var parser = bodyParser.urlencoded({extended:true});
var app = express.Router();

app.get("/", (req, res) => {
    var ap = new AuthPage("Auth");
    ap.AddElements([
        "",
        `<form class="step-form" action="" method="POST">
        <img src="../assets/images/ROTOOLS LOGO.png" alt="logo"style="width: 200px;" /><br/>
        <h2 class='display-2'>${global.HOSTSET.replace(".rotools.net", "")}</h2>
        ${req.query.redirect ? `<div class="alert alert-warning" role="alert">
                      <i class="mdi mdi-alert-circle"></i> You need to login to access '${req.query.redirect}' </div>` :""}
                <fieldset>
                <h3>Login to your account.</h3><br/>
                  <div class="form-group">
                    <input class="form-control" name="username" placeholder="Username">
                  </div>
                  <div class="form-group">
                    <input class="form-control" type="password" name="pass" placeholder="Password">
                  </div>
                  <button class="btn btn-inverse-primary next action-button float-left" type="submit" name="next" value="Next">Login</button>
                </fieldset>
              </form>`
    ])
    res.send(ap.Render());
})

app.post("/", parser, async (req,res)=>{
  var username = req.body.username;
  var password = req.body.pass;
  var tk = await Login(username, password);
  if (tk) {
    res.cookie("token", tk);
    if (req.query.redirect){
      res.redirect(req.query.redirect)
    }else {
      res.redirect("/")
    }
    
  }else {
    var ap = new AuthPage("Auth");
    var alr = new Alert("Invalid username or password.", "alert-circle");
    alr.Color = "danger";
    ap.AddElements([
      "",
      `<form class="step-form" action="" method="POST">
        <img src="../assets/images/ROTOOLS LOGO.png" alt="logo"style="width: 200px;" /><br/>
        <h2 class='display-2'>${global.HOSTSET.replace(".rotools.net", "")}</h2>
        ${req.query.redirect ? `<div class="alert alert-warning" role="alert">
                      <i class="mdi mdi-alert-circle"></i> You need to login to access '${req.query.redirect}' </div>` : ""}
                      ${alr.Render()}
                <fieldset>
                <h3>Login to your account.</h3><br/>
                  <div class="form-group">
                    <input class="form-control" name="username" placeholder="Username">
                  </div>
                  <div class="form-group">
                    <input class="form-control" type="password" name="pass" placeholder="Password">
                  </div>
                  <button class="btn btn-inverse-primary next action-button float-left" type="submit" name="next" value="Next">Login</button>
                </fieldset>
              </form>`
    ])
    res.send(ap.Render());
  }
})

export default app;