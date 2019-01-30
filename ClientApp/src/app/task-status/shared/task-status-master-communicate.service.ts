import { Injectable } from '@angular/core';
import { BaseCommunicateService } from 'src/app/shared2/baseclases/base-communicate.service';
import { TaskStatusMaster } from './task-status-master.model';

@Injectable()
export class TaskStatusMasterCommunicateService extends BaseCommunicateService<TaskStatusMaster> {
  constructor() { super(); }
}
