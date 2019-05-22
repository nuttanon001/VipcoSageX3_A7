// angular core
import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
// 3rd party
import "rxjs/Rx";
import "hammerjs";
// services
import { DialogsService } from "./shared/dialogs.service";
// modules
import { CustomMaterialModule } from "../shared/customer-material.module";
import { CustomMaterialModule as Alias } from "../shared2/customer-material.module";

import { SharedModule } from "../shared/shared.module";
import { SharedModule as Alias2 } from "../shared2/shared.module";

// components
import { ErrorDialog } from "./error-dialog/error-dialog.component";
import { ContextDialog } from "./context-dialog/context-dialog.component";
import { ConfirmDialog } from "./confirm-dialog/confirm-dialog.component";
import { BomDialogComponent } from './bom-dialog/bom-dialog.component';
import { BomTableDialogComponent } from './bom-dialog/bom-table-dialog/bom-table-dialog.component';
import { JobDialogComponent } from './job-dialog/job-dialog.component';
import { JobTableDialogComponent } from './job-dialog/job-table-dialog/job-table-dialog.component';
import { GroupDialogComponent } from './group-dialog/group-dialog.component';
import { GroupTableDialogComponent } from './group-dialog/group-table-dialog/group-table-dialog.component';
import { SupplierDialogComponent } from './supplier-dialog/supplier-dialog.component';
import { BankDialogComponent } from './bank-dialog/bank-dialog.component';
import { SupplierTableDailogComponent } from './supplier-dialog/supplier-table-dailog/supplier-table-dailog.component';
import { BankTableDialogComponent } from './bank-dialog/bank-table-dialog/bank-table-dialog.component';
import { CategoryDialogComponent } from './category-dialog/category-dialog.component';
import { CategoryTableDialogComponent } from './category-dialog/category-table-dialog/category-table-dialog.component';
import { EmployeeDialogComponent } from './employee-dialog/employee-dialog.component';
import { EmployeeTableDialogComponent } from './employee-dialog/employee-table-dialog/employee-table-dialog.component';
import { CustomerDialogComponent } from './customer-dialog/customer-dialog.component';
import { CustomerTableDialogComponent } from './customer-dialog/customer-table-dialog/customer-table-dialog.component';
import { BranchDialogComponent } from './branch-dialog/branch-dialog.component';
import { BranchTableDialogComponent } from './branch-dialog/branch-table-dialog/branch-table-dialog.component';
import { PartnerDialogComponent } from './partner-dialog/partner-dialog.component';
import { PartnerTableDialogComponent } from './partner-dialog/partner-table-dialog/partner-table-dialog.component';
import { PurchaseRequestDialogComponent } from './purchase-request-dialog/purchase-request-dialog.component';
import { PurchaseRequestTableDialogComponent } from './purchase-request-dialog/purchase-request-table-dialog/purchase-request-table-dialog.component';
import { DatePickDialogComponent } from './date-pick-dialog/date-pick-dialog.component';

@NgModule({
  imports: [
    // angular
    FormsModule,
    CommonModule,
    ReactiveFormsModule,
    // customer Module
    SharedModule,
    CustomMaterialModule,
    Alias,
    Alias2
  ],
  declarations: [
    ErrorDialog,
    ConfirmDialog,
    ContextDialog,
    BomDialogComponent,
    BomTableDialogComponent,
    JobDialogComponent,
    JobTableDialogComponent,
    GroupDialogComponent,
    GroupTableDialogComponent,
    SupplierDialogComponent,
    BankDialogComponent,
    SupplierTableDailogComponent,
    BankTableDialogComponent,
    CategoryDialogComponent,
    CategoryTableDialogComponent,
    EmployeeDialogComponent,
    EmployeeTableDialogComponent,
    CustomerDialogComponent,
    CustomerTableDialogComponent,
    BranchDialogComponent,
    BranchTableDialogComponent,
    PartnerDialogComponent,
    PartnerTableDialogComponent,
    PurchaseRequestDialogComponent,
    PurchaseRequestTableDialogComponent,
    DatePickDialogComponent,
  ],
  providers: [
    DialogsService,
  ],
  // a list of components that are not referenced in a reachable component template.
  // doc url is :https://angular.io/guide/ngmodule-faq
  entryComponents: [
    ErrorDialog,
    ConfirmDialog,
    ContextDialog,
    BomDialogComponent,
    BomTableDialogComponent,
    JobDialogComponent,
    JobTableDialogComponent,
    GroupDialogComponent,
    GroupTableDialogComponent,
    SupplierDialogComponent,
    BankDialogComponent,
    SupplierTableDailogComponent,
    BankTableDialogComponent,
    CategoryDialogComponent,
    CategoryTableDialogComponent,
    EmployeeDialogComponent,
    EmployeeTableDialogComponent,
    CustomerDialogComponent,
    CustomerTableDialogComponent,
    BranchDialogComponent,
    BranchTableDialogComponent,
    PartnerDialogComponent,
    PartnerTableDialogComponent,
    PurchaseRequestDialogComponent,
    PurchaseRequestTableDialogComponent,
    DatePickDialogComponent,
  ],
})
export class DialogsModule { }
