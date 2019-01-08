import { Injectable } from '@angular/core';
import { BaseRestService } from '../../shared/base-rest.service';
import { PurchaseOrder } from './purchase-order.model';
import { HttpClient } from '@angular/common/http';
import { HttpErrorHandler } from '../../shared/http-error-handler.service';

@Injectable()
export class PurchaseOrderService extends BaseRestService<PurchaseOrder> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/PurchaseOrder/",
      "PurchaseOrderService",
      "Rowid",
      httpErrorHandler
    )
  }
}
