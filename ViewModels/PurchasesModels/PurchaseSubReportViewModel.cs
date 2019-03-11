using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels
{
    public class PurchaseSubReportViewModel
    {
        public DateTime? PoDate { get; set; }
        public string PoDateString => PoDate != null ? PoDate.Value.ToString("dd/MM/yyyy") : "";
        public string PoNumber { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string TextName { get; set; }
        public string Project { get; set; }
        public string ProjectLine { get; set; }
        public string Branch { get; set; }
        public string SupName { get; set; }
        public string SupName2 { get; set; }
        public string Uom { get; set; }
        public double? Quantity { get; set; }
        public string QuantityString => this.Quantity != null ? string.Format("{0:#,##0.00}", this.Quantity) : "0";
        public double? UnitPrice => this.Quantity != null && this.Amount != null ? this.Amount / this.Quantity : 0;
        public string UnitPriceString => this.UnitPrice != null ? string.Format("{0:#,##0.00}", this.UnitPrice) : "0";
        public double? Amount { get; set; }
        public string AmountString => this.Amount != null ? string.Format("{0:#,##0.00}", this.Amount) : "0";
        public double? WeigthPerQuantity => this.Weigth != null && this.Quantity != null ? this.Weigth / this.Quantity : 0;
        public string WeigthPerQuantityString => this.WeigthPerQuantity != null ? string.Format("{0:#,##0.00}", this.WeigthPerQuantity) : "0";
        public double? Weigth { get; set; }
        public string WeigthString => this.Weigth != null ? string.Format("{0:#,##0.00}", this.Weigth) : "0";
        public double? AmountPerKg => this.Amount != null && this.Weigth != null && this.Weigth > 0 ? this.Amount / this.Weigth : 0;
        public string AmountPerKgString => this.AmountPerKg != null ? string.Format("{0:#,##0.00}", this.AmountPerKg) : "0";
    }
}
