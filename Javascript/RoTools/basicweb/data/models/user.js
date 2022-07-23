import mongoose from "mongoose";

var schema =  new mongoose.Schema({
    username:String,
    password:String,
    creation_date:Date,
    roles:{type:String, default:"000"},
    role:String,
    pfp:String,
    disabled:Boolean,
    deleted:Boolean,
    email:String
});

String.prototype.replaceAt = function (index, replacement) {
    return this.substr(0, index) + replacement + this.substr(index + replacement.length);
}

schema.methods.getRoleAtIndex = function (index) {
    if (this.roles.charAt(index)){
        var role = this.roles.charAt(index);
        if (role == "0") {
            return "";
        }else if (role == "1") {
            return "r";
        }else if (role == "2") {
            return "rw";
        }
    }else {
        return "";
    }
}
schema.methods.setRoleAtIndex = async function (index, permission) {
    var role = this.roles.charAt(index);
    var xperm = "";
    if (permission == "")
        xperm = "0"
    if (permission == "r")
        xperm = "1"
    if (permission == "rw")
        xperm = "2"
    this.roles = role.replaceAt(index, xperm);
    await this.save();
}

export default mongoose.model("User", schema);
