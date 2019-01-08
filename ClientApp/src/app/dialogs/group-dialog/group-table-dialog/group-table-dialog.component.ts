import { Component, OnInit } from '@angular/core';
import { BaseTableMK2Component } from '../../../shared/base-tablemk2.component';
import { Workgroup } from '../../../dimension-datas/shared/workgroup.model';
import { WorkgroupService } from '../../../dimension-datas/shared/workgroup.service';

@Component({
  selector: 'app-group-table-dialog',
  templateUrl: './group-table-dialog.component.html',
  styleUrls: ['./group-table-dialog.component.scss']
})
export class GroupTableDialogComponent extends BaseTableMK2Component<Workgroup, WorkgroupService> {

  constructor(
    service: WorkgroupService
  ) {
    super(service);
    this.columns = [
      { columnName: "", columnField: "select", cell: undefined },
      { columnName: "Group Code", columnField: "WorkGroupCode", cell: (row: Workgroup) => row.WorkGroupCode },
      { columnName: "Group Name", columnField: "WorkGroupName", cell: (row: Workgroup) => row.WorkGroupName },
    ];

    this.displayedColumns = this.columns.map(x => x.columnField);
    this.isDialog = true;
  }
}
