import { BaseModel } from 'src/app/shared2/basemode/base-model.model';
import { PurchaseExtend } from './purchase-extend.model';

export interface PurchaseOverHeader extends BaseModel {
  PurchaseOrderHeaderId: number;
  PrReceivedDate?: Date;
  PrReceivedTime?: string;
  Remark?: string;
  //Fk
  PurchaseExtends?: Array<PurchaseExtend>;
}
