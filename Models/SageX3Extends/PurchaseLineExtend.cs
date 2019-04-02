using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VipcoSageX3.Models.SageX3Extends
{
    public class PurchaseLineExtend:BaseModel
    {
        public int PurchaseLineExtendId { get; set; }
        public int? PrSageLineId { get; set; }
        [StringLength(20)]
        public string PrNumber { get; set; }
        public int PrLine { get; set; }
        [StringLength(50)]
        public string ItemCode { get; set; }
        [StringLength(500)]
        public string ItemName { get; set; }
        [StringLength(350)]
        public string Remark { get; set; }
        public double? Quantity { get; set; }
        //FK
        public int? PurchaseExtendId { get; set; }
        public PurchaseExtend PurchaseExtend { get; set; }
    }
}
