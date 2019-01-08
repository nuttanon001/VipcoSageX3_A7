import { Injectable } from '@angular/core';
import { BaseRestService } from '../../shared/base-rest.service';
import { HttpClient } from '@angular/common/http';
import { HttpErrorHandler } from '../../shared/http-error-handler.service';
import { Bank } from './bank.model';

@Injectable()
export class BankService extends BaseRestService<Bank> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/Bank/",
      "BankService",
      "Rowid",
      httpErrorHandler
    )
  }
}
