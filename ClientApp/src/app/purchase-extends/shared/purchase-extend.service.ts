import { Injectable } from '@angular/core';
import { BaseRestService } from 'src/app/shared2/baseclases/base-rest.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { PurchaseExtend } from './purchase-extend.model';
import { HttpErrorHandler } from 'src/app/shared2/baseclases/http-error-handler.service';
import { Scroll } from 'src/app/shared/scroll.model';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})

export class PurchaseExtendService extends BaseRestService<PurchaseExtend> {

  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/PurchaseExtends/",
      "PurchaseExtendsService",
      "PurchaseExtendId",
      httpErrorHandler
    )
  }

  getXlsx(scroll: Scroll, subReport: string = "PrReportOnlyReceivedGetReport/"): Observable<any> {
    let url: string = this.baseUrl + subReport;
    return this.http.post(url, JSON.stringify(scroll), {
      headers: new HttpHeaders({
        "Content-Type": "application/json",
        'Accept': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
      }),
      responseType: 'blob' // <-- changed to blob 
    }).pipe(map(res => this.downloadFile(res, 'application/xlsx', 'export.xlsx')));
  }
}
