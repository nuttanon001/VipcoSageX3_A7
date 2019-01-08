using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels
{
    public class PaymentViewModel
    {
        /// <summary>
        /// PAYMENTH.NUM_0
        /// </summary>
        public string PaymentNo { get; set; }
        /// <summary>
        /// PAYMENTH.ACCDAT_0
        /// </summary>
        public DateTime? PaymentDate { get; set; }
        public string PaymentDateString => this.PaymentDate?.ToString("dd/MM/yy") ?? "-";
        /// <summary>
        /// PAYMENTH.BAN_0
        /// </summary>
        public string BankNo { get; set; }
        /// <summary>
        /// BANK.DES_0
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// PAYMENTH.BPR_0
        /// </summary>
        public string PayBy { get; set; }
        /// <summary>
        /// PAYMENTH.CUR_0
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// PAYMENTH.AMTBAN_0
        /// </summary>
        public decimal? Amount { get; set; }
        public string AmountString => $"{this.Currency} " + this.Amount?.ToString("0,0.00") ?? "-";
        /// <summary>
        /// PAYMENTH.BANPAYTPY_0
        /// </summary>
        public decimal? Amount2 { get; set; }
        public string Amount2String => $"{this.Currency} " + (this.Amount2 * -1)?.ToString("0,0.00") ?? "-";
        /// <summary>
        /// PAYMENTH.DES_0
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// PAYMENTH.CHQNUM_0
        /// </summary>
        public string CheckNo { get; set; }
        /// <summary>
        /// PAYMENTH.BPAINV_0
        /// </summary>
        public string SupplierNo { get; set; }
        /// <summary>
        /// PAYMENTH.BPANAM_0
        /// </summary>
        public string SupplierName { get; set; }
        /// <summary>
        /// PAYMENTH.REF_0
        /// </summary>
        public string RefNo { get; set; }
    }
}
