import { Component, OnInit, Inject } from '@angular/core';
import { ProjectCodeService } from '../../dimension-datas/shared/project-code.service';
import { ProjectCode } from '../../dimension-datas/shared/project-code.model';
import { BaseDialogComponent } from '../../shared/base-dialog.component';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-job-dialog',
  templateUrl: './job-dialog.component.html',
  styleUrls: ['./job-dialog.component.scss'],
  providers: [ProjectCodeService]
})
export class JobDialogComponent extends BaseDialogComponent<ProjectCode, ProjectCodeService> {
  /** employee-dialog ctor */
  constructor(
    public service: ProjectCodeService,
    public dialogRef: MatDialogRef<JobDialogComponent>,
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
