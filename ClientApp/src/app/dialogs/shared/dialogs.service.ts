// Angular Core
import { MatDialogRef, MatDialog, MatDialogConfig } from "@angular/material";
import { Injectable, ViewContainerRef } from "@angular/core";
// rxjs
import { Observable } from "rxjs";
// module
import { Employee } from "../../employees/shared/employee.model";
import { EmployeeGroupMis } from "../../employees/shared/employee-group-mis.model";
// components
import { ErrorDialog } from "../error-dialog/error-dialog.component";
import { ConfirmDialog } from "../confirm-dialog/confirm-dialog.component";
import { ContextDialog } from "../context-dialog/context-dialog.component";
import { EmployeeGroup } from "../../employees/shared/employee-group.model";
import { BomLevel } from "../../dimension-datas/shared/bom-level.model";
import { BomDialogComponent } from "../bom-dialog/bom-dialog.component";
import { ProjectCode } from "../../dimension-datas/shared/project-code.model";
import { JobDialogComponent } from "../job-dialog/job-dialog.component";
import { Workgroup } from "../../dimension-datas/shared/workgroup.model";
import { GroupDialogComponent } from "../group-dialog/group-dialog.component";
import { Bank } from "../../dimension-datas/shared/bank.model";
import { BankDialogComponent } from "../bank-dialog/bank-dialog.component";
import { Supplier } from "../../dimension-datas/shared/supplier.model";
import { SupplierDialogComponent } from "../supplier-dialog/supplier-dialog.component";
import { CategoryDialogComponent } from "../category-dialog/category-dialog.component";
import { Category } from "../../dimension-datas/shared/category.model";

@Injectable()
export class DialogsService {
  // width and height > width and height in scss master-dialog
  width: string = "80vh";
  height: string = "80vw";

  constructor(private dialog: MatDialog) { }

  public confirm(title: string, message: string, viewContainerRef: ViewContainerRef): Observable<boolean> {

    let dialogRef: MatDialogRef<ConfirmDialog>;
    let config: MatDialogConfig = new MatDialogConfig();
    config.viewContainerRef = viewContainerRef;

    dialogRef = this.dialog.open(ConfirmDialog, config);

    dialogRef.componentInstance.title = title;
    dialogRef.componentInstance.message = message;

    return dialogRef.afterClosed();
  }

  public context(title: string, message: string, viewContainerRef: ViewContainerRef): Observable<boolean> {

    let dialogRef: MatDialogRef<ContextDialog>;
    let config: MatDialogConfig = new MatDialogConfig();
    config.viewContainerRef = viewContainerRef;

    dialogRef = this.dialog.open(ContextDialog, config);

    dialogRef.componentInstance.title = title;
    dialogRef.componentInstance.message = message;

    return dialogRef.afterClosed();
  }

  public error(title: string, message: string, viewContainerRef: ViewContainerRef): Observable<boolean> {

    let dialogRef: MatDialogRef<ErrorDialog>;
    let config: MatDialogConfig = new MatDialogConfig();
    config.viewContainerRef = viewContainerRef;

    dialogRef = this.dialog.open(ErrorDialog, config);

    dialogRef.componentInstance.title = title;
    dialogRef.componentInstance.message = message;

    return dialogRef.afterClosed();
  }

  /**
  * Bom
  * @param viewContainerRef
  * @param type = mode 0:fastSelected
  */
  public dialogSelectBomLevel(viewContainerRef: ViewContainerRef, type: number = 0): Observable<BomLevel> {
    let dialogRef: MatDialogRef<BomDialogComponent>;
    let config: MatDialogConfig = new MatDialogConfig();

    // config
    config.viewContainerRef = viewContainerRef;
    config.data = type;
    // config.height = this.height;
    // config.width= this.width;
    config.hasBackdrop = true;

    // open dialog
    dialogRef = this.dialog.open(BomDialogComponent, config);
    return dialogRef.afterClosed();
  }

  /**
* Project
* @param viewContainerRef
* @param type = mode 0:fastSelected
*/
  public dialogSelectProject(viewContainerRef: ViewContainerRef, type: number = 0): Observable<ProjectCode> {
    let dialogRef: MatDialogRef<JobDialogComponent>;
    let config: MatDialogConfig = new MatDialogConfig();

    // config
    config.viewContainerRef = viewContainerRef;
    config.data = type;
    // config.height = this.height;
    // config.width= this.width;
    config.hasBackdrop = true;

    // open dialog
    dialogRef = this.dialog.open(JobDialogComponent, config);
    return dialogRef.afterClosed();
  }

  /**
* WorkGroup
* @param viewContainerRef
* @param type = mode 0:fastSelected
*/
  public dialogSelectGroup(viewContainerRef: ViewContainerRef, type: number = 0): Observable<Workgroup> {
    let dialogRef: MatDialogRef<GroupDialogComponent>;
    let config: MatDialogConfig = new MatDialogConfig();

    // config
    config.viewContainerRef = viewContainerRef;
    config.data = type;
    // config.height = this.height;
    // config.width= this.width;
    config.hasBackdrop = true;

    // open dialog
    dialogRef = this.dialog.open(GroupDialogComponent, config);
    return dialogRef.afterClosed();
  }

  /**
* Bank
* @param viewContainerRef
* @param type = mode 0:fastSelected
*/
  public dialogSelectBank(viewContainerRef: ViewContainerRef, type: number = 0): Observable<Bank|Array<Bank>> {
    let dialogRef: MatDialogRef<BankDialogComponent>;
    let config: MatDialogConfig = new MatDialogConfig();

    // config
    config.viewContainerRef = viewContainerRef;
    config.data = type;
    // config.height = this.height;
    // config.width= this.width;
    config.hasBackdrop = true;

    // open dialog
    dialogRef = this.dialog.open(BankDialogComponent, config);
    return dialogRef.afterClosed();
  }

  /**
* Bank
* @param viewContainerRef
* @param type = mode 0:fastSelected
*/
  public dialogSelectCategory(viewContainerRef: ViewContainerRef, type: number = 0): Observable<Category | Array<Category>> {
    let dialogRef: MatDialogRef<CategoryDialogComponent>;
    let config: MatDialogConfig = new MatDialogConfig();

    // config
    config.viewContainerRef = viewContainerRef;
    config.data = type;
    // config.height = this.height;
    // config.width= this.width;
    config.hasBackdrop = true;

    // open dialog
    dialogRef = this.dialog.open(CategoryDialogComponent, config);
    return dialogRef.afterClosed();
  }

  /**
* Supplier
* @param viewContainerRef
* @param type = mode 0:fastSelected
*/
  public dialogSelectSupplier(viewContainerRef: ViewContainerRef, type: number = 0): Observable<Supplier> {
    let dialogRef: MatDialogRef<SupplierDialogComponent>;
    let config: MatDialogConfig = new MatDialogConfig();

    // config
    config.viewContainerRef = viewContainerRef;
    config.data = type;
    // config.height = this.height;
    // config.width= this.width;
    config.hasBackdrop = true;

    // open dialog
    dialogRef = this.dialog.open(SupplierDialogComponent, config);
    return dialogRef.afterClosed();
  }
}
