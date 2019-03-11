import { Injectable } from '@angular/core';
import { PrAndPo } from './pr-and-po.model';
import { BaseRestService } from '../../shared/base-rest.service';
import { HttpErrorHandler } from '../../shared/http-error-handler.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Scroll } from '../../shared/scroll.model';
import { Observable } from 'rxjs';
import { downloadFile } from '../../payments/shared/payment.service';
import { map } from 'rxjs/operators';

@Injectable()
export class PrService extends BaseRestService<PrAndPo> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/PurchaseRequest/",
      "PurchaseRequestService",
      "PrNumber",
      httpErrorHandler
    )
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
