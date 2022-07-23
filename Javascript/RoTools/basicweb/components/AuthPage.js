import { Page } from "./Page.js"
export class AuthPage extends Page {
    Scripts = [
        "/assets/vendors/js/vendor.bundle.base.js",
        "/assets/js/shared/off-canvas.js",
        "/assets/js/shared/hoverable-collapse.js",
        "/assets/js/shared/misc.js",
        "/assets/js/shared/settings.js",
        "/assets/js/shared/todolist.js",
        "/assets/js/main/dashboard.js",
        "/assets/vendors/icheck/icheck.min.js",
        "/assets/js/shared/iCheck.js"
    ]
    CSS = [
        "/assets/vendors/mdi/css/materialdesignicons.min.css",
        "/assets/vendors/flag-icon-css/css/flag-icon.min.css",
        "/assets/vendors/ti-icons/css/themify-icons.css",
        "/assets/vendors/typicons/typicons.css",
        "/assets/vendors/css/vendor.bundle.base.css",
        "/assets/vendors/icheck/all.css",
        "/assets/css/shared/style.css",
        "/assets/css/main/dark.css",
    ]

    RenderPage(){
        return `<html lang="en">
  <head>
    <!-- Required meta tags -->
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <title>RoTools - Register</title>
    ${this.Rendered.CSS}
  </head>
  <body class="dark-theme">
    <div class="container-scroller">
      <div class="container-fluid page-body-wrapper full-page-wrapper">
        <div class="content-wrapper d-flex align-items-center auth multi-step-login">
          <div class="row w-100">
          <div class="col-md-5 mx-auto py-5">
            ${this.RenderChildren()}
            </div>
          </div>
        </div>
        <!-- content-wrapper ends -->
      </div>
      <!-- page-body-wrapper ends -->
    </div>
    ${this.Rendered.Scripts}
  </body>
</html>`
    }
}