using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels
{
    public class IssueViewModel
    {
        public string MiscNumber { get; set; }
        public int? MiscLine { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Uom { get; set; }
        public double? Qty { get; set; }
        public string QtyString => this.Qty != null ? string.Format("{0:#,##0.00}", this.Qty) : "0";
        public string Branch { get; set; }
        public string WorkItem { get; set; }
        public string Project { get; set; }
        public string WorkGroup { get; set; }
        public string ItemNameREF { get; set; }
        public string ItemNameRFT { get; set; }
    }
}
