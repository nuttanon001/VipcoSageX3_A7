export interface PrOutstanding {
  PrDate?: Date;
  RequestDate?: Date;
  NowDate?: Date;
  Other?: string;
  PrType?: number;
  PrTypeString?: number;
  PrNumber?: string;
  Project?: string;
  PrDateString?: string;
  RequestDateString?: string;
  ItemNo?: string;
  ItemName?: string;
  TextName?: string;
  Uom?: string;
  Branch?: string;
  WorkItem?: string;
  ProjectLine?: string;
  WorkGroup?: string;
  QuantityString?: string;
  StatusCloseString?: string;
  StatusOrderString?: string;
  CreateBy?: string;
  ItemWeigthString?: string;
  WeightPerQtyString?: string;
  NowDateString?: string;
  DIFFString?: string;

  Quantity?: number;
  ItemWeigth?: number;
  WeightPerQty?: number;
  StatusClose?: number;
  StatusOrder?: number;
  DIFF?: number;
}
