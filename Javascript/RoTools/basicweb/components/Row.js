import { Renderable } from "./Renderable.js";

export class Row extends Renderable{
    constructor(items = []){
        super();
        this.AddElements(items);
    }
    Render(){
        return `<div class="row">${this.RenderChildren()}</div>`
    }
}