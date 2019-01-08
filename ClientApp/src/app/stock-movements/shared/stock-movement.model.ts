import { StockMovement2 } from "./stock-movement2.model";

export interface StockMovement {
  /// <summary>
  /// ITMMASTER.ITMREF_0
  /// </summary>
  ItemCode?: string;
  /// <summary>
  /// ITMMASTER.ITMDES1_0
  /// </summary>
  ItemDesc?: string;
  /// <summary>
  /// TEXCLOB.TEXTE_0
  /// </summary>
  ItemDescFull?: string;
  /// <summary>
  /// ITMMASTER.TCLCOD_0
  /// </summary>
  Category?: string;
  /// <summary>
  /// ATEXTRA.TEXTE_0
  /// </summary>
  CategoryDesc?: string;
  /// <summary>
  /// STOJOU.PRIVAL_0
  /// </summary>
  UnitPrice?: number;
  /// <summary>
  /// STOJOU.AMTVAL_0
  /// </summary>
  Amount?: number;
  /// <summary>
  /// STOJOU.PCU_0
  /// </summary>
  Uom?: string;

  StockMovement2s?: Array<StockMovement2>;
}
