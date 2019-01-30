using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels
{
    public class ScrollDataViewModel2<TEntity> where TEntity : class
    {
        public ScrollViewModel2 Scroll { get; private set; }
        public IEnumerable<TEntity> Data { get; private set; }
        public ScrollDataViewModel2(ScrollViewModel2 scroll, IEnumerable<TEntity> data)
        {
            this.Scroll = scroll;
            this.Data = data;
        }
    }
}
