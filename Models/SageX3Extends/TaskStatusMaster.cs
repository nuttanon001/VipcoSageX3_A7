using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VipcoSageX3.Models.SageX3Extends
{
    public class TaskStatusMaster:BaseModel
    {
        [Key]
        public int TaskStatusMasterId { get; set; }
        [StringLength(50)]
        public string WorkGroupCode { get; set; }
        [StringLength(200)]
        public string WorkGroupName { get; set; }
        [StringLength(200)]
        public string Remark { get; set; }
        //FK
        //TaskStatusDetail
        public ICollection<TaskStatusDetail> TaskStatusDetails { get; set; } = new List<TaskStatusDetail>();
    }
}
