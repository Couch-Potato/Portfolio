import { Page } from "./Page.js";
import { Renderable } from "./Renderable.js";

class AuthPage extends Page {
    Scripts = [
        "/assets/vendors/js/vendor.bundle.base.js"
    ]
    CSS = [
        "/assets/css/shared/style.css",
        "/assets/vendors/css/vendor.bundle.base.css",
        "/assets/vendors/typicons/typicons.css",
        "/assets/vendors/ti-icons/css/themify-icons.css",
        "/assets/vendors/flag-icon-css/css/flag-icon.min.css",
        "/assets/vendors/mdi/css/materialdesignicons.min.css"
    ]
    RenderPage() {
        return `
<!DOCTYPE html>
<html lang="en">
  <head>
    <!-- Required meta tags -->
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <title>${this.PageTitle}</title>
    <!-- plugins:css -->
${this.Rendered.CSS}
    <!-- endinject -->
    <!-- Plugin css for this page -->
    <!-- End Plugin css for this page -->
    <!-- inject:css -->
    
    <!-- endinject -->
  </head>
  <body>
    <div class="container-scroller">
      <div class="container-fluid page-body-wrapper full-page-wrapper">
        <div class="content-wrapper auth p-0 theme-two">
          <div class="row d-flex align-items-stretch">
            <div class="col-md-4 banner-section d-none d-md-flex align-items-stretch justify-content-center">
              <div class="slide-content bg-1"> </div>
            </div>
            <div class="col-12 col-md-8 h-100 bg-white">
              <div class="auto-form-wrapper d-flex align-items-center justify-content-center flex-column">
                ${this.RenderChildren()}
              </div>
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

export class LoginForm extends Renderable {
    Render() {
        return `<form action="" method="POST">
                  <h3 class="mr-auto">Hello! let's get started</h3>
                  <p class="mb-5 mr-auto">Enter your details below.</p>
                  <div class="form-group">
                    <div class="input-group">
                      <div class="input-group-prepend">
                        <span class="input-group-text">
                          <i class="mdi mdi-account-outline"></i>
                        </span>
                      </div>
                      <input type="text" name="username" class="form-control" placeholder="Username">
                    </div>
                  </div>
                  <div class="form-group">
                    <div class="input-group">
                      <div class="input-group-prepend">
                        <span class="input-group-text">
                          <i class="mdi mdi-lock-outline"></i>
                        </span>
                      </div>
                      <input type="password" name="password" class="form-control" placeholder="Password">
                    </div>
                  </div>
                  <div class="form-group">
                    <button class="btn btn-primary submit-btn">SIGN IN</button>
                  </div>
                </form>`
    }
}

export class LoginPage extends AuthPage {
    Children = [
        new LoginForm()
    ];
    PageTitle = "Login"
}