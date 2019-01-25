using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VipcoSageX3.Models.SageX3Extends
{
    public class ScheduleEmailAddress
    {
        [Key]
        public int ScheduleEmailId { get; set; }
        [StringLength(200)]
        public string FullName { get; set; }
        [Required]
        [StringLength(250)]
        public string EmailAddress { get; set; }
        //FK
        //WorkGroup
        [Required]
        [StringLength(50)]
        public string WorkGroupCode { get; set; }
        [StringLength(200)]
        public string WorkGroupName { get; set; }

    }
}
