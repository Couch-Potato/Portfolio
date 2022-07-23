import mongoose, { Model, Document } from "mongoose";

export interface IRoToolsInstance extends Document {
    owner: string, // Metadata here
    privateKey: string,
    gameKey: string,
    online: boolean,
    hostname:string,
    dbUser:string
}

const RoToolsInstanceSchema = new mongoose.Schema({
    owner: String, // Metadata here
    privateKey: String,
    gameKey: String,
    online: Boolean,
    hostname:String,
    dbUser:String
});

const RoToolsInstance: Model<IRoToolsInstance> = mongoose.model("Instance", RoToolsInstanceSchema);
export default RoToolsInstance;