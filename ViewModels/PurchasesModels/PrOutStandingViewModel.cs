using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels
{
    public class PrOutStandingViewModel
    {
        public string PrNumber { get; set; }
        public int? PrLine { get; set; }
        public string Project { get; set; }
        public DateTime? PrDate { get; set; }
        public string PrDateString => this.PrDate != null ? this.PrDate.Value.ToString("dd/MM/yyyy") : "-";
        public DateTime? RequestDate { get; set; }
        public string RequestDateString => this.RequestDate != null ? this.RequestDate.Value.ToString("dd/MM/yyyy") : "-";
        public string Other { get; set; }
        public int? PrType { get; set; }
        public string PrTypeString => this.PrType != null ? (this.PrType == 1 ? "จัดซื้อ" : "จัดจ้าง") : "-";
        public string ReceivedDate { get; set; }
        public string PurchaseComment { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string TextName { get; set; }
        public string Uom { get; set; } 
        public string Branch { get; set; }
        public string WorkItem { get; set; }
        public string ProjectLine { get; set; }
        public string WorkGroup { get; set; }
        public double? Quantity { get; set; }
        public double? ItemWeigth { get; set; }
        public string QuantityString => this.Quantity != null ? string.Format("{0:#,##0.00}", this.Quantity) : "0";
        public string ItemWeigthString => this.ItemWeigth != null ? string.Format("{0:#,##0.00}", this.ItemWeigth) : "0";
        public double? WeightPerQty => this.Quantity != null && this.ItemWeigth != null ? this.ItemWeigth / this.Quantity : 0;
        public string WeightPerQtyString => this.WeightPerQty != null ? string.Format("{0:#,##0.00}", this.WeightPerQty) : "0";
        public int? StatusClose { get; set; }
        public string StatusCloseString => this.StatusClose != null ? (this.StatusClose == 1 ? "No" : "Yes") : "-";
        public int? StatusOrder { get; set; }
        public string StatusOrderString => this.StatusOrder != null ? (this.StatusOrder == 1 ? "No" : "Yes") : "-";
        public string CreateBy { get; set; }
        public DateTime? NowDate { get; set; }
        public string NowDateString => this.NowDate != null ? this.NowDate.Value.ToString("dd/MM/yyyy") : "-";
        public int? DIFF { get; set; }
        public string DIFFString => this.DIFF != null ? ( this.DIFF < -21 ? "> 21" : (this.DIFF < -14 ? "> 14" : (this.DIFF < -7 ? "> 7" : "< 7"))) : "-";
    }
}
