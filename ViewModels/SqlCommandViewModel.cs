using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels
{
    public class SqlCommandViewModel
    {
        public string SelectCommand { get; set; }
        public string FromCommand { get; set; }
        public string WhereCommand { get; set; }
        public string OrderCommand { get; set; }
        public string GroupCommand { get; set; }
    }
}
