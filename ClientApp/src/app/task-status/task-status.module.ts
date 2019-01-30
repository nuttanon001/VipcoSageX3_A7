import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TaskStatusRoutingModule } from './task-status-routing.module';
import { TaskStatusCenterComponent } from './task-status-center.component';
import { TaskStatusMasterComponent } from './task-status-master/task-status-master.component';
import { TaskStatusInfoComponent } from './task-status-info/task-status-info.component';
import { TaskStatusTableComponent } from './task-status-table/task-status-table.component';
import { TaskStatusDetailTableComponent } from './task-status-detail-table/task-status-detail-table.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CustomMaterialModule } from '../shared2/customer-material.module';
import { SharedModule } from '../shared2/shared.module';

@NgModule({
  declarations: [
    TaskStatusCenterComponent,
    TaskStatusMasterComponent,
    TaskStatusInfoComponent,
    TaskStatusTableComponent,
    TaskStatusDetailTableComponent],
  imports: [
    FormsModule,
    CommonModule,
    SharedModule,
    ReactiveFormsModule,
    CustomMaterialModule,
    TaskStatusRoutingModule
  ]
})
export class TaskStatusModule { }
