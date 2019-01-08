using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels
{
    public class ReturnViewModel<Entity>
    {
        public List<Entity> Entities { get; set; }
        public int? TotalRow { get; set; }
    }
}
