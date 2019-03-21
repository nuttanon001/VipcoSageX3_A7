import { Component, OnInit, Inject } from '@angular/core';
import { BaseDialogComponent } from 'src/app/shared/base-dialog.component';
import { Branch } from 'src/app/dimension-datas/shared/branch.model';
import { BankService } from 'src/app/dimension-datas/shared/bank.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-branch-dialog',
  templateUrl: './branch-dialog.component.html',
  styleUrls: ['./branch-dialog.component.scss'],
  providers: [BankService]
})
export class BranchDialogComponent extends BaseDialogComponent<Branch, BankService> {
  /** employee-dialog ctor */
  constructor(
    public service: BankService,
    public dialogRef: MatDialogRef<BranchDialogComponent>,
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
