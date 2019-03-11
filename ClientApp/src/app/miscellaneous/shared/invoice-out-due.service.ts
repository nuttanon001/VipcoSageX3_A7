import { Injectable } from '@angular/core';
import { InvoiceOutDue } from './invoice-out-due.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Scroll } from 'src/app/shared2/basemode/scroll.model';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { BaseRestService } from 'src/app/shared/base-rest.service';
import { HttpErrorHandler } from 'src/app/shared/http-error-handler.service';
import { downloadFile } from 'src/app/payments/shared/payment.service';

@Injectable({
  providedIn: 'root'
})
export class InvoiceOutDueService extends BaseRestService<InvoiceOutDue> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/Invoice/",
      "InvoiceOutDueService",
      "InvoiceNo",
      httpErrorHandler
    )
  }

  getXlsx(scroll: Scroll, subAction: string = "InvoiceOutStandingGetReport/"): Observable<any> {
    let url: string = this.baseUrl + subAction;

    return this.http.post(url, JSON.stringify(scroll), {
      headers: new HttpHeaders({
        "Content-Type": "application/json",
        'Accept': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
      }),
      responseType: 'blob' // <-- changed to blob 
    }).pipe(map(res => downloadFile(res, 'application/xlsx', 'export.xlsx')));
  }
}
