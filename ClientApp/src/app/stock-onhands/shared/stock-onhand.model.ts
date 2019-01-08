import { StockLocation } from "./stock-location.model";

export interface StockOnhand {
  /// <summary>
  /// ITMMASTER.ITMREF_0
  /// </summary>
  ItemCode?: string;
  /// <summary>
  /// ITMMASTER.ITMDES1_0
  /// </summary>
  ItemDesc?: string;
  /// <summary>
  /// ITMMASTER.STU_0
  /// </summary>
  Uom?: string;
  /// <summary>
  /// ITMMASTER.TCLCOD_0
  /// </summary>
  Category?: string;
  /// <summary>
  /// TEXCLOB.TEXTE_0
  /// </summary>
  ItemDescFull?: string;
  /// <summary>
  /// ATEXTRA.TEXTE_0
  /// </summary>
  CategoryDesc?: string;
  /// <summary>
  /// ITMMVT.PHYSTO_0
  /// </summary>
  InternelStock?: number;
  /// <summary>
  /// ITMFACILIT.OFS_0
  /// </summary>
  OnOrder?: number;
  // ViewModel
  InternelStockString?: string;
  OnOrderString?: string;

  StockLocations?: Array<StockLocation>;
}
