using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels.PaymentModels
{
    public class PaymentRetentionViewModel
    {
        public string PartnerNo { get; set; }
        public string PartnerName { get; set; }
        public string PaymentNo { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentDateString => this.PaymentDate != null ? this.PaymentDate.Value.ToString("dd/MM/yyyy") : "";
        public string DescriptionLine { get; set; }
        public string Branch { get; set; }
        public string WorkItem { get; set; }
        public string Project { get; set; }
        public string WorkGroup { get; set; }
        public string Currency { get; set; }
        public string Attribute { get; set; }
        public double? AmountRetenion { get; set; }
        public string AmountRetenionString => this.AmountRetenion != null ? string.Format("{0:#,##0.00}", (this.AmountRetenion > 0 ? this.AmountRetenion : this.AmountRetenion * -1)) : "0";
        public double? AmountDeduct { get; set; }
        public string AmountDeductString => this.AmountDeduct != null ? string.Format("{0:#,##0.00}", (this.AmountDeduct > 0 ? this.AmountDeduct : this.AmountDeduct * -1)) : "0";
        public string Comment { get; set; }
    }
}
