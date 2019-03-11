using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels
{
    public class Journal2ViewModel
    {
        public string DocumentNo { get; set; }
        public DateTime? Date { get; set; }
        public string DateString => this.Date == null ? "-" : this.Date.Value.ToString("dd/MM/yyyy");
        public string EntryType { get; set; }
        public string Journal { get; set; }
        public string Site { get; set; }
        public string Account { get; set; }
        public double? Debit { get; set; }
        public string DebitString => this.Debit != null ? string.Format("{0:#,##0.00}", this.Debit) : "0.00";
        public double? Credit { get; set; }
        public string CreditString => this.Credit != null ? string.Format("{0:#,##0.00}", this.Credit) : "0.00";
        public string Description { get; set; }
        public string Project { get; set; }
        public string Branch { get; set; }
        public string Bom { get; set; }
        public string WorkGroup { get; set; }
        public string CostCenter { get; set; }
        public string FreeReference { get; set; }
        public string Tax { get; set; }
    }
}
