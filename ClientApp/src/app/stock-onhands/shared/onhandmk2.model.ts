export interface Onhandmk2 {
  LocationCode?: string;
  ItemNo?: string;
  ItemName?: string;
  ItemCate?: string;
  TextName?: string;
  LotCode?: string;
  QuantityString?: string;
  Uom?: string;
  UnitPriceString?: string;
  AmountString?: string;
  OrderDateString?: string;

  Quantity?: number;
  UnitPrice?: number;
  Amount?: number;
  PriceTotal?: number;
  PriceAndVat?: number;
  Vat?: number;

  OrderDate?: Date;
}
