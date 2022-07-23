import { Renderable } from "./Renderable.js";
import showdown from "showdown";

export class MarkdownView extends Renderable {
    #converter = new showdown.Converter();
    /**
     * The markdown text to be turned into HTML.
     * @type {String}
     */
    Markdown = "";
    constructor(text){
        super()
        this.Markdown = text
        this.#converter.setFlavor("github");
        this.#converter.setOption("parseImgDimensions", true);
        this.#converter.setOption("simplifiedAutoLink", true);
        this.#converter.setOption("literalMidWordUnderscores", true);
        this.#converter.setOption("strikethrough", true);
        this.#converter.setOption("tables", true);

        this.#converter.setOption("ghCodeBlocks", true);
        this.#converter.setOption("smartIndentationFix", true);
        this.#converter.setOption("openLinksInNewWindow", true);
        this.#converter.setOption("emoji", true);
        this.#converter.setOption("underline", true);
        this.#converter.setOption("parseImgDimensions", true);
    }
    Render(){
        return this.#converter.makeHtml(this.Markdown)
    }
}
