import { DataModelBase } from "./DataModelBase.js";
import server from "./models/server.js";
import { Player, Players } from "./Players.js";

export class Server extends DataModelBase {
    InnerId;
    RobloxId;
    /**
     * @type {Players}
     */
    Players;
    MaxPlayers;
    Macros;
    constructor(internal_id) {
        super();
        this.InnerId = internal_id;
    }
    /**
     * Fetches the record from the mongodb database.
     */
    async Open() {
        this.InnerRecord = await server.findById(this.InnerId)
        this.IsOpened = true;
        this.RobloxId = this.InnerRecord.roblox_id;
        this.Players = new Players(this.InnerRecord.players);
        this.Macros = this.InnerRecord.macros;
        this.MaxPlayers = this.InnerRecord.max_players;
    }
    /**
     * Adds a player to a server datamodel.
     * @param {String} name Their roblox username
     * @param {Number} id Their roblox userid
     * @returns {Player} The player added.
     */
    async AddPlayer(name, id) {
        var plyr = new Player(name, id);
        console.log(JSON.parse(JSON.stringify(this.Players)))
        this.Players.Add(plyr);
        console.log(JSON.parse(JSON.stringify(this.Players)))
        //Add this after we append to player array to prevent circularness...
        await this.Save();
        plyr.ServerReference = this;
        return plyr;
    }
    /**
     * Saves the datamodel as it is currently set. 
     */
    async Save() {
        console.log(JSON.parse(JSON.stringify(this.Players)))
        this.InnerRecord.players = this.Players;
        await super.Save();
    }
}