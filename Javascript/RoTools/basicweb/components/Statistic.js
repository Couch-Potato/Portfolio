import { Renderable } from "./Renderable.js";

export class Statistic extends Renderable {
    Color = "";
    SubText = "";
    Value;
    Title=""
    constructor(color, title, sub, val){
        super();
        this.Value = val;
        this.Title = title;
        this.SubText = sub;
        this.Color = color;
    }
    Render(){
        return `<div class="col-md-4 grid-margin stretch-card">
                <div class="card aligner-wrapper">
                  <div class="card-body">
                    <div class="absolute left top bottom h-100 v-strock-2 bg-${this.Color}"></div>
                    <p class="text-muted mb-2">${this.Title}</p>
                    <div class="d-flex align-items-center">
                      <h1 class="font-weight-medium mb-2">${kFormatter(this.Value)}</h1>
                    </div>
                    <div class="d-flex align-items-center">
                      <div class="bg-${this.Color} dot-indicator"></div>
                      <p class="text-muted mb-0 ml-2">${this.SubText}</p>
                    </div>
                  </div>
                </div>
              </div>`
    }
}

function kFormatter(num) {
    return Math.abs(num) > 999 ? Math.sign(num) * ((Math.abs(num) / 1000).toFixed(1)) + 'k' : Math.sign(num) * Math.abs(num)
}