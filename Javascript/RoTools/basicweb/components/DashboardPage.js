"use strict";
import { Badge } from "./Badge.js";
import { MenuHolder, MenuItem, UserMenu } from "./MenuItem.js";
import {Page} from "./Page.js"
export class DashboardPage extends Page{
    Scripts = [
        "/assets/vendors/js/vendor.bundle.base.js",
        "/assets/js/shared/off-canvas.js",
        "/assets/js/shared/hoverable-collapse.js",
        "/assets/js/shared/misc.js",
        "/assets/js/shared/settings.js",
        "/assets/js/shared/todolist.js",
        "/assets/js/main/dashboard.js"
    ]
    CSS = [
        "/assets/vendors/mdi/css/materialdesignicons.min.css",
        "/assets/vendors/flag-icon-css/css/flag-icon.min.css",
        "/assets/vendors/ti-icons/css/themify-icons.css",
        "/assets/vendors/typicons/typicons.css",
        "/assets/vendors/css/vendor.bundle.base.css",
        "/assets/css/shared/style.css",
        "/assets/css/main/dark.css",
    ]

    /**
     * @type {Array<MenuHolder>}
     */
    MenuItems = [];

    /**
     * @type {UserMenu}
     */
    UserMenuItems = [];

    Username = "";

    ProfilePicture = "https://tr.rbxcdn.com/3d035a67f15d1d84ee11507affaac230/150/150/AvatarHeadshot/Png";

    SetActive(route) {
        var holder = route.split('/')[0];
        var item = route.split('/')[1];
        for (var x of this.MenuItems){
            if (x.Name == holder) {
                x.IsActive = true;
                for (var i of x.Children){
                    if (i.Name == item)
                        i.IsActive = true;
                }
            }
        }
    }

    RenderPage() {
        return `<!DOCTYPE html>
<html lang="en">
  <head>
    <!-- Required meta tags -->
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <title>RoTools - ${this.PageTitle}</title>
    <!-- plugins:css -->
    ${this.Rendered.CSS}
  </head>
  <body>
    <div class="container-scroller">
      <!-- partial:partials/_navbar.html -->
      <nav class="navbar default-layout col-lg-12 col-12 p-0 fixed-top d-flex flex-row">
        <div class="text-center navbar-brand-wrapper d-flex align-items-top justify-content-center">
          <a class="navbar-brand brand-logo" href="/">
            <img src="../assets/images/ROTOOLS LOGO.svg" alt="logo"style="width: 150px;" />
          </a>
          <a class="navbar-brand brand-logo-mini" href="/">
            <img src="" alt="logo" />
          </a>
        </div>
        <div class="navbar-menu-wrapper d-flex align-items-center">
          <button class="navbar-toggler navbar-toggler align-self-center" type="button" data-toggle="minimize">
            <span class="mdi mdi-menu"></span>
          </button>
          <form action="#" class="form form-search ml-auto d-none d-md-flex">
          
          </form>
          <ul class="navbar-nav">
            <li class="nav-item dropdown d-none d-xl-inline-flex">
              <a class="nav-link dropdown-toggle pl-4 d-flex align-items-center" id="UserDropdown" href="#" data-toggle="dropdown" aria-expanded="false">
                <div class="count-indicator d-inline-flex mr-3">
                  <img class="img-xs rounded-circle" src="${this.ProfilePicture}" alt="Profile image">
          
                </div>
                <span class="profile-text font-weight-medium">${this.Username}</span>
              </a>
              <div class="dropdown-menu dropdown-menu-right navbar-dropdown" aria-labelledby="UserDropdown">
                    ${UserMenu.RenderMany(this.UserMenuItems)}
              </div>
            </li>
          </ul>
          <button class="navbar-toggler navbar-toggler-right d-lg-none align-self-center" type="button" data-toggle="offcanvas">
            <span class="icon-menu"></span>
          </button>
        </div>
      </nav>
      <!-- partial -->
      <!-- partial:partials/_settings-panel.html -->

      <!-- partial -->
      <div class="container-fluid page-body-wrapper" style="margin-top: 0;">
        <!-- partial:partials/_sidebar.html -->
        <nav class="sidebar sidebar-offcanvas dynamic-active-class-disabled" id="sidebar">
          <ul class="nav">
            ${MenuHolder.RenderMany(this.MenuItems)}
          </ul>
          
        </nav>
        <!-- partial -->
        <div class="main-panel">
          <div class="content-wrapper" style="margin-top: 0;">
            <div class="content-header d-flex flex-column flex-md-row">
              <h4 class="mb-0">${this.PageTitle}</h4>
              
            </div>
            <br/>
            ${this.RenderChildren()}
          </div>
          <!-- content-wrapper ends -->
          <!-- partial:partials/_footer.html -->
          <footer class="footer">
            <div class="container-fluid clearfix">
               <span class="text-muted d-block text-center text-sm-left d-sm-inline-block">Copyright © 2021 <a href="https://rotools.net/" target="_blank">RoTools</a>. All rights reserved. ${new Badge("primary", `v${global.RTVERSION}${global.CONFIGSOCKET ? "" : "-dev"}`).Render()} ${global.INSTANCE_ID ? `<small>${global.INSTANCE_ID}</small>` :""}</span>
              <span class="float-none float-sm-right d-block mt-1 mt-sm-0 text-center">Hand-crafted & made with <i class="mdi mdi-heart text-danger"></i>
              </span>
            </div>
          </footer>
          <!-- partial -->
        </div>
        <!-- main-panel ends -->
      </div>
      <!-- page-body-wrapper ends -->
    </div>
    <!-- container-scroller -->
    ${this.Rendered.Scripts}
    <!-- End custom js for this page -->
  </body>
</html>`
    }
}