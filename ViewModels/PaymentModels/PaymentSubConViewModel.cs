using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels.PaymentModels
{
    public class PaymentSubConViewModel
    {
        public string PartnerNo { get; set; }
        public string Comment { get; set; }
        public string PartnerName { get; set; }
        public string PaymentNo { get; set; }
        public string Reference { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentDateString => this.PaymentDate != null ? this.PaymentDate.Value.ToString("dd/MM/yyyy") : "";
        public string Project { get; set; }
        public string Currency { get; set; }
        public string Attribute { get; set; }
        public string PayType { get; set; }
        public double? AmountProgress { get; set; }
        public string AmountProgressString => this.AmountProgress != null ? string.Format("{0:#,##0.00}", (this.AmountProgress > 0 ? this.AmountProgress : this.AmountProgress * -1)) : "0";
        public double? AmountDown { get; set; }
        public string AmountDownString => this.AmountDown != null ? string.Format("{0:#,##0.00}", (this.AmountDown > 0 ? this.AmountDown : this.AmountDown * -1)) : "0";
        public double? AmountConsume { get; set; }
        public string AmountConsumeString => this.AmountConsume != null ? string.Format("{0:#,##0.00}", (this.AmountConsume > 0 ? this.AmountConsume : this.AmountConsume * -1)) : "0";
        public double? AmountRetenion { get; set; }
        public string AmountRetenionString => this.AmountRetenion != null ? string.Format("{0:#,##0.00}", (this.AmountRetenion > 0 ? this.AmountRetenion : this.AmountRetenion * -1)) : "0";
        public double? AmountVat { get; set; }
        public string AmountVatString => this.AmountVat != null ? string.Format("{0:#,##0.00}", (this.AmountVat > 0 ? this.AmountVat : this.AmountVat * -1)) : "0";
        public double? AmountVat2 { get; set; }
        public string AmountVat2String => this.AmountVat2 != null ? string.Format("{0:#,##0.00}", (this.AmountVat2 > 0 ? this.AmountVat2 : this.AmountVat2 * -1)) : "0";
        public double? AmountTax { get; set; }
        public string AmountTaxString => this.AmountTax != null ? string.Format("{0:#,##0.00}", (this.AmountTax > 0 ? this.AmountTax : this.AmountTax * -1)) : "0";
        public double? AmountTax2 { get; set; }
        public string AmountTax2String => this.AmountTax2 != null ? string.Format("{0:#,##0.00}", (this.AmountTax2 > 0 ? this.AmountTax2 : this.AmountTax2 * -1)) : "0";
        public double? AmountDeduct { get; set; }
        public string AmountDeductString => this.AmountDeduct != null ? string.Format("{0:#,##0.00}", (this.AmountDeduct > 0 ? this.AmountDeduct : this.AmountDeduct * -1)) : "0";

    }
}
