import { Renderable } from "./Renderable.js";

export class AvatarRenderer extends Renderable {
    Picture;
    constructor(pic){
        super();
        this.Picture = pic;
    }
    Render() {
        return `<img class="img-xs rounded-circle" src="${this.Picture}" alt="Profile image">`
    }
}