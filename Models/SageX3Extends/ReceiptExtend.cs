using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using VipcoSageX3.Models.Machines;

namespace VipcoSageX3.Models.SageX3Extends
{
    public class ReceiptExtend:BaseModel
    {
        [Key]
        public int ReceiptExtendId { get; set; }
        public int? StartRange { get; set; }
        public int? EndRange { get; set; }
        public DateTime? GetDate { get; set; }
        [StringLength(10)]
        public string GetTime { get; set; }
    }
}
