import { Renderable } from "./Renderable.js";

export class Icon extends Renderable {
    Icon;
    Pack; 
    /**
     * Creates a new icon
     * @param {string} icon Icon Name 
     * @param {"MaterialDesign" | "FontAwesome"} pack Icon pack it belongs to
     */
    constructor(icon, pack="MaterialDesign") {
        super();
        this.Icon = icon;
        this.Pack = Packs[pack];
    }
    Render(){
        return `<i class="${this.Pack}${this.Icon}"></i>`
    }
}

const Packs = {
    "MaterialDesign" : "mdi mdi-",
    "FontAwesome" : "fa fa-"
}