import { Component, OnInit, Inject } from '@angular/core';
import { BaseDialogComponent } from 'src/app/shared/base-dialog.component';
import { Partner } from 'src/app/dimension-datas/shared/partner.model';
import { CustomerService } from 'src/app/dimension-datas/shared/customer.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-partner-dialog',
  templateUrl: './partner-dialog.component.html',
  styleUrls: ['./partner-dialog.component.scss']
})
export class PartnerDialogComponent extends BaseDialogComponent<Partner, CustomerService> {
  /** employee-dialog ctor */
  constructor(
    public service: CustomerService,
    public dialogRef: MatDialogRef<PartnerDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public mode: number
  ) {
    super(
      service,
      dialogRef
    );
  }
  // on init
  onInit(): void {
    this.fastSelectd = this.mode === 0 ? true : false;
  }

}
