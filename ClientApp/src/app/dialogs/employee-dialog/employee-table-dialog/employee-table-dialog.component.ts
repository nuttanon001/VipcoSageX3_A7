import { Component, OnInit } from '@angular/core';
import { BaseTableComponent } from 'src/app/shared2/baseclases/base-table.component';
import { Employee } from 'src/app/employees/shared/employee.model';
import { EmployeeService } from 'src/app/employees/shared/employee.service';
import { AuthService } from 'src/app/core/auth/auth.service';

@Component({
  selector: 'app-employee-table-dialog',
  templateUrl: './employee-table-dialog.component.html',
  styleUrls: ['./employee-table-dialog.component.scss']
})
export class EmployeeTableDialogComponent extends BaseTableComponent<Employee, EmployeeService>{

  constructor(service: EmployeeService,servierAuth:AuthService) {
    super(service, servierAuth);

    this.columns = [
      { columnName: "Code.", columnField: "EmpCode", cell: (row: Employee) => row.EmpCode },
      { columnName: "Name.", columnField: "NameThai", cell: (row: Employee) => row.NameThai },
      { columnName: "WorkGroup.", columnField: "GroupName", cell: (row: Employee) => row.GroupName },
    ];
    this.displayedColumns = this.columns.map(x => x.columnField);
    this.displayedColumns.splice(0, 0, "select");
    //this.displayedColumns.splice(0, 0, "Command");

    this.isKeyIndex = "EmpCode";
  }

}
