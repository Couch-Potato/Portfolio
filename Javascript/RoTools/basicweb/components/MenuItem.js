import { Renderable } from "./Renderable.js";

export class MenuItem extends Renderable {
    Name;
    URL;
    IsActive = false;
    Render() {
        return `  <li class="nav-item">
                    <a class="nav-link ${this.IsActive ? "active" : ""}" href="${this.URL}">${this.Name}</a>
                  </li>`
    }
    constructor(name, url) {
        super();
        this.Name = name;
        this.URL = url;
    }
}

export class MenuHolder extends Renderable {
    /**
     * @type {String}
     */
    Name;
    Icon;
    IsActive = false;
    #SanitizedName;
    constructor(name, icon){
        super();
        this.#SanitizedName = name.replace(" ", "-").toLowerCase();
        this.Name = name;
        this.Icon = icon;
    }
    Render() {
        return `<li class="nav-item ${this.IsActive ? "active" : ""}">
              <a class="nav-link" data-toggle="collapse" href="#${this.#SanitizedName}" aria-expanded="false" aria-controls="${this.#SanitizedName}">
                <i class="menu-icon mdi mdi-${this.Icon}"></i>
                <span class="menu-title">${this.Name}</span>
                <i class="menu-arrow"></i>
              </a>
              <div class="collapse ${this.IsActive ? "show" : ""}" id="${this.#SanitizedName}">
                <ul class="nav flex-column sub-menu">
                  ${this.RenderChildren()}
                </ul>
              </div>
            </li>`
    }

    /**
     * Creates a menu array.
     * @param {Array<{Name, URL}>} arr Menu array.
     */
    Populate(arr) {
        for (var x of arr) {
            this.Children.push(new MenuItem(x.Name, x.URL));
        }
    }


}

export class UserMenu extends MenuItem {
    Render(){
        return `<a class="dropdown-item" href="${this.URL}"> ${this.Name} </a>`
    }
}

export class SingletonMenu extends Renderable {
    Name;
    Icon;
    URL;
    IsActive = false;
    #SanitizedName;
    constructor(name, icon, url) {
        super();
        this.#SanitizedName = name.replace(" ", "-").toLowerCase();
        this.Name = name;
        this.Icon = icon;
        this.URL = url;
    }
    Render() {
        return `<li class="nav-item ${this.IsActive ? "active" : ""}">
              <a class="nav-link" href="${this.URL}">
                <i class="menu-icon mdi mdi-${this.Icon}"></i>
                <span class="menu-title">${this.Name}</span>
              </a>
            </li>`
    }
}