using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels
{
    public class MiscAndAcountViewModel
    {
        // Misc
        public string MiscNumber { get; set; }
        public DateTime? MiscDate { get; set; }
        public string MiscDateString => this.MiscDate == null ? "-" : this.MiscDate.Value.ToString("dd/MM/yyyy");
        public string Project { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        // Account
        public string AccNumber { get; set; }
        public DateTime? AccDate { get; set; }
        public string AccDateString => this.AccDate == null ? "-" : this.AccDate.Value.ToString("dd/MM/yyyy");
        public string MiscLink { get; set; }
        public string AccType { get; set; }
        public byte? AccCat { get; set; }
        public string AccIssue { get; set; }
        public DateTime? DocDate { get; set; }
        public string DocDateString => this.DocDate == null ? "-" : this.DocDate.Value.ToString("dd/MM/yyyy");

        // Detail
        public ICollection<IssueViewModel> Issues { get; set; } = new List<IssueViewModel>();
        public ICollection<JournalViewModel> Journals { get; set; } = new List<JournalViewModel>();
    }
}
