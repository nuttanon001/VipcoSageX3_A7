export interface StockBalance {
  LocationCode?: string;
  ItemNo?: string;
  ItemName?: string;
  ItemCate?: string;
  TextName?: string;
  QuantityString?: string
  Uom?: string;
  Available?: string;
  ReOrderString?: string;

  Quantity?: number;
  ReOrder?: number;
}
