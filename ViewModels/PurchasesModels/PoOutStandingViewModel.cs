using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels.PurchasesModels
{
    public class PoOutStandingViewModel
    {
        public string PoNumber { get; set; }
        public string Project { get; set; }
        public DateTime? PoDate { get; set; }
        public string PoDateString => this.PoDate != null ? this.PoDate.Value.ToString("dd/MM/yyyy") : "-";
        public DateTime? DueDate { get; set; }
        public string DueDateString => this.DueDate != null ? this.DueDate.Value.ToString("dd/MM/yyyy") : "-";
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string TextName { get; set; }
        public int? PoStatus { get; set; }
        public string Uom { get; set; }
        public string Branch { get; set; }
        public string WorkItem { get; set; }
        public string ProjectLine { get; set; }
        public string WorkGroup { get; set; }
        public double? Quantity { get; set; }
        public string QuantityString => this.Quantity != null ? string.Format("{0:#,##0.00}", this.Quantity) : "0";
        public double? Weigth { get; set; }
        public string WeigthString => this.Weigth != null ? string.Format("{0:#,##0.00}", this.Weigth) : "0";
        public double? Amount { get; set; }
        public string AmountString => this.Amount != null ? string.Format("{0:#,##0.00}", this.Amount) : "0";
        public int? StatusClose { get; set; }
        public string StatusCloseString => this.StatusClose != null ? (this.StatusClose == 1 ? "No" : "Yes") : "-";
        public int? StatusOrder { get; set; }
        public string StatusOrderString => this.StatusOrder == null ? "-" :
                                            this.StatusOrder == 1 ? "จัดซื้อในประเทศ" :
                                            (this.StatusOrder == 2 ? "จัดจ้าง" :
                                                (this.StatusOrder == 3 ? "Oversea Purchasing" :
                                                    (this.StatusOrder == 4 ? "Mat Stock" :
                                                        (this.StatusOrder == 5 ? "Surplus" : "Consumable Stock"))));
        public string SupName { get; set; }
        public string SupName2 { get; set; }
        public DateTime? SysDate { get; set; }
        public string SysDateString => this.SysDate != null ? this.SysDate.Value.ToString("dd/MM/yyyy") : "-";
        public int? DIFF { get; set; }
        public string DIFFString => this.DIFF != null ? (this.DIFF < -21 ? "> 21" : (this.DIFF < -14 ? "> 14" : (this.DIFF < -7 ? "> 7" : "< 7"))) : "-";
    }
}
