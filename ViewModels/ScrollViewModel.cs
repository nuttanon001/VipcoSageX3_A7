using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels
{
    public class ScrollViewModel
    {
        //The Skip number
        public int? Skip { get; set; }
        //The Take number
        public int? Take { get; set; }
        public string SortField { get; set; }
        public int? SortOrder { get; set; }
        public string Filter { get; set; }
        public bool? Reload { get; set; }
        public string Where { get; set; }
        public string WhereWorkItem { get; set; }
        public List<string> WhereWorkItems { get; set; } = new List<string>();

        public string WhereWorkGroup { get; set; }
        public string WhereProject { get; set; }
        public List<string> WhereProjects { get; set; } = new List<string>();
        public string WhereBranch { get; set; }
        public List<string> WhereBranchs { get; set; } = new List<string>();
        public string WhereBank { get; set; }
        public List<string> WhereBanks { get; set; } = new List<string>();
        public string WhereSupplier { get; set; }
        public string WhereRange11 { get; set; }
        public string WhereRange12 { get; set; }
        public string WhereRange21 { get; set; }
        public string WhereRange22 { get; set; }
        public int? WhereId { get; set; }
        public int? TotalRow { get; set; }
        public bool? CheckOption { get; set; }
        public DateTime? SDate { get; set; }
        public DateTime? EDate { get; set; }
        public DateTime? SDate2 { get; set; }
        public DateTime? EDate2 { get; set; }
    }
}
