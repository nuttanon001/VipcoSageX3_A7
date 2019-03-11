import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Customer } from './customer.model';
import { BaseRestService } from 'src/app/shared/base-rest.service';
import { HttpErrorHandler } from 'src/app/shared/http-error-handler.service';
import { Scroll } from 'src/app/shared/scroll.model';
import { ScrollData } from 'src/app/shared/scroll-data.model';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class CustomerService extends BaseRestService<Customer> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/Bank/",
      "CustomerService",
      "Rowid",
      httpErrorHandler
    )
  }

  /** get all with scroll data */
  getAllWithScroll(scroll: Scroll, subAction: string = "GetCustomerScroll/"): Observable<any | ScrollData<Customer>> {
    // console.log(this.baseUrl + subAction);

    return this.http.post<ScrollData<Customer>>(this.baseUrl + subAction, JSON.stringify(scroll),
      {
        headers: new HttpHeaders({
          "Content-Type": "application/json",
        }),
      })
      .shareReplay()
      .pipe(catchError(this.handleError("Get models for scroll component", <ScrollData<Customer>>{})));
  }
}
