"use strict";
import { Renderable } from "./Renderable.js"

function concatNoDuplicate(a1, a2noDup) {
  for (var x of a2noDup) {
    var match = false
    for (var i of a1) {
      if (i == x) {
        match = true
      }
    }
    if (!match)
      a1.push(x);
  }
  return a1;
}

export class Page extends Renderable {
  constructor(title) {
    super();
    this.PageTitle = title
  }

  PageTitle = "RoTools"

  FindAllAssets = function (object) {
    for (var x of object.GetChildren()) {
      if (x.Renderable){
        this.CSS = concatNoDuplicate(this.CSS, x.CSS)
        this.InlineScripts = concatNoDuplicate(this.InlineScripts, x.InlineScripts)
        this.Scripts = concatNoDuplicate(this.Scripts, x.Scripts)
        this.FindAllAssets(x)
      }
    }
  }

  RenderChildren = function () {
    var render = "";
    for (var x of this.Children) {
      if (x.Renderable){
        render += x.Render()
      }else {
        render += x;
      }
      
    }
    return render
  }

  /**
   * @type {{Scripts:String, CSS:String}}
   */
  Rendered = {
    Scripts : "",
    CSS : ""
  }
  
  RenderPage(){
    return `<head>${this.Rendered.CSS}</head><body>${this.RenderChildren()}${this.Rendered.Scripts}</body>`;
  }

  /**
   * Renders a page into a string to be displayed to a user.
   * @returns {String}
   */
  Render() {
    var renderCss = ""
    var renderScript = ""
    this.FindAllAssets(this)
    for (var x of this.CSS)
      renderCss += `<link rel="stylesheet" href="${x}">`
    for (var x of this.Scripts)
      renderScript += `<script src="${x}"></script>`
    for (var x of this.InlineScripts)
      renderScript += `<script>${x}</script>`
    this.Rendered.CSS = renderCss;
    this.Rendered.Scripts = renderScript;
    return this.RenderPage();
  }
}