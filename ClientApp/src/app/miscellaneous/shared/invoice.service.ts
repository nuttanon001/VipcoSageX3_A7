import { Injectable } from '@angular/core';
import { BaseRestService } from 'src/app/shared/base-rest.service';
import { InvoiceSupBp } from './invoice-sup-bp.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { HttpErrorHandler } from 'src/app/shared/http-error-handler.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { downloadFile } from 'src/app/payments/shared/payment.service';
import { Scroll } from "../../shared/scroll.model";

@Injectable({
  providedIn: 'root'
})
export class InvoiceService extends BaseRestService<InvoiceSupBp> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/Invoice/",
      "InvoiceService",
      "DocumentNo",
      httpErrorHandler
    )
  }

  getXlsx(scroll: Scroll, subAction: string = "GetReport/"): Observable<any> {
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
