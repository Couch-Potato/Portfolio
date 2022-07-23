import { Renderable } from "./Renderable.js";

export class Timeline extends Renderable {
    Render(){
        return `<div class="row">
                    <div class="col-12 mt-5">
                      <div class="stage-wrapper pl-4">
                       ${this.RenderChildren()}
                      </div>
                    </div>
                  </div>`
    }
}
export class TimelineEntry extends Renderable {
    Subject = "";
    Time = "";
    Caption = "";
    constructor(subject, caption, time){
        super()
        this.Subject=subject;
        this.Time = time;
        this.Caption = caption;
    }
    Render(){
        return `<div class="stages border-left pl-5 pb-4">
                          <div class="d-flex align-items-center mb-2 justify-content-between flex-wrap">
                            <h5 class="mb-0 mr-2">${this.Subject}</h5>
                            <small class="text-muted">${this.Time}</small>
                          </div>
                          <p>${this.Caption}</p>
                        </div>`
    }
}