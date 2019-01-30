import { Component, OnInit } from '@angular/core';
import { BaseTableComponent } from 'src/app/shared2/baseclases/base-table.component';
import * as moment from "moment";
import { TaskStatusMaster } from '../shared/task-status-master.model';
import { TaskStatusMasterService } from '../shared/task-status-master.service';
import { AuthService } from 'src/app/core/auth/auth.service';

@Component({
  selector: 'app-task-status-table',
  templateUrl: './task-status-table.component.html',
  styleUrls: ['./task-status-table.component.scss']
})
export class TaskStatusTableComponent extends BaseTableComponent<TaskStatusMaster, TaskStatusMasterService> {

  constructor(
    service: TaskStatusMasterService,
    serviceAuth:AuthService
  ) {
    super(service,serviceAuth);

    this.columns = [
      { columnName: "WorkGroupCodeอ", columnField: "WorkGroupCode", cell: (row: TaskStatusMaster) => row.WorkGroupCode },
      { columnName: "WorkGroupName", columnField: "WorkGroupName", cell: (row: TaskStatusMaster) => row.WorkGroupName },
      { columnName: "CreateDate", columnField: "CreateDate", cell: (row: TaskStatusMaster) => moment(row.CreateDate).format("DD-MM-YYYY") },
    ];
    this.displayedColumns = this.columns.map(x => x.columnField);
    //this.displayedColumns.splice(0, 0, "select");
    this.displayedColumns.splice(0, 0, "Command");
  }

}
