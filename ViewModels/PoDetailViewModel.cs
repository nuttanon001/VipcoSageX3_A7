using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels
{
    public class PoDetailViewModel
    {
        public string ItemNo { get; set; }
        public string ItemDescription { get; set; }
        public string LineOrder { get; set; }
        public string Uom { get; set; }
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public double Discount { get; set; }
        public decimal NetPrice { get; set; }
        public decimal TotalLine { get; set; }
        public string ProjectName { get; set; }
        public string ProjectCode { get; set; }
        public string WorkGroupCode { get; set; }
        public string WorkGroupName { get; set; }
        public string WorkItemCode { get; set; }
        public string WorkItemName { get; set; }
    }
}
