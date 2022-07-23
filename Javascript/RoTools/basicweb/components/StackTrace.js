import { Renderable } from "./Renderable.js";

export class StackTrace extends Renderable {
    CSS = ["/assets/css/stack.css"]
    /**
     * The stack items
     * @type {Array<StackItem | StackPackage>}
     */
    Children = [];
    constructor(stack) {
        super();
        var lines = stack.split(/\r?\n/);
        for (var line of lines) {
            var iserrorStackFrame = line.includes(",");
            line = line.replace(",",":")
            if (!line.includes(":")) continue;
            var item = new StackItem(line, iserrorStackFrame);
            if (item.IsFunction) {
                var foundStackPackage = false;
                for (var x in this.Children){
                    if (this.Children[x] && this.Children[x].PackageName == item.Source)
                    {
                        foundStackPackage = true;
                        this.Children[x].Children.push(item);
                    }
                }
                if (!foundStackPackage){
                    var pkg = new StackPackage(item.Source);
                    pkg.Children.push(item);
                    this.Children.push(pkg);
                }
            }else {
                this.Children.push(item);
            }
        }
    }
    Render = this.RenderChildren;
}
export class StackItem extends Renderable {

    /**
     * The name of the function
     * @type {String}
     */
    FunctionName;
    /**
     * The line the function occured on
     * @type {Number}
     */
    Line;
    /**
     * The source of the stack trace
     * @type {String}
     */
    Source;
    /**
     * Whether this stack item is a function
     * @type {Boolean}
     */
    IsFunction = false;

    constructor(line, isErrorFrame) {
        super();

        if (isErrorFrame) {
            if (StackItem.IsFunction(line)) {
                this.IsFunction = true
                this.Source = line.split(":")[0];
                this.Line = Number(line.split(":")[1].split(" ")[0]);
                this.FunctionName = line.split(":")[1].split(" ")[2].replace(" function ", "");
            } else if (line.includes(":")) {
                this.Source = line.split(":")[0];
                if (line.split(":")[1].includes(" "))
                    this.Line = Number(line.split(": line")[1]);
                else
                    this.Line = Number(line.split(":")[1]);
            }
            return;
        }

        if (StackItem.IsFunction(line)) {
            this.IsFunction = true
            this.Source = line.split(":")[0];
            this.Line = Number(line.split(":")[1].split(" ")[0]);
            this.FunctionName = line.split(":")[1].split(" ")[2].replace(" function ", "");
        } else if (line.includes(":")) {
            this.Source = line.split(":")[0];
            console.log(line);
            if (line.split(":")[1].includes(" "))
                this.Line = Number(line.split(":")[1].split(" ")[0]);
            else
                this.Line = Number(line.split(":")[1]);
        }
    }
    static IsFunction(line) {
        return line.includes(" function ");
    }
    Render() {
        if (!this.IsFunction) {
            return `<div class="group ng-scope has-exception">
   <ul>
      <li class="line app ng-binding ng-scope li-no-hover">
         ${this.Source}&nbsp;<span class="label label-line ng-binding ng-scope">line ${this.Line}</span>
      </li>
   </ul>
</div>`
        }
        else {
            return `  <li class="line ng-binding ng-scope li-no-hover">
               ${this.Source}.${this.FunctionName}()&nbsp;<span class="label label-line ng-binding ng-scope">line ${this.Line}</span>
            </li>`
        }
    }
}
export class StackPackage extends Renderable {
    /**
     * The name of the package
     * @type {String}
     */
    PackageName;
    /**
     * The stack item
     * @type {Array<StackItem>}
     */
    Children =[];
    Render(){
        return `<div class="group ng-scope active">
   <ul class="vendor">
      <li class="line ng-scope">
         <span class="vendor-header"> <span class="vendor-name badge badge-primary ng-binding">${this.PackageName}</span>&nbsp; <small class="vendor-calls text-muted ng-binding">${this.Children.length} calls</small> </span> 
         <ul class="vendor-body">
            ${this.RenderChildren()}
         </ul>
      </li>
     </ul>
</div>
`
    }
    constructor(name){
        super();
        this.PackageName = name;
    }
    IsStackPage = true;

}