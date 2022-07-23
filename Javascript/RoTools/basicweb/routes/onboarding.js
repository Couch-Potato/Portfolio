import express from "express"
import { AuthPage } from "../components/AuthPage.js";
import { GetUserRoleFromToken, HashPassword, ValidatePasswordStrength, VerifyOnBoard, CreateOnBoard } from "../modules/auth_client.js";
import bodyParser from "body-parser";
import { Alert } from "../components/Alert.js";
import user from "../data/models/user.js";
import noblox from "noblox.js";
import axios from "axios";
var app = express.Router();

var parser = bodyParser.urlencoded({extended:true})

app.get("/user", async (req,res)=>{

  if (!req.query.token) return res.redirect("/auth");
  var uData = await VerifyOnBoard(req.query.token);
  if (!uData) return res.redirect("/auth");

    

    var ap = new AuthPage("Auth");
    ap.AddElements([
        "",
        `<form class="step-form" action="" method="POST">
        <img src="../assets/images/ROTOOLS LOGO.png" alt="logo"style="width: 200px;" />
        <h2 class='display-2'>Welcome, ${uData.username}</h2>
        <h4>Lets setup your account.</h4>
                <fieldset>
                <h5 class="float-left">Lets get you a password.</h5>
                  <div class="form-group">
                    <input class="form-control" type="password" name="pass" placeholder="Password">
                  </div>
                  <div class="form-group">
                    <input class="form-control" type="password" name="cpass" placeholder="Confirm Password">
                  </div>
                  <h5 class="float-left">We need you to confirm some things.</h5>
                  <br/><br/>
                  <div class="icheck-square float-left">
                          <input tabindex="5" type="checkbox" name="privacy" id="square-checkbox-1">
                          <label for="square-checkbox-1">I agree to the <a href="/doc/privacy">Privacy Policy</a></label>
                        </div>
                         <br/><br/>
                <div class="icheck-square float-left">
                          <input tabindex="5" type="checkbox" name="tos" id="square-checkbox-1">
                          <label for="square-checkbox-1">I agree to the <a href="/doc/tos">Terms of Service</a></label>
                        </div> <br/><br/>
                  <button class="btn btn-inverse-primary next action-button float-left" type="submit" name="next" value="Next">Continue</button>
                </fieldset>
              </form>`
    ])
    res.send(ap.Render());
})

app.post("/user",parser, async (req,res)=>{
  if (!req.query.token) return res.redirect("/auth");
  var uData = await VerifyOnBoard(req.query.token);
  if (!uData) return res.redirect("/auth");
  var tos = req.body.tos;
  var privacy = req.body.privacy;
  var error;
  if (!tos) {
    error = "You need to accept the terms of service."
  }
  if (!privacy) {
    error = "You need to accept the privacy policy."
  }
  if (req.body.pass != req.body.cpass) {
    error = "Passwords do not match"
  }
  var pdValidate = ValidatePasswordStrength(req.body.pass);
  if (!pdValidate.valid) {
    error = "Password is not strong enough: <br/>" + pdValidate.reason;
  }

  if (!error) {
    var pwhash = await HashPassword(req.body.pass);
    uData.password = pwhash;
    await uData.save();
    res.redirect("/auth");
  }else {
    var alert = new Alert(error, "alert-circle");
    alert.Color = "danger";
    var ap = new AuthPage("Auth");
    ap.AddElements([
      "",
      `<form class="step-form" action="" method="POST">
        <img src="../assets/images/ROTOOLS LOGO.png" alt="logo"style="width: 200px;" />
        <h2 class='display-2'>Welcome, ${uData.username}</h2>
        <h4>Lets setup your account.</h4>
        ${alert.Render()}
                <fieldset>
                <h5 class="float-left">Lets get you a password.</h5>
                  <div class="form-group">
                    <input class="form-control" type="password" name="pass" placeholder="Password">
                  </div>
                  <div class="form-group">
                    <input class="form-control" type="password" name="cpass" placeholder="Confirm Password">
                  </div>
                  <h5 class="float-left">We need you to confirm some things.</h5>
                  <br/><br/>
                  <div class="icheck-square float-left">
                          <input tabindex="5" type="checkbox" name="privacy" id="square-checkbox-1">
                          <label for="square-checkbox-1">I agree to the <a href="/doc/privacy">Privacy Policy</a></label>
                        </div>
                         <br/><br/>
                <div class="icheck-square float-left">
                          <input tabindex="5" type="checkbox" name="tos" id="square-checkbox-1">
                          <label for="square-checkbox-1">I agree to the <a href="/doc/tos">Terms of Service</a></label>
                        </div> <br/><br/>
                  <button class="btn btn-inverse-primary next action-button float-left" type="submit" name="next" value="Next">Continue</button>
                </fieldset>
              </form>`
    ])
    res.send(ap.Render());
  }
})

app.get("/", async (req,res)=>{
  if (req.query.token) {
    res.cookie("obid", req.query.token);
    return res.redirect("/onboarding");
  }
  if (!req.cookies.obid) return res.redirect("/auth");
  var role = await GetUserRoleFromToken(req.cookies.obid);
  if (!role || role != "root") return res.redirect("/auth");
  var accs = await user.find({});
  if (accs.length > 1) return res.redirect("/auth");
  var ap = new AuthPage("Auth");
  ap.AddElements([
    "",
    `<form class="step-form" action="" method="POST">
        <img src="../assets/images/ROTOOLS LOGO.png" alt="logo"style="width: 200px;" />
        <h2 class='display-2'>Welcome to RoTools.</h2>
        <h4>Lets setup your account.</h4>
                <fieldset>
                <h5 class="float-left">We need to collect some info.</h5>
                  <div class="form-group">
                    <input class="form-control" type="text" name="username" placeholder="Roblox Username">
                  </div>
                  <div class="form-group">
                    <input class="form-control" type="email" name="email" placeholder="Email Address">
                  </div>

                  <button class="btn btn-inverse-primary next action-button float-left" type="submit" name="next" value="Next">Continue</button>
                </fieldset>
              </form>`
  ])
  return res.send(ap.Render());
})
app.post("/",parser, async (req,res)=>{
  if (!req.cookies.obid) return res.redirect("/auth");
  var role = await GetUserRoleFromToken(req.cookies.obid);
  if (!role || role != "root") return res.redirect("/auth");
  var email = req.body.email;
  var username = req.body.username;
  var userId = await noblox.getIdFromUsername(username);
  var tb = await axios.get(`https://www.roblox.com/headshot-thumbnail/json?userId=${userId}&width=180&height=180`)
  const pfp = tb.data.Url;

  var id = await user.create({
    username: username,
    creation_date: new Date(),
    role: "owner",
    email: email,
    pfp: pfp
  })
  var token = CreateOnBoard(id._id)
  res.redirect("/onboarding/user?token=" + token);
})


export default app;