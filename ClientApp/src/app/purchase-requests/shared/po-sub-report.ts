export interface PoSubReport {
  PoDate?: Date;
  PoDateString?: string;
  PoNumber?: string;
  ItemNo?: string;
  ItemName?: string;
  TextName?: string;
  Project?: string;
  ProjectLine?: string;
  Branch?: string;
  SupName?: string;
  SupName2?: string;
  Uom?: string;
  QuantityString?: string;
  UnitPriceString?: string;
  AmountString?: string;
  WeigthPerQuantityString?: string;
  WeigthString?: string;
  AmountPerKgString?: string;

  Quantity?: number;
  UnitPrice?: number;
  Amount?: number;
  WeigthPerQuantity?: number;
  Weigth?: number;
  AmountPerKg?: number;
}
