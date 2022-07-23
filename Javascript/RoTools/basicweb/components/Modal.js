import { Renderable } from "./Renderable.js"
import {Button} from "./Button.js"
import { RandomString } from "../util.js";
export class Modal extends Renderable{
    IsForm = false;
    FormUrl = "";
    ModalButton = new Button();
    Title = ""
    Scrollable = false;
    Centered = true;
    Size = ModalSize.Regular
    Render(){
        var id = "a" + RandomString(10);
        this.ModalButton.Attributes = {
            "data-toggle":"modal",
            "data-target":`#${id}`
        }
        return ` <div class="text-center">
                      ${this.ModalButton.Render()}
                    </div>
                    <div class="modal fade" id="${id}" tabindex="-1" role="dialog" aria-labelledby="${id}Label" aria-hidden="true">
                      <div class="modal-dialog ${this.Size} ${this.Centered ? `modal-dialog-centered` : ""} ${this.Scrollable ?`modal-dialog-scrollable`:""}" role="document">
                        <div class="modal-content">
                          <div class="modal-header">
                            <h5 class="modal-title" id="${id}Label">${this.Title}</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                              <span aria-hidden="true">&times;</span>
                            </button>
                          </div>
                        ${this.IsForm ? `<form method="POST" action="${this.FormUrl}">`:""}
                          <div class="modal-body">
                            ${this.RenderChildren()}
                          </div>
                          <div class="modal-footer">
                            ${this.IsForm ? ` <button type="submit" class="btn btn-inverse-primary">Submit</button>
                            <button type="button" class="btn btn-inverse-light" data-dismiss="modal">Cancel</button>` :`<button type="button" class="btn btn-light" data-dismiss="modal">Exit</button>`}
                            
                          </div>
                        ${this.IsForm ? `</form>` : ""}

                        </div>
                      </div>
                    </div>`
    }
    constructor(title){
        super();
        this.Title = title;
        this.ModalButton.Text = title;
    }
}

export let ModalSize = {
    Regular:"",
    Large:"modal-lg",
    Small:"modal-sm"
}