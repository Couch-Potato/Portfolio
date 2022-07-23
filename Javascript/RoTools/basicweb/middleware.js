import { Client } from "./modules/auth_client.js";

export default async function(req,res,next){
    // Handle authentication
    req.Client = new Client(req, res);
    await req.Client.AuthToken(req.cookies.token);
    // Handle auth routes
    for (var x of req.endpoints){
        if (x.authentication) {

            var pathOptions = req.path.split("/");

            var vpath = "*/" + pathOptions[1] + "*" ;

            if (vpath.includes("*" + x.endpoint + "*")) {
                if (req.Client.Authenticated) {
                    return next();
                }else {
                    // If we are unauthenticated, we still want to send out our assets.
                    if (vpath.includes("*/assets"))
                        return next();
                    return res.redirect("/auth?redirect=" + req.url)
                }
            }
        }
    }
    next();
}