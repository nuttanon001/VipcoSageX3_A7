import { Injectable } from '@angular/core';
import { BaseCommunicateService } from 'src/app/shared2/baseclases/base-communicate.service';
import { PurchaseExtend } from './purchase-extend.model';

@Injectable()
export class PurchaseExtendCommunicateService
  extends BaseCommunicateService<PurchaseExtend> {

  constructor() { super(); }
}
