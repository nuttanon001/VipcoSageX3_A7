using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VipcoSageX3.Models.Machines;

namespace VipcoSageX3.ViewModels
{
    public class UserViewModel:User
    {
        public string NameThai { get; set; }
        public string Token { get; set; }
        public int? SubLevel { get; set; }
    }
}
