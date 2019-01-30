import { NgModule } from "@angular/core";
import { HttpModule } from "@angular/http";
import { RouterModule } from "@angular/router";
import { CommonModule } from "@angular/common";
import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { BrowserModule } from "@angular/platform-browser";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
// Components
import { AppComponent } from './core/app/app.component';
import { HomeComponent } from "./core/home/home.component";
import { NavMenuComponent } from "./core/nav-menu/nav-menu.component";
import { DialogsModule } from "./dialogs/dialog.module";
import { CustomMaterialModule } from "./shared/customer-material.module";
import { LoginComponent } from "./users/login/login.component";
import { RegisterComponent } from "./users/register/register.component";
// Services
import { AuthGuard } from "./core/auth/auth-guard.service";
import { AuthService } from "./core/auth/auth.service";
import { MessageService } from "./shared/message.service";
import { HttpErrorHandler } from "./shared/http-error-handler.service";
import { JwtInterceptorService } from './core/auth/jwt-interceptor.service';
import { ErrorInterceptorService } from './core/auth/error-interceptor.service';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    NavMenuComponent,
    LoginComponent,
    RegisterComponent,
  ],
  imports: [
    // Angular Core
    HttpModule,
    FormsModule,
    CommonModule,
    HttpClientModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    // Modules
    DialogsModule,
    CustomMaterialModule,
    // Router
    RouterModule.forRoot([
      { path: "", redirectTo: "home", pathMatch: "full" },
      { path: "home", component: HomeComponent },
      { path: "login", component: LoginComponent },
      { path: "register/:condition", component: RegisterComponent },
      { path: "register", component: RegisterComponent },
      {
        path: "stock-onhand",
        loadChildren: "./stock-onhands/stock-onhand.module#StockOnhandModule",
      },
      {
        path: "purchase-order",
        loadChildren: "./purchase-orders/purchase-order.module#PurchaseOrderModule",
        canActivate: [AuthGuard]
      },
      {
        path: "purchase-request",
        loadChildren: "./purchase-requests/pr.module#PrModule",
      },
      {
        path: "stock-movement",
        loadChildren: "./stock-movements/stock-movement.module#StockMovementModule",
      },
      {
        path: "payment",
        loadChildren: "./payments/payment.module#PaymentModule",
        canActivate: [AuthGuard]
      },
      {
        path: "task-status",
        loadChildren: "./task-status/task-status.module#TaskStatusModule",
        canActivate: [AuthGuard]
      },
      {
        path: "mics-account",
        loadChildren: "./miscellaneous/misc.module#MiscModule",
        canActivate: [AuthGuard]
      },
      { path: "**", redirectTo: "home" },
    ]),
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptorService, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptorService, multi: true },
    AuthGuard,
    AuthService,
    MessageService,
    HttpErrorHandler
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
