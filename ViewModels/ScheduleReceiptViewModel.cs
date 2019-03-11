using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels
{
    public class ScheduleReceiptViewModel
    {
        public int? RowId { get; set; }
        public string PoNumber { get; set; }
        public int? PoLine { get; set; }
        public string ItemCode { get; set; }
        public string Branch { get; set; }
        public string BranchName { get; set; }
        public string Project { get; set; }
        public string ProjectName { get; set; }
        public string WorkItem { get; set; }
        public string WorkItemName { get; set; }
        public string WorkGroup { get; set; }
        public string WorkGroupName { get; set; }
        public byte? PoStatusInt { get; set; }
        public string PoStatus => this.PoStatusInt == null ? "-" :
                        this.PoStatusInt == 1 ? "จัดซื้อในประเทศ" :
                        (this.PoStatusInt == 2 ? "จัดจ้าง" :
                            (this.PoStatusInt == 3 ? "Oversea Purchasing" :
                                (this.PoStatusInt == 4 ? "Mat Stock" :
                                    (this.PoStatusInt == 5 ? "Surplus" : "Consumable Stock"))));
        public double? PoQuantityPur { get; set; }
        public string PrNumber { get; set; }
        public int? PrLine { get; set; }
        public double? QuantityPur { get; set; }
        public string PurUom { get; set; }
        public string ItemName { get; set; }
        public double? RCQuantityPur { get; set; }
    }
}
