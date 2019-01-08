import { Component, OnInit, Inject, ViewChild, OnDestroy } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material";
// models
import { Scroll } from "./scroll.model";
// rxjs
import { Observable, Subscription } from "rxjs";
import { BaseModel } from "./base-model.model";
// services
import { BaseRestService } from "./base-rest.service";
import { BaseCommunicateService } from "./base-communicate.service";
import { AuthService } from "../core/auth/auth.service";

/** base-dialog component*/
export abstract class BaseMasterDialogComponent
  <Model extends BaseModel,
  Service extends BaseRestService<Model>,
  CommunicateService extends BaseCommunicateService<Model>>
  implements OnInit, OnDestroy {
  /*
   * constructor
   */
  constructor(
    protected service: Service,
    protected communicateService: CommunicateService,
    protected authService: AuthService,
    protected dialogRef: MatDialogRef<any>,
  ) { }
  /*
   * Parameter
   */
  ////////////
  // Dialog //
  ////////////
  getValue: Model;
  getValues: Array<Model>;
  fastSelectd: boolean = false;
  ////////////
  // Master //
  ////////////
  displayValue: Model | undefined;
  subscription: Subscription;
  // boolean event
  _showDetail: boolean;
  onLoading: boolean;
  canSave: boolean;
  /*
   * Property
   */
  get DisplayDataNull(): boolean {
    return this.displayValue === undefined;
  }
  get ShowDetail(): boolean {
    if (this._showDetail) {
      return this._showDetail;
    } else {
      return false;
    }
  }
  set ShowDetail(showEdit: boolean) {
    if (showEdit !== this._showDetail) {
      this._showDetail = showEdit;
    }
  }
  /*
   * Method
   * /
  /** Called by Angular after cutting-plan-dialog component initialized */
  ngOnInit(): void {
    this.onInit();

    this.ShowDetail = false;
    this.onLoading = false;
    this.canSave = false;
    // Subsciption
    this.subscription = this.communicateService.ToParent$.subscribe(
      (communicateDate: Model) => {
        this.displayValue = communicateDate;
        this.canSave = communicateDate !== undefined && this.ShowDetail;
      });
  }
  // angular hook
  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe;
    }
  }
  // on Init data
  abstract onInit(): void;
  // Selected Value
  onSelectedValue(value?: Model): void {
    // console.log("OnSelectedValue", JSON.stringify(value));
    if (value) {
      this.getValue = value;
      if (this.fastSelectd) {
        this.onSelectedClick();
      }
    }
  }
  // Selected Values
  onSelectedValues(values?: Array<Model>): void {
    if (values) {
      this.getValues = new Array;
      this.getValues = [...values];
    }
  }
  // No Click
  onCancelClick(): void {
    this.dialogRef.close();
  }
  // Update Click
  onSelectedClick(): void {
    if (this.getValue) {
      this.dialogRef.close(this.getValue);
    } else if (this.getValues) {
      this.dialogRef.close(this.getValues);
    }
  }
  // on submit
  onSubmit(): void {
    this.canSave = false;
    if (this.displayValue.CreateDate) {
      this.onUpdateToDataBase(this.displayValue);
    } else {
      this.onInsertToDataBase(this.displayValue);
    }
  }
  // on detail view
  onDetailView(value?: { data: Model, option: number }): void {
    if (value) {
      if (value.option === 1) {
        this.onLoading = true;
        this.displayValue = value.data;
        this.ShowDetail = true;
        setTimeout(() => {
          this.communicateService.toChildEdit(this.displayValue);
          this.onLoading = false;
        }, 1000);
      } 
    } else {
      this.displayValue = undefined;
      this.ShowDetail = true;
      setTimeout(() => this.communicateService.toChildEdit(this.displayValue), 1000);
    }
  }
  // on insert data
  onInsertToDataBase(value: Model): void {
    if (this.authService.getAuth) {
      value["Creator"] = this.authService.getAuth.UserName || "";
    }
    // insert data
    this.service.addModel(value).subscribe(
      (complete: any) => {
        if (complete) {
          this.getValue = complete;
          this.onSelectedClick();
        }
        if (this.onLoading) {
          this.onLoading = false;
        }
      });
  }
  // on update data
  onUpdateToDataBase(value: Model): void {
    if (this.authService.getAuth) {
      value["Modifyer"] = this.authService.getAuth.UserName || "";
    }
    // update data
    this.service.updateModelWithKey(value).subscribe(
      (complete: any) => {
        if (complete) {
          this.getValue = complete;
          this.onSelectedClick();
        }
        if (this.onLoading) {
          this.onLoading = false;
        }
      });
  }
}
