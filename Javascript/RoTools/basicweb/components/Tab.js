import { RandomString } from "../util.js";
import { Renderable } from "./Renderable.js";
import { Colors } from "./Theme.js";

export class Tab extends Renderable {
    TabId;
    Color = Colors.Primary;
    /**
     * @type {TabPage[]}
     */
    TabPages = [];
    constructor(){
        super();
        this.TabId = RandomString(10);
    }
    #RenderTabButtons = ()=>{
        var isHome = true;
        var strx = "";
        for (var item of this.TabPages) {
            strx += `<li class="nav-item">
                        <a class="nav-link ${isHome ? `active` : ""}" id="t_${this.TabId}_${item.Name.toLowerCase()}_" data-toggle="tab" href="#t_${this.TabId}_${item.Name.toLowerCase()}" role="tab" aria-controls="t_${this.TabId}_${item.Name.toLowerCase()}" aria-selected="${isHome}">${item.Name}</a>
                      </li>`;
            isHome = false;
        }
        return strx;
    }
    #RenderTabPages = ()=>{
        var isHome = true;
        var strx = "";
        for (var item of this.TabPages) {
            strx += `<div class="tab-pane fade show ${isHome ? `active` :""}" id="t_${this.TabId}_${item.Name.toLowerCase()}" role="tabpanel" aria-labelledby="t_${this.TabId}_${item.Name.toLowerCase()}_">
                        ${item.Render()}
                      </div>`;
            isHome = false;
        }
        return strx;
    }
    AddTab(tabItem){
        this.TabPages.push(tabItem)
    }
    GetChildren() {
        return this.TabPages;
    }
    Render() {
        //tab-solid-${this.Color}
        return `
                    <ul class="nav nav-tabs " role="tablist">
                      ${this.#RenderTabButtons()}
                    </ul>
                   
                    <div class="tab-content tab-content-solid">
                        ${this.#RenderTabPages()} 
                    </div>`
    }
}

export class TabPage extends Renderable {
    Name = "";
    constructor(name, content) {
        super();
        this.Name = name;
        this.AddElements(content)
    }
    Render = this.RenderChildren;
}