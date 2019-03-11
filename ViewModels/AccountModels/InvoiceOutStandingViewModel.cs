using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels
{
    public class InvoiceOutStandingViewModel
    {
        public string InvoiceNo { get; set; }
        public double? InvPriceInTax { get; set; }
        public string InvPriceInTaxString => 
            this.InvPriceInTax != null ? string.Format("{0:#,##0.00}", this.InvPriceInTax) : "0.0";
        public double? InvPriceExTax { get; set; }
        public string InvPriceExTaxString => 
            this.InvPriceExTax != null ? string.Format("{0:#,##0.00}", this.InvPriceExTax) : "0.0";
        public string Currency { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string Project { get; set; }
        public int? StatusClose { get; set; }
        public double? THB_TAX { get; set; }
        public string THB_TAXString => 
            this.THB_TAX != null ? string.Format("{0:#,##0.00}", this.THB_TAX) : "0.0";
        public double? USD_TAX { get; set; }
        public string USD_TAXString => 
            this.USD_TAX != null ? string.Format("{0:#,##0.00}", this.USD_TAX) : "0.0";
        public double? EUR_TAX { get; set; }
        public string EUR_TAXString => 
            this.EUR_TAX != null ? string.Format("{0:#,##0.00}", this.EUR_TAX) : "0.0";
        public double? THB { get; set; }
        public string THBString => 
            this.THB != null ? string.Format("{0:#,##0.00}", this.THB) : "0.0";
        public double? USD { get; set; }
        public string USDString => 
            this.USD != null ? string.Format("{0:#,##0.00}", this.USD) : "0.0";
        public double? EUR { get; set; }
        public string EURString => 
            this.EUR != null ? string.Format("{0:#,##0.00}", this.EUR) : "0.0";
        public DateTime? DueDate { get; set; }
        public string DueDateString => 
            this.DueDate != null ? this.DueDate.Value.ToString("dd/MM/yy") : "-";
        public DateTime? DocDate { get; set; }
        public string DocDateString => 
            this.DocDate != null ? this.DocDate.Value.ToString("dd/MM/yy") : "-";
        public DateTime? NowDate { get; set; }
        public string NowDateString => 
            this.NowDate != null ? this.NowDate.Value.ToString("dd/MM/yy") : "-";
        public double? DIFF { get; set; }
        public string DIFFString => 
            this.DIFF != null ? string.Format("{0:#,##0}", this.DIFF) : "0";
        public InvoiceStatus? InvoiceStatus { get; set; }
        public string InvoiceStatusString => 
            this.InvoiceStatus != null ? System.Enum.GetName(typeof(InvoiceStatus), this.InvoiceStatus) : "-";
    }

    public enum InvoiceStatus
    {
        OutStanding = 1,
        OverDue 
    }
}
