import { BaseModel } from 'src/app/shared2/basemode/base-model.model';

export interface PurchaseLineExtend extends BaseModel {
  PurchaseLineExtendId: number;
  PrSageLineId?: number;
  PrLine?: number;
  PurchaseExtendId?: number;
  PrNumber?: string;
  ItemCode?: string;
  ItemName?: string;
  Remark?: string;
  Quantity?: number;
}
