import { BaseModel } from 'src/app/shared2/basemode/base-model.model';

export interface PurchaseRequestPure extends BaseModel {
  PrNumber?: string;
  PrType?: string;
  StatusClose?: string;
  StatusOrder?: string;
  PrSageHeaderId?: number;
  PrDate?: Date;
}
