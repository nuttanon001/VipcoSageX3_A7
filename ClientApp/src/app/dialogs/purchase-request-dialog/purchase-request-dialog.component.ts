import { Component, OnInit, Inject } from '@angular/core';
import { BaseDialogEntryComponent } from 'src/app/shared2/baseclases/base-dialog-entry.component';
import { PurchaseRequestPure } from 'src/app/purchase-extends/shared/purchase-request-pure.model';
import { PurchaseExtendService } from 'src/app/purchase-extends/shared/purchase-extend.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DialogInfo } from 'src/app/shared2/basemode/dialog-info.model';

@Component({
  selector: 'app-purchase-request-dialog',
  templateUrl: './purchase-request-dialog.component.html',
  styleUrls: ['./purchase-request-dialog.component.scss'],
  providers: [PurchaseExtendService]
})
export class PurchaseRequestDialogComponent
  extends BaseDialogEntryComponent<PurchaseRequestPure, PurchaseExtendService> {
  /** employee-dialog ctor */
  constructor(
    service: PurchaseExtendService,
    public dialogRef: MatDialogRef<PurchaseRequestDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogInfo<PurchaseRequestPure>
  ) {
    super(
      service,
      dialogRef
    );
  }

  // on init
  onInit(): void {
    if (this.data) {
      this.fastSelectd = this.data.option;
    }
  }
}
