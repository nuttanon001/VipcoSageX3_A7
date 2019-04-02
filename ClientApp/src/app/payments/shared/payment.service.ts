import { Injectable } from '@angular/core';
import { BaseRestService } from '../../shared/base-rest.service';
import { HttpErrorHandler } from '../../shared/http-error-handler.service';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Payment } from './payment.model';
import { Observable } from 'rxjs';
import { Scroll } from '../../shared/scroll.model';
import { map } from "rxjs/operators"

@Injectable()
export class PaymentService extends BaseRestService<Payment> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/Payment/",
      "PaymentService",
      "PaymentNo",
      httpErrorHandler
    )
  }

  // get Task Machine Number
  getPaymentReport(scroll: Scroll): Observable<any> {
    let url: string = this.baseUrl + "GetReport/";

    // return this.http.post(url, JSON.stringify(scroll),
    //   {
    //     headers: new HttpHeaders({
    //       "Content-Type": "application/json",
    //       'Accept': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    //     }),
    //     responseType: 'blob'
    //   }
    // );
    var headers = new Headers();
    headers.append('responseType', 'arraybuffer');

    return this.http.post(url, JSON.stringify(scroll),
      {
        withCredentials: true,
        responseType: "blob"
      });

    // return this.http.get<User[]>(`${config.apiUrl}/users`);
  }

  getXlsx(scroll: Scroll, subReport: string = "GetReport/"): Observable<any> {
    let url: string = this.baseUrl + subReport;

    return this.http.post(url, JSON.stringify(scroll), {
      headers: new HttpHeaders({
        "Content-Type": "application/json",
        'Accept': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
      }),
      responseType: 'blob' // <-- changed to blob 
    }).pipe(map(res => downloadFile(res, 'application/xlsx', 'export.xlsx')));
  }
}

export function downloadFile(blob: any, type: string, filename: string): string {
  const url = window.URL.createObjectURL(blob); // <-- work with blob directly

  // create hidden dom element (so it works in all browsers)
  const a = document.createElement('a');
  a.setAttribute('style', 'display:none;');
  document.body.appendChild(a);

  // create file, attach to hidden element and open hidden element
  a.href = url;
  a.download = filename;
  a.click();
  return url;
}
