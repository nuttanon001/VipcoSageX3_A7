using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VipcoSageX3.Models.SageX3Extends
{
    public class TaskStatusDetail:BaseModel
    {
        [Key]
        public int TaskStatusDetailId { get; set; }
        [StringLength(20)]
        public string EmployeeCode { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(250)]
        public string Email { get; set; }
        [StringLength(200)]
        public string Remark { get; set; }

        // FK
        //TaskStatusMaster
        public int? TaskStatusMasterId { get; set; }
        public TaskStatusMaster TaskStatusMaster { get; set; }
    }
}
