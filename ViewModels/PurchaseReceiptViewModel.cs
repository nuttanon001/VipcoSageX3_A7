using System;

namespace VipcoSageX3.ViewModels
{
    public class PurchaseReceiptViewModel
    {
        #region PurchaseReceipt

        /// <summary>
        /// PRECEIPTD.PTHNUM
        /// </summary>
        public string RcNumber { get; set; }

        /// <summary>
        /// PRECEIPTD.PTDLIN
        /// </summary>+
        public int? RcLine { get; set; }

        /// <summary>
        /// PRECEIPTD.RCPDAT
        /// </summary>
        public DateTime? RcDate { get; set; }

        // Request Date PRQDAT
        public string RcDateString => this.RcDate == null ? "-" : this.RcDate.Value.ToString("dd/MM/yyyy");

        /// <summary>
        /// STOJOU.LOT_0
        /// </summary>
        public string HeatNumber { get; set; }
        /// <summary>
        /// STOJOU.SLO_0
        /// </summary>
        public string HeatNumber2 { get; set; }

        /// <summary>
        /// STOJOU.STA_0
        /// </summary>
        public string RcStatus { get; set; }

        /// <summary>
        /// PRECEIPTD.PUU
        /// </summary>
        public string RcPurUom { get; set; }

        /// <summary>
        /// PRECEIPTD.STU
        /// </summary>
        public string RcStkUom { get; set; }

        /// <summary>
        /// PRECEIPTD.UOM
        /// </summary>
        public string RcUom { get; set; }

        /// <summary>
        /// PORDERQ.QTYPUU
        /// </summary>
        public double? RcQuantityPur { get; set; }

        /// <summary>
        /// PORDERQ.QTYSTU
        /// </summary>
        public double? RcQuantityStk { get; set; }

        /// <summary>
        /// PORDERQ.QTYUOM
        /// </summary>
        public double? RcQuantityUom { get; set; }

        /// <summary>
        /// PORDERQ.QTYWEU
        /// </summary>
        public double? RcQuantityWeight { get; set; }

        /// <summary>
        /// PORDERQ.INVQTYPUU
        /// </summary>
        public double? RcQuantityInvPur { get; set; }

        /// <summary>
        /// PORDERQ.INVQTYSTU
        /// </summary>
        public double? RcQuantityInvStk { get; set; }

        /// <summary>
        /// CPTANALIN.CCE0 WHERE VCRNUM = PTHNUM AND VCRLIN = PTDLIN
        /// </summary>
        public string RcBranch { get; set; }

        public string RcBranchName { get; set; }

        /// <summary>
        /// CPTANALIN.CCE1 WHERE VCRNUM = PTHNUM AND VCRLIN = PTDLIN
        /// </summary>
        public string RcWorkItem { get; set; }

        public string RcWorkItemName { get; set; }

        /// <summary>
        /// CPTANALIN.CCE3 WHERE VCRNUM = PTHNUM AND VCRLIN = PTDLIN
        /// </summary>
        public string RcWorkGroup { get; set; }

        public string RcWorkGroupName { get; set; }

        /// <summary>
        /// CPTANALIN.CCE2 WHERE VCRNUM = PTHNUM AND VCRLIN = PTDLIN
        /// </summary>
        public string RcProject { get; set; }

        public string RcProjectName { get; set; }

        #endregion PurchaseReceipt
    }
}