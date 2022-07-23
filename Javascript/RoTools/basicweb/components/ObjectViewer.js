import { RandomString } from "../util.js";
import { Renderable } from "./Renderable.js";

export class ObjectViewer extends Renderable {
    CSS = ["/assets/vendors/jstree/themes/default/style.min.css"]
    Scripts = ["/assets/vendors/jstree/jstree.min.js"]
    TreeObject = {};
    /**
     * The content of the tree.
     * @type {String}
     */
    #Content = "";

    constructor(object){
        super();
        this.TreeObject = object;
        var id = "a" + RandomString(12);
        this.Id = id;
        this.InlineScripts = [`
        $('#${id}').jstree({
        "types": {
            "default": {
                "icon": "/assets/images/file-icons/extension/folder.png"
            },
            "file": {
                "icon": "/assets/images/file-icons/extension/doc-file.png",
                "valid_children": []
            },
            'f-open': {
                'icon': 'fa fa-folder-open fa-fw'
            },
            'f-closed': {
                'icon': 'fa fa-folder fa-fw'
            }
        },
        "core": {
            "variant": "large",
            "check_callback": true
        },
        "plugins": [
            "types",
            "dnd",
            "wholerow"
        ]
    });
        `]
    }

    ProcessContent(parent, treeItem) {
        for (var i in parent)
        {
            if (typeof parent[i] == "object"){
                var appendTo = new TreeItem("/assets/images/file-icons/extension/folder.png", i);
                this.ProcessContent(parent[i], appendTo);
                treeItem.Children.push(appendTo);
            }else {
                var item = new TreeItem("/assets/images/file-icons/extension/doc-file.png", i, parent[i]);
                treeItem.Children.push(item);
            }
        }
        this.#Content = treeItem.Render();
    }

    Render() {
        
        var root = new TreeItem("/assets/images/file-icons/extension/server.png", "Object")
        root.IsOpen = true;
        this.ProcessContent(this.TreeObject, root);
        
        return `<div id="${this.Id}">
                          <ul>
                            ${this.#Content}
                          </ul>
                        </div>`
        
    }

}

export class TreeItem extends Renderable {
    /**
     * The item's icon.
     * @type {String}
     */
    Icon;
    /**
     * The name of the item.
     * @type {String}
     */
    Name;
    /**
     * Child Items
     * @type {Array<TreeItem>}
     */
    Children = [];

    Value;

    /**
     * Whether or not the default state of the tree item is open.
     * @type {Boolean}
     */
    IsOpen = false;

    constructor(icon, name, val = 0){
        super();
        this.Icon = icon;
        this.Name = name;
        this.Value = val;
    }

    Render(){
        if (this.Children.length > 0) {
            return `<li data-jstree='{"icon":"${this.Icon}"}' ${this.IsOpen ? `class="jstree-open"`: ""}>${this.Name}<ul>
                                ${this.RenderChildren()}
                              </ul>
                            </li>`
        }else {
            return `<li data-jstree='{"icon":"${this.Icon}"}'>${this.Name} = ${this.Value}</li>`
        }
    }
}