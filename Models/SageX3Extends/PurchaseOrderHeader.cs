using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VipcoSageX3.Models.SageX3Extends
{
    public class PurchaseOrderHeader : BaseModel
    {
        public int PurchaseOrderHeaderId { get; set; }
        public DateTime? PrReceivedDate { get; set; }
        [StringLength(10)]
        public string PrReceivedTime { get; set; }
        [StringLength(350)]
        public string Remark { get; set; }
        //Fk
        public ICollection<PurchaseExtend> PurchaseExtends { get; set; } = new List<PurchaseExtend>();
    }
}
