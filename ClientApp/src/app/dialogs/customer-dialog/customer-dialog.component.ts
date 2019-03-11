import { Component, OnInit, Inject } from '@angular/core';
import { BaseDialogComponent } from 'src/app/shared/base-dialog.component';
import { Customer } from 'src/app/dimension-datas/shared/customer.model';
import { CustomerService } from 'src/app/dimension-datas/shared/customer.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-customer-dialog',
  templateUrl: './customer-dialog.component.html',
  styleUrls: ['./customer-dialog.component.scss']
})
export class CustomerDialogComponent extends BaseDialogComponent<Customer, CustomerService> {
  /** employee-dialog ctor */
  constructor(
    public service: CustomerService,
    public dialogRef: MatDialogRef<CustomerDialogComponent>,
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
