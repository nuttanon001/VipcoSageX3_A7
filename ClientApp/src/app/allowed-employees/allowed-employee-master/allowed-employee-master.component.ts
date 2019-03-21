import { Component, OnInit, ViewContainerRef, ViewChild } from '@angular/core';
import { BaseMasterComponent } from 'src/app/shared2/baseclases/base-master-component';
import { AllowedEmployee } from '../shared/allowed-employee.model';
import { AllowedEmployeeService } from '../shared/allowed-employee.service';
import { AllowedEmployeeCommunicateService } from '../shared/allowed-employee-communicate.service';
import { AuthService } from 'src/app/core/auth/auth.service';
import { DialogsService } from 'src/app/dialogs/shared/dialogs.service';
import { AllowedEmployeeTableComponent } from '../allowed-employee-table/allowed-employee-table.component';

@Component({
  selector: 'app-allowed-employee-master',
  templateUrl: './allowed-employee-master.component.html',
  styleUrls: ['./allowed-employee-master.component.scss'],
  providers: [AllowedEmployeeCommunicateService]
})
export class AllowedEmployeeMasterComponent
  extends BaseMasterComponent<AllowedEmployee, AllowedEmployeeService, AllowedEmployeeCommunicateService> {

  constructor(
    service: AllowedEmployeeService,
    serviceCom: AllowedEmployeeCommunicateService,
    serviceAuth: AuthService,
    serviceDialog: DialogsService,
    viewCon: ViewContainerRef,
  ) {
    super(service, serviceCom, serviceAuth, serviceDialog, viewCon);
  }

  backToSchedule: boolean = false;
  @ViewChild(AllowedEmployeeTableComponent)
  private tableComponent: AllowedEmployeeTableComponent;

  onReloadData(): void {
    this.tableComponent.reloadData();
  }

}
