import { Renderable } from "./Renderable.js";

export class Alert extends Renderable {
    /**
     * @type {"primary" | "success" | "info" | "warning" | "danger"}
     */
    Color = "primary";
    Icon;
    Text;
    Render(){
        return `<div class="alert alert-${this.Color}" role="alert">
                      <i class="mdi mdi-${this.Icon}"></i> ${this.Text} ${this.RenderChildren()} </div>`
    }
    constructor(text, icon) {
        super();
        this.Text = text;
        this.Icon = icon;
    }
}