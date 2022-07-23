import { Renderable } from "./Renderable.js";
import dateformat from "dateformat";
import { LogViewer } from "./LogViewer.js";
import { ButtonStyles, ButtonTypes } from "./Button.js";
export class Issue extends Renderable {
    Name;
    RecentEvents;
    Source;
    ServerId;
    Start;
    End;
    Hash;
    Message;
    LogView;

    /**
     * Creates a new issue.
     * @param {String} name Name of the issue
     * @param {Number} recent Recent events
     * @param {String} source Source stack
     * @param {any} objec Object referenced
     * @param {String} message The message
     * @param {String} serverid Serverid
     * @param {Date} start Start of issue
     * @param {Date} end Latest issue time
     * @param {String} hash Issue hash
     */
    constructor(name, recent, source, objec, message, serverid, start, end, hash, issueId) {
        super();
        this.LogView = new LogViewer(name, source, objec, message, "ISSUE", "danger")
        this.Source = this.LogView.StackView.Children[0].Source;
        this.ServerId = serverid;
        this.RecentEvents = recent;
        this.Start = start;
        this.End = end;
        this.Hash = hash;
        this.Name = name;
        this.Message=message;
        this.LogView.ModalButton.Text="View"
        this.LogView.ModalButton.FixedWidth = false;
        this.LogView.ModalButton.Style = ButtonStyles.Inverse;
        this.IssueId = issueId;
    }

    IssueId;
    Render(){
        return `<div class="col-lg-4 col-md-4 col-sm-6 col-12 project-grid">
                          <div class="project-grid-inner" style="">
                            <div class="d-flex align-items-start">
                              <div class="wrapper">
                                <h5 class="project-title">${this.Name}</h5>
                                <p class="project-location text-danger">Issue</p>
                              </div>
                              <div class="badge badge-inverse-danger ml-auto">${this.Source}</div>
                            </div>
                            <p class="project-description"><span>${this.Message}</span></p>
                            <div class="d-flex justify-content-between">
                              <p class="mb-2 font-weight-medium">Recent Events</p>
                              <p class="mb-2 font-weight-medium">${this.RecentEvents}</p>
                            </div>
                            <div class="d-flex justify-content-between align-items-center">
                              <div class="action-tags d-flex flex-row">
                                <a type="button" class="btn btn-inverse-primary" href="/server/${this.ServerId}">View Server</a>
                                &nbsp;
                                <a type="button" class="btn btn-inverse-warning" href="/issues/ignore/${this.IssueId}">Ignore</a>
                                &nbsp;
                                ${this.LogView.Render()}
                                &nbsp;
                              </div>
                            </div>
                            <hr />
                            <div class="d-flex justify-content-between align-items-center">
                              Start Date: ${dateformat(this.Start, "mm/dd/yy")}&nbsp;&nbsp;&nbsp;&nbsp;Latest: ${dateformat(this.End, "mm/dd/yy")}
                            </div>
                            <div class="d-flex justify-content-between align-items-center">
                              Hash: <small>${this.Hash}</small>
                            </div>
                          </div>
                        </div>`
    }
    GetChildren(){
        return this.LogView.Children;
    }
}
export class IssueList extends Renderable {
    Render() {
      return `<div class="row project-list-showcase">${this.RenderChildren()}</div>`
    }
}