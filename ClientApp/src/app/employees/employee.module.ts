import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
// Modules
import { CustomMaterialModule } from '../shared/customer-material.module';
// Services
import { EmployeeService } from './shared/employee.service';
import { EmployeeGroupMisService } from './shared/employee-group-mis.service';
// Components
import { EmployeeGroupService } from './shared/employee-group.service';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    CustomMaterialModule,
  ],
  declarations: [],
  providers: [
    EmployeeGroupService,
    EmployeeGroupMisService,
  ],
})
export class EmployeeModule { }
