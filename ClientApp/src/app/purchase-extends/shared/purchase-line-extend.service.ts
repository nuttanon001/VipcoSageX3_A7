import { Injectable } from '@angular/core';
import { BaseRestService } from 'src/app/shared2/baseclases/base-rest.service';
import { PurchaseLineExtend } from './purchase-line-extend.model';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { HttpErrorHandler } from 'src/app/shared2/baseclases/http-error-handler.service';
import { Observable } from 'rxjs';
import { PurchaseRequestLinePure } from './purchase-request-line-pure.model';
import { shareReplay, catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class PurchaseLineExtendService extends BaseRestService<PurchaseLineExtend> {

  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/PurchaseLineExtend/",
      "PurchaseLineExtendService",
      "PurchaseLineExtendId",
      httpErrorHandler
    )
  }

  //PurchaseReqLineExtendByPurchaseRequest
  getPurchaseReqLineExtendByPurchaseRequest(purchaseRequest: string): Observable<any|Array<PurchaseRequestLinePure>> {
    let url: string = this.baseUrl + "PurchaseReqLineExtendByPurchaseRequest/";
    return this.http.get<Array<PurchaseRequestLinePure>>(url, {
      params: new HttpParams().set("key", purchaseRequest),
    }).pipe(shareReplay(), catchError(this.handleError("Get model with key", <PurchaseRequestLinePure>{})));
  }
}
