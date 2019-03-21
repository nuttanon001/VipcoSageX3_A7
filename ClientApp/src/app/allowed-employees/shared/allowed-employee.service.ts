import { Injectable } from '@angular/core';
import { AllowedEmployee } from './allowed-employee.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseRestService } from 'src/app/shared2/baseclases/base-rest.service';
import { HttpErrorHandler } from 'src/app/shared2/baseclases/http-error-handler.service';
import { Scroll } from 'src/app/shared2/basemode/scroll.model';
import { Observable } from 'rxjs';
import { ScrollData } from 'src/app/shared2/basemode/scroll-data.model';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AllowedEmployeeService extends BaseRestService<AllowedEmployee> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/Employee/",
      "AllowedEmployeeService",
      "EmpCode",
      httpErrorHandler
    )
  }

  /** add Model @param nObject */
  addModel(nObject: AllowedEmployee): Observable<any | AllowedEmployee> {
    // debug here
    // console.log("Path:", this.baseUrl);
    // console.log("Data is:", JSON.stringify(nObject));

    return this.http.post<AllowedEmployee>(this.baseUrl + "UpdateAllowedEmployee/", JSON.stringify(nObject), {
      headers: new HttpHeaders({ "Content-Type": "application/json" })
    }).pipe(catchError(this.handleError("Add model", <AllowedEmployee>{})));
  }

  updateModelWithKey(uObject: AllowedEmployee): Observable<any | AllowedEmployee> {
    return this.http.post<AllowedEmployee>(this.baseUrl, JSON.stringify(uObject), {
      headers: new HttpHeaders({ "Content-Type": "application/json" })
    }).pipe(catchError(this.handleError("Update model", <AllowedEmployee>{})));
  }
}
