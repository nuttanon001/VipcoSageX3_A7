using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels.PurchasesModels
{
    public class PurchaseRequestPureViewModel
    {
        public string PrNumber { get; set; }
        public DateTime? PrDate { get; set; }
        public string PrType { get; set; }
        public string StatusClose { get; set; }
        public string StatusOrder { get; set; }
        public int? PrSageHeaderId { get; set; }
    }
}
