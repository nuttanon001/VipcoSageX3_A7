using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels
{
    public class JournalViewModel
    {
        public int? AccLine { get; set; }
        public int? CurType { get; set; }
        public decimal? AmountCurrency { get; set; }
        public string AmountCurrencyString => this.AmountCurrency == null ? "-" : string.Format("{0:#,##0.00}", this.AmountCurrency*this.CurType);
        public string AccountCode { get; set; }
        public string Branch { get; set; }
        public string BranchName { get; set; }
        public string WorkItem { get; set; }
        public string WorkItemName { get; set; }
        public string Project { get; set; }
        public string ProjectName { get; set; }
        public string WorkGroup { get; set; }
        public string WorkGroupName { get; set; }
        public int? AccountNumber { get; set; }
        public string Description { get; set; }
        public string FreeREF { get; set; }
    }
}
