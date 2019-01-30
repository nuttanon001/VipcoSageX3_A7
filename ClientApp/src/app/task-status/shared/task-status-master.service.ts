import { Injectable } from '@angular/core';
import { BaseRestService } from 'src/app/shared2/baseclases/base-rest.service';
import { HttpClient } from '@angular/common/http';
import { HttpErrorHandler } from 'src/app/shared2/baseclases/http-error-handler.service';
import { TaskStatusMaster } from './task-status-master.model';

@Injectable({
  providedIn: 'root'
})
export class TaskStatusMasterService extends BaseRestService<TaskStatusMaster> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/TaskStatusMaster/",
      "TaskStatusMasterService",
      "TaskStatusMasterId",
      httpErrorHandler
    )
  }
}
