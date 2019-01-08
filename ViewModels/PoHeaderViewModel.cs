using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.ViewModels
{
    public class PoHeaderViewModel
    {
        public decimal Rowid { get; set; }
        public string PurchaseOrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public string OrderDateString { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public string ProjectName { get; set; }
        public string ProjectCode { get; set; }
        public string ReceiveByCode { get; set; }
        public string ReceiveByName { get; set; }
        public string WorkGroupCode { get; set; }
        public string WorkGroupName { get; set; }
        public string WorkItemCode { get; set; }
        public string WorkItemName { get; set; }
        public string ShipTo { get; set; }
        public string TypePoHString { get; set; }
        public int? TypePoH { get; set; }
        public int? CloseStatus { get; set; }
        public string CloseStatusString { get; set; }
        public int? ReceivedStatus { get; set; }
        public string ReceivedStatusString { get; set; }
    }
}
