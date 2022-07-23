"use strict";
export class Renderable {
    constructor() { }
    /**
     * @type {Array<Renderable|String>}
     */
    Children = []
    /**
     * @type {any}
     */
    Attributes = {}
    /**
     * @type {Array<String>}
     */
    Scripts = []
    /**
     * @type {Array<String>}
     */
    CSS = []
    /**
     * @type {Array<String>}
     */
    InlineScripts = []
    Class = "";
    Id = "";
    Body = "";
    /**
     * Renders the element.
     * @returns {String}
     */
    Render() {
        return ``
    }
    /**
     * Appends a child element.
     * @param {Renderable} renderable The child element. 
     */
    AddElement(renderable) {
        this.Children.push(renderable)
    }

    /**
     * Appends several elements to the parent.
     * @param {Array<Renderable>} renderables Child elements
     */
    AddElements(renderables) {
        this.Children = this.Children.concat(renderables);
    }

    /**
     * @returns {String}
     */
    RenderChildren() {
        var result = "";
        for (var x of this.GetChildren()) {
            if (x.Renderable) {
                result += x.Render();
            } else {
                result += x;
            }

        }
        return this.Body + result;
    }
    /**
     * @returns {String}
     */
    RenderAttributes() {
        var attb = "";
        for (var i in this.Attributes) {
            attb += `${i}="${this.Attributes[i]}" `;
        }
        return attb;
    }

    /**
     * Sets a CSS value of the renderable
     * @param {String} key They css property to set.
     * @param {any} val The value
     */
    SetCSS(key, val) {
        if (this.Attributes.style) {
            this.Attributes.style += ` ${key}: ${val};`
        } else {
            this.Attributes.style = `${key}: ${val};`
        }
    }

    /**
     * Gets all renderable children
     * @type {Renderable}
     */
    GetChildren() {
        return this.Children;
    }
    Renderable = true;

    /**
     * Renders an array of elements.
     * @param {Array<Renderable|String>} arr The renderables
     * @returns {String}
     */
    static RenderMany(arr) {
        var rendered = "";
        for (var x of arr) {
            if (x.Renderable)
                rendered += x.Render();
            else
                rendered += x;
        }
        return rendered;
    }
}