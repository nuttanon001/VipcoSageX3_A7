import { Component, OnInit } from '@angular/core';
import { BaseTableMK2Component } from '../../../shared/base-tablemk2.component';
import { ProjectCode } from '../../../dimension-datas/shared/project-code.model';
import { ProjectCodeService } from '../../../dimension-datas/shared/project-code.service';

@Component({
  selector: 'app-job-table-dialog',
  templateUrl: './job-table-dialog.component.html',
  styleUrls: ['./job-table-dialog.component.scss']
})
export class JobTableDialogComponent extends BaseTableMK2Component<ProjectCode, ProjectCodeService> {

  constructor(
    service: ProjectCodeService
  ) {
    super(service);
    this.columns = [
      { columnName: "", columnField: "select", cell: undefined },
      { columnName: "Job Code", columnField: "ProjectCode", cell: (row: ProjectCode) => row.ProjectCode },
      { columnName: "Job Name", columnField: "ProjectName", cell: (row: ProjectCode) => row.ProjectName },
    ];

    this.displayedColumns = this.columns.map(x => x.columnField);
    this.isDialog = true;
  }
}
