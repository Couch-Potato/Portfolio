import { Renderable } from "./Renderable.js";
import { Colors } from "./Theme.js";

export class Button extends Renderable
{
    FixedWidth = false;
    Size = ButtonSizes.Small;
    Color = Colors.Primary;
    Action = "";
    Text = "";
    Style = ButtonStyles.Inverse;
    ButtonType = ButtonTypes.Button;
    Render(){
        return `<button ${this.RenderAttributes()} type="${this.ButtonType}" class="btn ${this.Style}${this.Color} ${this.FixedWidth ? "btn-fw" : ""} ${this.Size}" ${this.Action != "" ? `onclick="${this.Action}"` : ""}>${this.Text}</button>`
    }
}

export class LinkButton extends Button {
    URL;
    NewTab = false;
    Render() {
        return `<a ${this.RenderAttributes()} href="${this.URL}" ${this.NewTab ? `target="_blank"` : ""} class="btn ${this.Style}${this.Color} ${this.FixedWidth ? "btn-fw" : ""} ${this.Size}" ${this.Action != "" ? `onclick="${this.Action}"` : ""}>${this.Text}</a>`
    }
    constructor(text, url, newTab = false) {
        super()
        this.URL = url;
        this.Text = text;
        this.NewTab = newTab;
    }
}

export let ButtonSizes = {
    Large:"btn-lg",
    Medium:"btn-md",
    Small:"btn-sm"
}

export let ButtonTypes = {
    Button:"button",
    Submit:"submit"
}

export let ButtonStyles = {
    Normal:"btn-",
    Outlined:"btn-outline-",
    Inverse:"btn-inverse-"
}