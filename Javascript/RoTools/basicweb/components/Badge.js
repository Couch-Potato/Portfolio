import { Renderable } from "./Renderable.js";

export class Badge extends Renderable {
    Color;
    Text;
    constructor(color,text){
        super();
        this.Color = color;
        this.Text = text;
    }
    Render(){
        return `<label class="badge badge-inverse-${this.Color}">${this.Text}</label>`
    }
}
export class IconBadge extends Renderable {
    Color;
    Text;
    Icon;
    constructor(color, text, icon) {
        super();
        this.Color = color;
        this.Text = text;
        this.Icon = icon
    }
    Render() {
        return `<label class="badge badge-${this.Color}"><i class="mdi mdi-${this.Icon}"></i> ${this.Text}</label>`
    }
}