export interface StockLocation {
  /// <summary>
  /// STOCK.LOC_0
  /// </summary>
  LocationCode?: string;
  /// <summary>
  /// STOCK.QTYPCU_0
  /// </summary>
  Quantity?: number;
  QuantityString?: string;
  /// <summary>
  /// STOCK.PCU_0
  /// </summary>
  Uom?: string;
  /// <summary>
  /// STOCK.PJT_0
  /// </summary>
  Project?: string;
  /// <summary>
  /// STOCK.LOT_0
  /// </summary>
  LotNo?: string;
  /// <summary>
  /// STOCK.BPSLOT_0
  /// </summary>
  HeatNo?:string;
  /// <summary>
  /// STOCK.PALNUM_0
  /// </summary>
  Origin?:string;
  /// <summary>
  /// STOLOT.SHLDAT
  /// </summary>
  ExpDate?: Date;
  ExpDateString?: string;
}
