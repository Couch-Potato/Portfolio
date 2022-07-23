import mongoose from "mongoose";
import { RandomString } from "../../util.js";
const api_model = new mongoose.Schema({
    creator:{type: mongoose.SchemaTypes.ObjectId, ref:"User"},
    client_id:String,
    token:String,
    date:Date
});


api_model.statics.authenticate = async function(client, token) {
    var item = await mongoose.model("APIKey").findOne({
        client_id:client,
        token:token
    })
    var x = await item.execPopulate();
    return x;
}

api_model.statics.getAll = async function() {
    return await this.find({}).populate('creator').exec();
}

api_model.statics.deleteKey = async function(id){
    return await (await this.findById(id)).delete()
}
api_model.statics.generateKey = async function(creator) {
    var client_id = RandomString(24);
    var key = RandomString(128);
    return await this.create({
        creator:creator,
        client_id:client_id,
        token:key,
        date: new Date()
    })
}




export default mongoose.model("APIKey", api_model);