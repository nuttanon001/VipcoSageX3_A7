import { Component, OnInit, Inject } from '@angular/core';
import { WorkgroupService } from '../../dimension-datas/shared/workgroup.service';
import { Workgroup } from '../../dimension-datas/shared/workgroup.model';
import { BaseDialogComponent } from '../../shared/base-dialog.component';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-group-dialog',
  templateUrl: './group-dialog.component.html',
  styleUrls: ['./group-dialog.component.scss'],
  providers: [WorkgroupService]
})
export class GroupDialogComponent extends BaseDialogComponent<Workgroup, WorkgroupService> {
  /** employee-dialog ctor */
  constructor(
    public service: WorkgroupService,
    public dialogRef: MatDialogRef<GroupDialogComponent>,
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
