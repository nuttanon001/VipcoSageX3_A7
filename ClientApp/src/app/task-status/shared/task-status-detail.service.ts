import { Injectable } from '@angular/core';
import { BaseRestService } from 'src/app/shared2/baseclases/base-rest.service';
import { HttpClient } from '@angular/common/http';
import { HttpErrorHandler } from 'src/app/shared2/baseclases/http-error-handler.service';
import { TaskStatusDetail } from './task-status-detail.model';

@Injectable({
  providedIn: 'root'
})
export class TaskStatusDetailService extends BaseRestService<TaskStatusDetail> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/TaskStatusDetail/",
      "TaskStatusDetailService",
      "TaskStatusDetailId",
      httpErrorHandler
    )
  }
}
