import { Injectable } from '@angular/core';
import { Workgroup } from './workgroup.model';
import { BaseRestService } from '../../shared/base-rest.service';
import { HttpClient } from '@angular/common/http';
import { HttpErrorHandler } from '../../shared/http-error-handler.service';

@Injectable()
export class WorkgroupService extends BaseRestService<Workgroup> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/WorkGroup/",
      "WorkGroupService",
      "Rowid",
      httpErrorHandler
    )
  }
}
