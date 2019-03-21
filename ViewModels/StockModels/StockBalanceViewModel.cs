using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels.StockModels
{
    public class StockBalanceViewModel
    {
        public string LocationCode { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string ItemCate { get; set; }
        public string TextName { get; set; }
        public double? Quantity { get; set; }
        public string QuantityString => this.Quantity != null ? string.Format("{0:#,##0.00}", this.Quantity) : "0";
        public string Uom { get; set; }
        public string Available { get; set; }
        public double? ReOrder { get; set; }
        public string ReOrderString => this.ReOrder != null ? string.Format("{0:#,##0.00}", this.ReOrder) : "-";
    }
}
