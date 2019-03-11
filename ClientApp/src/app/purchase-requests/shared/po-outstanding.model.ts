export interface PoOutstanding {
  PoDate?: Date;
  DueDate?: Date;
  SysDate?: Date;

  PoNumber?: string;
  Project?: string;
  PoDateString?: string;
  DueDateString?: string;
  ItemNo?: string;
  ItemName?: string;
  TextName?: string;
  Uom?: string;
  Branch?: string;
  WorkItem?: string;
  ProjectLine?: string;
  WorkGroup?: string;
  QuantityString?: string;
  WeigthString?: string;
  AmountString?: string;
  StatusCloseString?: string;
  StatusOrderString?: string;
  SupName?: string;
  SupName2?: string;
  SysDateString?: string;
  DIFFString?: string;

  Quantity?: number;
  Weigth?: number;
  Amount?: number;
  StatusClose?: number;
  StatusOrder?: number;
  DIFF?: number;
}
