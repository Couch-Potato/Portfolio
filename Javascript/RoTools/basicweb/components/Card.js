import { Column } from "./Column.js";
import { Renderable } from "./Renderable.js";

export class Card extends Column{
    Title = "Card";
    /**
     * @type {CardFooter}
     */
    Footer;
    Render(){
        var tChildren = this.RenderChildren();
        return this.RenderWithStringBody(`<div class="card">
                <div class="card-body">
                  <h4 class="card-title">${this.Title}</h4>
                  ${tChildren}
                </div>${this.Footer? this.Footer.Render() : ""}
              </div>`)
    }
    constructor(title, size)
    {
        super(size);
        this.Title = title;
    }
}
export class CardFooter extends Renderable {
  Render(){
    return `<div class="card-footer">${this.RenderChildren()}</div>`
  }
  /**
   * Creates a new card footer
   * @param {Renderable[]} items 
   */
  constructor(items) {
    super();
    this.Children = items;
  }
}