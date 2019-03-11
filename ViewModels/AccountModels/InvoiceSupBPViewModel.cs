using System;

namespace VipcoSageX3.ViewModels
{
    public class InvoiceSupBPViewModel
    {
        public string DocumentNo { get; set; }
        public DateTime? AccountDate { get; set; }
        public string AccountDateString => this.AccountDate == null ? "-" : this.AccountDate.Value.ToString("dd/MM/yyyy");

        public string InvType { get; set; }
        public string Site { get; set; }
        public string Supplier { get; set; }
        public string SupplierName { get; set; }
        public int? HeadAccountCode { get; set; }
        public string LineAccountCode { get; set; }
        public double? AmountTax { get; set; }
        public string AmountTaxString => this.AmountTax != null ? string.Format("{0:#,##0.00}", this.AmountTax) : "0.00";
        public string Tax { get; set; }
        public double? TaxAmount { get; set; }
        public string TaxAmountString => this.TaxAmount != null ? string.Format("{0:#,##0.00}", this.TaxAmount) : "0.00";
        public string Comment { get; set; }
        public string Project1 { get; set; }
        public string Project2 { get; set; }
        public string Branch { get; set; }
        public string Bom { get; set; }
        public string WorkGroup { get; set; }
        public string CostCenter { get; set; }
        public string Issued { get; set; }
        public string Title { get; set; }
        public string TaxinvNo { get; set; }
    }
}