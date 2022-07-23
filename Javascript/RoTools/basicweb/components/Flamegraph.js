import { Renderable } from "./Renderable.js";
import { RandomString } from "../util.js";
export class Flamegraph extends Renderable {
    Dataset = "http://martinspier.io/d3-flame-graph/stacks.json";
    CSS = ["https://cdn.jsdelivr.net/npm/d3-flame-graph@4.0.6/dist/d3-flamegraph.css", "/assets/css/flame.css"]
    Scripts = ["https://d3js.org/d3.v4.min.js", "https://cdn.jsdelivr.net/npm/d3-flame-graph@4.0.6/dist/d3-flamegraph.min.js"]
    Width = 1000;
    CellHeight = 28;
    Render(){
       
        return `<div id="a_${this.Id}">
        </div>
        <hr>
        <div id="d_${this.Id}">
        </div>
`
    }
    constructor(source){
        super();
        this.Dataset = source;
        this.Id = RandomString(10);

        this.InlineScripts = [`var x_${this.Id} = flamegraph()
            .width(${this.Width})
            .cellHeight(${this.CellHeight})
            .transitionDuration(750)
            .minFrameSize(5)
            .transitionEase(d3.easeCubic)
            .sort(true)
            .title("Stack Performance")
            .selfValue(false)
            .setColorHue("aqua")

        var dx_${this.Id} = document.getElementById("d_${this.Id}");
        x_${this.Id}.setDetailsElement(d_${this.Id});
        d3.json("${this.Dataset}", function (error, data) {
            if (error) return console.warn(error);
            d3.select("#a_${this.Id}")
                .datum(data)
                .call(x_${this.Id});
        });
        `]
    }
}
