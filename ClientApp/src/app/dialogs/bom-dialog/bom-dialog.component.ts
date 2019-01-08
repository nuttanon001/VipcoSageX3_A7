import { Component, OnInit, Inject } from '@angular/core';
import { BomLevel } from '../../dimension-datas/shared/bom-level.model';
import { BomLevelService } from '../../dimension-datas/shared/bom-level.service';
import { BaseDialogComponent } from '../../shared/base-dialog.component';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-bom-dialog',
  templateUrl: './bom-dialog.component.html',
  styleUrls: ['./bom-dialog.component.scss'],
  providers: [BomLevelService]
})
export class BomDialogComponent extends BaseDialogComponent<BomLevel, BomLevelService> {
  /** employee-dialog ctor */
  constructor(
    public service: BomLevelService,
    public dialogRef: MatDialogRef<BomDialogComponent>,
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
