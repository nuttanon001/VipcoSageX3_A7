using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels.StockModels
{
    public class OnHandMk2ViewMode
    {
        public string LocationCode { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string ItemCate { get; set; }
        public string TextName { get; set; }
        public string LotCode { get; set; }
        public double? Quantity { get; set; }
        public string QuantityString => this.Quantity != null ? string.Format("{0:#,##0.00}", this.Quantity) : "0";
        public string Uom { get; set; }
        public double? PriceTotal { get; set; }
        public double? PriceAndVat { get; set; }
        public double? Vat { get; set; }
        public double? UnitPrice { get; set; }
        public string UnitPriceString => this.UnitPrice != null ? string.Format("{0:#,##0.00}", this.UnitPrice) : "0";
        public DateTime? OrderDate { get; set; }
        public double? Amount => this.UnitPrice != null && this.Quantity != null ? this.UnitPrice * this.Quantity : 0;
        public string AmountString => this.Amount != null ? string.Format("{0:#,##0.00}", this.Amount) : "0";
        public string OrderDateString => OrderDate != null ? OrderDate.Value.ToString("dd/MM/yyyy") : "";
    }
}
