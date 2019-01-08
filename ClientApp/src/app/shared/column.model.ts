export interface MyPrimengColumn {
  field?: string;
  header?: string;
  style?: string;
  width?: number;
  type?: ColumnType;
  colspan?: number;
}

export enum ColumnType {
  PurchaseRequest = 1,
  PurchaseOrder,
  PurchaseReceipt,
  Hidder,
  Group1,
  Group2,
}
