import { BaseModel } from "../../shared/base-model.model";

export interface PurchaseOrder extends BaseModel {
  Rowid?: number;
  PurchaseOrderNo? : string;
  OrderDate?: Date;
  OrderDateString?: string;
  SupplierName? : string;
  SupplierCode? : string;
  ProjectName? : string;
  ProjectCode? : string;
  ReceiveByCode? : string;
  ReceiveByName? : string;
  WorkGroupCode? : string;
  WorkGroupName? : string;
  WorkItemCode? : string;
  WorkItemName? : string;
  ShipTo? : string;
  TypePoHString? : string;
  TypePoH ?: number;
  CloseStatus ?: number;
  CloseStatusString? : string;
  ReceivedStatus ?: number;
  ReceivedStatusString? : string;
}
