import { BaseModel } from 'src/app/shared2/basemode/base-model.model';
import { PurchaseLineExtend } from './purchase-line-extend.model';

export interface PurchaseExtend extends BaseModel {
  PurchaseExtendId: number;
  PrSageHeaderId?: number;
  PrReceivedDate?: Date;
  PRNumber?: string;
  PrReceivedTime?: string;
  Remark?: string;
  // FK
  PurchaseOrderHeaderId?: number; 
  PurchaseLineExtends?: Array<PurchaseLineExtend>;
}
