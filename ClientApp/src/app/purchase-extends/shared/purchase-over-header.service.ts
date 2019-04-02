import { Injectable } from '@angular/core';
import { BaseRestService } from 'src/app/shared2/baseclases/base-rest.service';
import { PurchaseOverHeader } from './purchase-over-header.model';
import { HttpClient } from '@angular/common/http';
import { HttpErrorHandler } from 'src/app/shared2/baseclases/http-error-handler.service';

@Injectable({
  providedIn: 'root'
})
export class PurchaseOverHeaderService extends BaseRestService<PurchaseOverHeader> {

  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/PurchaseOrderHeader/",
      "PurchaseOrderHeaderService",
      "PurchaseOrderHeaderId",
      httpErrorHandler
    )
  }
}
