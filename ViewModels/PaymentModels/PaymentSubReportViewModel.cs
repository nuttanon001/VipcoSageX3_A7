using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels.PaymentModels
{
    public class PaymentSubReportViewModel
    {
        public string InvoiceNo { get; set; }
        public string Job { get; set; }
        public double? Progress { get; set; }
        public double? ProgressVat { get; set; }
        public double? DownPayment { get; set; }
        public double? Consume { get; set; }
        public double? TaxBase => (this.Progress ?? 0) - ((this.DownPayment ?? 0) + (this.Consume ?? 0) + (this.ProgressVat ?? 0));
        public double? Retention { get; set; }
        public double? Deduct { get; set; }
        public double? TotalSubPayment => ((this.Progress ?? 0) + (this.Deduct ?? 0)) - ((this.DownPayment ?? 0) + (this.Consume ?? 0) + (this.Retention ?? 0) + (this.ProgressVat ?? 0));
        public double? Vat { get; set; }
        public double? Tax { get; set; }
        public double? NetPayment => ((this.Progress ?? 0) + (this.Deduct ?? 0) + (this.Vat ?? 0)) - ((this.DownPayment ?? 0) + (this.Consume ?? 0) + (this.Retention ?? 0) + (this.Tax ?? 0) + (this.ProgressVat ?? 0));
    }
}
