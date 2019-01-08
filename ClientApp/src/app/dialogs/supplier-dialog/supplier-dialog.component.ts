import { Component, OnInit, Inject } from '@angular/core';
import { BaseDialogComponent } from '../../shared/base-dialog.component';
import { Supplier } from '../../dimension-datas/shared/supplier.model';
import { SupplierService } from '../../dimension-datas/shared/supplier.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-supplier-dialog',
  templateUrl: './supplier-dialog.component.html',
  styleUrls: ['./supplier-dialog.component.scss'],
  providers: [SupplierService]
})
export class SupplierDialogComponent extends BaseDialogComponent<Supplier, SupplierService> {
  /** employee-dialog ctor */
  constructor(
    public service: SupplierService,
    public dialogRef: MatDialogRef<SupplierDialogComponent>,
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
