import { DataArray } from "./DataModelBase.js";
import { Server } from "./Server.js";

export class Players extends Array {
    /**
     * @param {Player} player A player object
     */
    Add(player){
        this.push(player);
    }
    /**
     * Gets a player by their userid
     * @param {Number} userid Their roblox userid
     * @returns {Player}
     */
    GetById(userid){
        for (var x of this){
            if (x.UserId == userid)
                return x;
        }
    }
    /**
     * Gets a player by their username
     * @param {String} username Their roblox username
     * @returns {Player}
     */
    GetByName(username){
        for (var x of this){
            if (x.Username == username)
                return x;
        }
    }
    constructor(playerList){
        super();
        if (playerList){
            for (var x of playerList)
            {
                this.push(new Player(x.Username, x.UserId));
            }
        }
    }
}
export class Player {
    /**
     * @type {String}
     */
    Username;
    /**
     * @type {Number}
     */
    UserId;
    /**
     * @type {Server}
     */
    ServerReference;
    constructor(un, uid){
        this.Username=un;
        this.UserId=uid
    }
    Ban()
    {
        console.log("Banned");
    }
}