import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
// service
import { HttpErrorHandler } from 'src/app/shared2/baseclases/http-error-handler.service';
// models
import { Employee } from "../../employees/shared/employee.model";
import { BaseRestService } from 'src/app/shared2/baseclases/base-rest.service';
// component

@Injectable({
   providedIn:"root"
})
export class EmployeeService extends BaseRestService<Employee> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "http://192.168.2.31/machinemk2/api/version2/Employee/",
      "EmployeeService",
      "EmpCode",
      httpErrorHandler
    )
  }
}
