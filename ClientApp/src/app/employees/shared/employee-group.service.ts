import { Injectable } from '@angular/core';
import { BaseRestService } from '../../shared/base-rest.service';
import { EmployeeGroup } from './employee-group.model';
import { HttpClient, HttpParams } from '@angular/common/http';
import { HttpErrorHandler } from '../../shared/http-error-handler.service';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class EmployeeGroupService extends BaseRestService <EmployeeGroup> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/version2/EmployeeGroup/",
      "EmployeeGroupMisService",
      "GroupMIS",
      httpErrorHandler
    )
  }

  // ===================== EmployeeGroup ===========================\\
  // action require maintenance
  getGroupByEmpCode(EmpCode: string): Observable<EmployeeGroup> {
    return this.http.get<EmployeeGroup>(this.baseUrl + "GroupByEmpCode/", {
      params: new HttpParams().set("EmpCode", EmpCode.toString())
    }).pipe(catchError(this.handleError(this.serviceName + "/get employee group mis by empcode", <EmployeeGroup>{})));
  }
}
