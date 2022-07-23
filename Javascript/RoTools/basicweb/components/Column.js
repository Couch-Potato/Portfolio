import { Renderable } from "./Renderable.js";

export class Column extends Renderable{
    /**
     * The size of the column 1-12
     * @type {Number}
     */
    ColumnSize = 12;
    ColumnVariant = ColumnMaxSize.Medium
    GridMargin = true
    StretchCard = true
    JustifyCenter = false;
    Render(){
        const renderColHeader = this.ColumnSize != 12; // Assume it is a full-size column if it is 12
        return `<div class="${renderColHeader ? `${this.ColumnVariant}${this.ColumnSize}` : ``} ${this.GridMargin ? "grid-margin" : ""} ${this.StretchCard ? "stretch-card" : ""} ${this.JustifyCenter ? "justify-content-center" :""}" ${this.RenderAttributes()}>${this.RenderChildren()}</div>`
    }
    RenderWithStringBody(body){
        const renderColHeader = this.ColumnSize != 12;
        return `<div class="${renderColHeader ? `${this.ColumnVariant}${this.ColumnSize}` : ``} ${this.GridMargin ? "grid-margin" : ""} ${this.StretchCard ? "stretch-card" : ""} ${this.JustifyCenter ? "justify-content-center" : ""}" ${this.RenderAttributes()}>${body}</div>`
    }
    constructor(size){
        super();
        this.ColumnSize = size;
    }
}
/** 
 * The size of the column.
*/
export let ColumnMaxSize = {
    ExtraSmall:"col-",
    Small:"col-sm-",
    Medium:"col-md-",
    Large:"col-lg-",
    ExtraLarge:"col-xl-"
}