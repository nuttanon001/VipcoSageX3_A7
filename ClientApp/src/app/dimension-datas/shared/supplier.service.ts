import { Injectable } from '@angular/core';
import { BaseRestService } from '../../shared/base-rest.service';
import { Supplier } from './supplier.model';
import { HttpClient } from '@angular/common/http';
import { HttpErrorHandler } from '../../shared/http-error-handler.service';

@Injectable()
export class SupplierService extends BaseRestService<Supplier> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/Supplier/",
      "SupplierService",
      "Rowid",
      httpErrorHandler
    )
  }
}
