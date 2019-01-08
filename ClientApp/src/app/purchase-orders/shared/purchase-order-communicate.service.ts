import { Injectable } from '@angular/core';
import { BaseCommunicateService } from '../../shared/base-communicate.service';
import { PurchaseOrder } from './purchase-order.model';

@Injectable()
export class PurchaseOrderCommunicateService extends BaseCommunicateService<PurchaseOrder> {
  constructor() { super() }
}
