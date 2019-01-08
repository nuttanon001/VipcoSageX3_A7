import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
// Modules
import { EmployeeRoutingModule } from './employee-routing.module';
import { CustomMaterialModule } from '../shared/customer-material.module';
// Services
import { EmployeeService } from './shared/employee.service';
import { EmployeeCommuncateService } from './shared/employee-communcate.service';
import { EmployeeGroupMisService } from './shared/employee-group-mis.service';
// Components
import { EmployeeGroupService } from './shared/employee-group.service';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    CustomMaterialModule,
    EmployeeRoutingModule
  ],
  declarations: [],
  providers: [
    EmployeeService,
    EmployeeGroupMisService,
    EmployeeCommuncateService,
    EmployeeGroupService
  ],
})
export class EmployeeModule { }
