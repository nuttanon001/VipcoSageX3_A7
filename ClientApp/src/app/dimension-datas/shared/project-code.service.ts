import { Injectable } from '@angular/core';
import { ProjectCode } from './project-code.model';
import { HttpClient } from '@angular/common/http';
import { HttpErrorHandler } from '../../shared/http-error-handler.service';
import { BaseRestService } from '../../shared/base-rest.service';

@Injectable()
export class ProjectCodeService extends BaseRestService<ProjectCode> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/Project/",
      "ProjectCodeService",
      "Rowid",
      httpErrorHandler
    )
  }
}
