import { Injectable } from '@angular/core';
import { BaseRestService } from 'src/app/shared2/baseclases/base-rest.service';
import { HttpClient } from '@angular/common/http';
import { PurchaseExtend } from './purchase-extend.model';
import { HttpErrorHandler } from 'src/app/shared2/baseclases/http-error-handler.service';

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
}
