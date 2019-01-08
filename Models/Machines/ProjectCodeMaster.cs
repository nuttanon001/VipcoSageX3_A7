using System;
using System.Collections.Generic;

namespace VipcoSageX3.Models.Machines
{
    public partial class ProjectCodeMaster
    {
        public int ProjectCodeMasterId { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Creator { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Modifyer { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public DateTime? StartDate { get; set; }
    }
}
