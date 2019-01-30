import { Component, OnInit, ViewContainerRef, ViewChild } from '@angular/core';
import { BaseMasterComponent } from 'src/app/shared2/baseclases/base-master-component';
import { TaskStatusMaster } from '../shared/task-status-master.model';
import { TaskStatusMasterService } from '../shared/task-status-master.service';
import { TaskStatusMasterCommunicateService } from '../shared/task-status-master-communicate.service';
import { DialogsService } from 'src/app/dialogs/shared/dialogs.service';
import { AuthService } from 'src/app/core/auth/auth.service';
import { TaskStatusTableComponent } from '../task-status-table/task-status-table.component';

@Component({
  selector: 'app-task-status-master',
  templateUrl: './task-status-master.component.html',
  styleUrls: ['./task-status-master.component.scss'],
  providers: [TaskStatusMasterCommunicateService]
})
export class TaskStatusMasterComponent extends BaseMasterComponent<TaskStatusMaster, TaskStatusMasterService, TaskStatusMasterCommunicateService> {

  constructor(
    service: TaskStatusMasterService,
    serviceCom: TaskStatusMasterCommunicateService,
    serviceAuth:AuthService,
    serviceDialog: DialogsService,
    viewCon: ViewContainerRef,
  ) {
    super(service, serviceCom, serviceAuth, serviceDialog, viewCon);
  }

  backToSchedule: boolean = false;
  @ViewChild(TaskStatusTableComponent)
  private tableComponent: TaskStatusTableComponent;

  onReloadData(): void {
    this.tableComponent.reloadData();
  }

}
