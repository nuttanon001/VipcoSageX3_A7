import { Injectable } from '@angular/core';
import { BaseRestService } from '../../shared/base-rest.service';
import { BomLevel } from './bom-level.model';
import { HttpClient } from '@angular/common/http';
import { HttpErrorHandler } from '../../shared/http-error-handler.service';

@Injectable()
export class BomLevelService extends BaseRestService<BomLevel> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/BomLevel/",
      "BomLevelService",
      "Rowid",
      httpErrorHandler
    )
  }
}
