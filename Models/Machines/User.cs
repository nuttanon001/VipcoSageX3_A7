using System;
using System.Collections.Generic;

namespace VipcoSageX3.Models.Machines
{
    public partial class User
    {
        public int UserId { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Creator { get; set; }
        public string EmpCode { get; set; }
        public string MailAddress { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Modifyer { get; set; }
        public string PassWord { get; set; }
        public string UserName { get; set; }
        public int LevelUser { get; set; }

        public Employee EmpCodeNavigation { get; set; }
    }
}
