using System;
using System.Collections.Generic;

namespace VipcoSageX3.Models.Machines
{
    public partial class Employee
    {
        public Employee()
        {
            User = new HashSet<User>();
        }

        public string EmpCode { get; set; }
        public string NameEng { get; set; }
        public string NameThai { get; set; }
        public string Title { get; set; }
        public int? TypeEmployee { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public string GroupMis { get; set; }

        public EmployeeGroupMis GroupMisNavigation { get; set; }
        public ICollection<User> User { get; set; }
    }
}
