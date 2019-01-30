import { Component, OnInit } from '@angular/core';
import { BaseTableDetailComponent } from 'src/app/shared2/baseclases/base-table-detail.component';
import { TaskStatusDetail } from '../shared/task-status-detail.model';

@Component({
  selector: 'app-task-status-detail-table',
  templateUrl: './task-status-detail-table.component.html',
  styleUrls: ['./task-status-detail-table.component.scss']
})
export class TaskStatusDetailTableComponent extends BaseTableDetailComponent<TaskStatusDetail>{

  constructor() {
    super();
    this.columns = [
      { columnName: "EmployeeCode", columnField: "EmployeeCode", cell: (row: TaskStatusDetail) => row.EmployeeCode },
      { columnName: "Name", columnField: "Name", cell: (row: TaskStatusDetail) => row.Name },
      { columnName: "Email", columnField: "Email", cell: (row: TaskStatusDetail) => row.Email, canEdit:true },
      { columnName: "Remark", columnField: "Remark", cell: (row: TaskStatusDetail) => row.Remark,canEdit:true },
    ];
    this.displayedColumns = this.columns.map(x => x.columnField);
    this.displayedColumns.splice(0, 0, "Command");
  }

  // on blur
  onBlurText(rowData?: TaskStatusDetail): void {
    //Debug here
    if (rowData) {
      this.returnSelectedWith.emit({
        data: rowData,
        option: 2
      });
    }
  }

}
