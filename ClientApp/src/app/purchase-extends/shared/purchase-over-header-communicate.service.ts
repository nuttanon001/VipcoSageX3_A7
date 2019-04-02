import { Injectable } from '@angular/core';
import { BaseCommunicateService } from 'src/app/shared2/baseclases/base-communicate.service';
import { PurchaseOverHeader } from './purchase-over-header.model';

@Injectable()
export class PurchaseOverHeaderCommunicateService
  extends BaseCommunicateService<PurchaseOverHeader> {

  constructor() { super(); }
}
