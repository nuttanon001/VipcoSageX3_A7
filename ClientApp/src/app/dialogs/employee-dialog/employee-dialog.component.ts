// Angular Core
import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
// Base Dialog
import { BaseDialogEntryComponent } from 'src/app/shared2/baseclases/base-dialog-entry.component';
// Services
import { EmployeeService } from 'src/app/employees/shared/employee.service';
// Models
import { Employee } from 'src/app/employees/shared/employee.model';
import { DialogInfo } from 'src/app/shared2/basemode/dialog-info.model';

@Component({
  selector: 'app-employee-dialog',
  templateUrl: './employee-dialog.component.html',
  styleUrls: ['./employee-dialog.component.scss'],
})
export class EmployeeDialogComponent
  extends BaseDialogEntryComponent<Employee, EmployeeService> {
  /** employee-dialog ctor */
  constructor(
    service: EmployeeService,
    public dialogRef: MatDialogRef<EmployeeDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogInfo<Employee>
  ) {
    super(
      service,
      dialogRef
    );
  }

  // on init
  onInit(): void {
    if (this.data) {
      this.fastSelectd = this.data.option;
    }
  }

}
