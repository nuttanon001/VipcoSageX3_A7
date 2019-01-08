import { BaseModel } from "../../shared/base-model.model";

export interface Supplier extends BaseModel {
  SupplierNo?: string;
  SupplierName?: string;
  RowId?: number;
}
