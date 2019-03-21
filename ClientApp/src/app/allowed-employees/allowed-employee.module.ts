// Angular Core
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
// Modules
import { SharedModule } from '../shared2/shared.module';
import { CustomMaterialModule } from '../shared2/customer-material.module';
import { AllowedEmployeeRoutingModule } from './allowed-employee-routing.module';
// Components
import { AllowedEmployeeCenterComponent } from './allowed-employee-center.component';
import { AllowedEmployeeMasterComponent } from './allowed-employee-master/allowed-employee-master.component';
import { AllowedEmployeeTableComponent } from './allowed-employee-table/allowed-employee-table.component';
import { AllowedEmployeeInfoComponent } from './allowed-employee-info/allowed-employee-info.component';

@NgModule({
  declarations: [
    AllowedEmployeeCenterComponent,
    AllowedEmployeeMasterComponent,
    AllowedEmployeeTableComponent,
    AllowedEmployeeInfoComponent
  ],
  imports: [
    FormsModule,
    CommonModule,
    SharedModule,
    ReactiveFormsModule,
    CustomMaterialModule,
    AllowedEmployeeRoutingModule
  ]
})
export class AllowedEmployeeModule { }
