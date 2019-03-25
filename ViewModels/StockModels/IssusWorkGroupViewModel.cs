using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels.StockModels
{
    public class IssusWorkGroupViewModel
    {
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string TextName { get; set; }
        public string Uom { get; set; }
        public string Branch { get; set; }
        public string Project { get; set; }
        public string WorkItem { get; set; }
        public string WorkGroup { get; set; }
        public string WorkGroupName { get; set; }
        public double? Quantity { get; set; }
        public string QuantityString => this.Quantity != null ? string.Format("{0:#,##0.00}", (this.Quantity >= 0 ? this.Quantity : this.Quantity * -1 )) : "0";
        public double? UnitPrice { get; set; }
        public string UnitPriceString => this.UnitPrice != null ? string.Format("{0:#,##0.00}", (this.UnitPrice >= 0 ? this.UnitPrice : this.UnitPrice * -1)) : "0";
        public double? TotalCost { get; set; }
        public string TotalCostString => this.TotalCost != null ? string.Format("{0:#,##0.00}", (this.TotalCost >= 0 ? this.TotalCost : this.TotalCost * -1)) : "0";
    }
}
