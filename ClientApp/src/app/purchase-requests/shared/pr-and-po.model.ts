import { PurchaseReceipt } from "./purchase-receipt.model";

export interface PrAndPo {
  //Purchase Request
  ToDate?: Date;
  PrWeight?: number;
  PrWeightString?: string;
  /// <summary>
  /// PREQUISD.PRQDAT
  /// </summary>
  RequestDate?: Date;
  RequestDateString?: string;
  /// <summary>
  /// PREQUISD.PSHNUM
  /// </summary>
  PrNumber?: string;
  /// <summary>
  /// PREQUISD.PSDLIN
  /// </summary>
  PrLine?: number;
  PROther?: string;
  /// <summary>
  /// PREQUISD.ITMREF
  /// </summary>
  ItemCode?: string;
  /// <summary>
  /// PREQUISD.ITMDES
  /// </summary>
  ItemName?: string;
  /// <summary>
  /// PREQUISD.PUU
  /// </summary>
  PurUom?: string;
  /// <summary>
  /// PREQUISD.STU
  /// </summary>
  StkUom?: string;
  /// <summary>
  /// CPTANALIN.CCE0 WHERE VCRNUM = PSHNUM AND VCRLIN = PSDLIN
  /// </summary>
  Branch?: string;
  BranchName?: string;
  /// <summary>
  /// CPTANALIN.CCE1 WHERE VCRNUM = PSHNUM AND VCRLIN = PSDLIN
  /// </summary>
  WorkItem?: string;
  WorkItemName?: string;
  /// <summary>
  /// CPTANALIN.CCE3 WHERE VCRNUM = PSHNUM AND VCRLIN = PSDLIN
  /// </summary>
  WorkGroup?: string;
  WorkGroupName?: string;
  /// <summary>
  /// CPTANALIN.CCE2 WHERE VCRNUM = PSHNUM AND VCRLIN = PSDLIN
  /// </summary>
  Project?: string;
  ProjectName?: string;
  /// <summary>
  /// PREQUISD.QTYPUU
  /// </summary>
  QuantityPur?: number;
  /// <summary>
  /// PREQUISD.QTYSTU
  /// </summary>
  QuantityStk?: number;
  /// <summary>
  /// PREQUISD.EXTRCPDAT
  /// </summary>
  PRDate?: Date;
  PRDateString?: string;
  /// <summary>
  /// PREQUISO.POHNUM
  /// </summary>
  LinkPoNumber?: string;
  /// <summary>
  /// PREQUISO.POPLIN
  /// </summary>
  LinkPoLine?: number;
  /// <summary>
  /// PREQUISO.POQSEQ
  /// </summary>
  LinkPoSEQ?: number;
  /// <summary>
  /// PREQUIS.CREUSR
  /// </summary>
  CreateBy?: string;
  /// <summary>
  /// PREQUIS.CLEFLG_0
  /// </summary>
  PrCloseStatus?: string;
  // Purchase Order
  /// <summary>
  /// PORDERQ.POHNUM
  /// </summary>
  PoNumber?: string;
  /// <summary>
  /// PORDERQ.POPLIN
  /// </summary>
  PoLine?: number;
  /// <summary>
  /// PORDERQ.POQSEQ
  /// </summary>
  PoSequence?: number;
  /// <summary>
  /// PORDERQ.ORDDAT
  /// </summary>
  PoDate?: Date;
  PoDateString?: string;
  /// <summary>
  /// PORDERQ.EXTRCPDAT
  /// </summary>
  DueDate?: Date;
  DueDateString?: string;
  /// <summary>
  /// PORDERQ.PUU
  /// </summary>
  PoPurUom?: string;
  /// <summary>
  /// PORDERQ.STU
  /// </summary>
  PoStkUom?: string;
  /// <summary>
  /// PORDERQ.QTYPUU
  /// </summary>
  PoQuantityPur?: number;
  /// <summary>
  /// PORDERQ.QTYSTU
  /// </summary>
  PoQuantityStk?: number;
  /// <summary>
  /// PORDERQ.QTYWEU
  /// </summary>
  PoQuantityWeight?: number;
  /// <summary>
  /// CPTANALIN.CCE0 WHERE VCRNUM = POHNUM AND VCRLIN = POPLIN AND VCRSEQ = POQSEQ
  /// </summary>
  PoBranch?: string;
  PoBranchName?: string;
  /// <summary>
  /// CPTANALIN.CCE1 WHERE VCRNUM = POHNUM AND VCRLIN = POPLIN AND VCRSEQ = POQSEQ
  /// </summary>
  PoWorkItem?: string;
  PoWorkItemName?: string;
  /// <summary>
  /// CPTANALIN.CCE3 WHERE VCRNUM = PSHNUM AND VCRLIN = PSDLIN AND VCRSEQ = POQSEQ
  /// </summary>
  PoWorkGroup?: string;
  PoWorkGroupName?: string;
  /// <summary>
  /// CPTANALIN.CCE2 WHERE VCRNUM = PSHNUM AND VCRLIN = PSDLIN AND VCRSEQ = POQSEQ
  /// </summary>
  PoProject?: string;
  PoProjectName?: string;
  /// <summary>
  /// PORDER.ZPO210
  /// </summary>
  PoStatus?: string;
  /// <summary>
  /// PORDER.Cleflg0
  /// </summary>
  CloseStatus?: string;
  PurchaseReceipts?: Array<PurchaseReceipt>;

  //Addition Requirement
  ReceivedDate?: string;
  PurchaseComment?: string;
}
