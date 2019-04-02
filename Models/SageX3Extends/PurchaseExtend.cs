using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VipcoSageX3.Models.SageX3Extends
{
    public class PurchaseExtend : BaseModel
    {
        [Key]
        public int PurchaseExtendId { get; set; }
        public int? PrSageHeaderId { get; set; }  
        [StringLength(20)]
        public string PRNumber { get; set; }
        public DateTime? PrReceivedDate { get; set; }
        [StringLength(10)]
        public string PrReceivedTime { get; set; }
        [StringLength(350)]
        public string Remark { get; set; }
        // FK
        public int? PurchaseOrderHeaderId { get; set; }
        public PurchaseOrderHeader PurchaseOrderHeader { get; set; }
        public ICollection<PurchaseLineExtend> PurchaseLineExtends { get; set; } = new List<PurchaseLineExtend>();
    }
}
