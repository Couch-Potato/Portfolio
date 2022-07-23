import mongoose from "mongoose";
import {env, getConnectionString} from "../env.js";
export default () => {
    mongoose.connect(getConnectionString("rotools-controller"), { useNewUrlParser: true, useUnifiedTopology: true });
    const db = mongoose.connection;
    db.on('error', console.error.bind(console, 'connection error:'));
    db.once('open', function () {
        console.log('connected to db');
    });
}