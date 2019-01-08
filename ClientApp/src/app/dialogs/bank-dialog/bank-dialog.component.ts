import { Component, OnInit, Inject } from '@angular/core';
import { Bank } from '../../dimension-datas/shared/bank.model';
import { BankService } from '../../dimension-datas/shared/bank.service';
import { BaseDialogComponent } from '../../shared/base-dialog.component';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-bank-dialog',
  templateUrl: './bank-dialog.component.html',
  styleUrls: ['./bank-dialog.component.scss'],
  providers: [BankService]
})
export class BankDialogComponent extends BaseDialogComponent<Bank, BankService> {
  /** employee-dialog ctor */
  constructor(
    public service: BankService,
    public dialogRef: MatDialogRef<BankDialogComponent>,
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
