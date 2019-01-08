import { Component, OnInit, Inject, ViewChild } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material";
// models
import { Scroll } from "./scroll.model";
// rxjs
import { Observable, Subscription } from "rxjs";
import { BaseModel } from "./base-model.model";
import { BaseRestService } from "./base-rest.service";
import { BaseCommunicateService } from "./base-communicate.service";
// 3rd party
// import { DatatableComponent, TableColumn } from "@swimlane/ngx-datatable";

/** base-dialog component*/
export abstract class BaseDialogViewComponent<Model extends BaseModel,CommunicateService extends BaseCommunicateService<Model>>
  implements OnInit {
  /** cutting-plan-dialog ctor */
  constructor(
    protected communicateService: CommunicateService,
    protected dialogRef: MatDialogRef<any>
  ) { }
  //@param
  onLoading: boolean = false;

  //@method
  /** Called by Angular after cutting-plan-dialog component initialized */
  ngOnInit(): void {
    this.onInit();
  }

  // on detail view
  onDetailView(infoValue?: Model): void {
    if (infoValue) {
      this.onLoading = true;
      setTimeout(() => {
        this.communicateService.toChildEdit(infoValue);
        this.onLoading = false;
      }, 1000);
    } 
  }

  // on Init data
  abstract onInit(): void;

  // No Click
  onCancelClick(): void {
    this.dialogRef.close();
  }

  // Update Click
  abstract onSelectedClick(): void;
}
