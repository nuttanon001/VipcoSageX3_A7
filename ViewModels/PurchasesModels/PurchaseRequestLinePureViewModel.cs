using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace VipcoSageX3.ViewModels.PurchasesModels
{
    public class PurchaseRequestLinePureViewModel
    {
        public int? PrSageLineId { get; set; }
        public string PrNumber { get; set; }
        public int? PrLine { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public double? Quantity { get; set; }
    }
}
