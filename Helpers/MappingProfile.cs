using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using AutoMapper;
using VipcoSageX3.Models.SageX3;
using VipcoSageX3.Models.Machines;
using VipcoSageX3.ViewModels;

namespace VipcoSageX3.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region User

            //User
            CreateMap<User, UserViewModel>()
                // CuttingPlanNo
                .ForMember(x => x.NameThai,
                           o => o.MapFrom(s => s.EmpCodeNavigation == null ? "-" : $"คุณ{s.EmpCodeNavigation.NameThai}"))
                .ForMember(x => x.EmpCodeNavigation, o => o.Ignore());

            #endregion User

            #region Employee

            //Employee
            CreateMap<Employee, EmployeeViewModel>()
                .ForMember(x => x.User, o => o.Ignore())
                .ForMember(x => x.GroupMisNavigation, o => o.Ignore());

            #endregion

            #region EmployeeGroupMis

            CreateMap<EmployeeGroupMis, GroupMisViewModel>()
                .ForMember(x => x.Employee, o => o.Ignore());

            #endregion

            #region PurchaseOrder

            CreateMap<Porder, PoHeaderViewModel>()
                .ForMember(x => x.CloseStatus, o => o.MapFrom(s => s.Cleflg0))
                .ForMember(x => x.CloseStatusString, o => o.MapFrom(s => s.Cleflg0 == 1 ? "Not Close" : "Is Close"))
                .ForMember(x => x.OrderDate, o => o.MapFrom(s => s.Orddat0))
                .ForMember(x => x.OrderDateString, o => o.MapFrom(s => s.Orddat0.ToString("dd/MM/yy")))
                .ForMember(x => x.ProjectCode, o => o.MapFrom(s => s.Pjth0))
                .ForMember(x => x.PurchaseOrderNo, o => o.MapFrom(s => s.Pohnum0))
                .ForMember(x => x.ReceiveByCode, o => o.MapFrom(s => s.Zpo050))
                .ForMember(x => x.ReceivedStatus, o => o.MapFrom(s => s.Rcpflg0))
                .ForMember(x => x.ReceivedStatusString, o => o.MapFrom(s =>
                     s.Rcpflg0 == 2 ? "Partly" :
                         (s.Rcpflg0 == 3 ? "Completely" :
                             (s.Rcpflg0 == 4 ? "Not Managed" :
                                 (s.Rcpflg0 == 5 ? "Yes automatic" : "No Action")))))
                .ForMember(x => x.ShipTo, o => o.MapFrom(s => s.Zpo090))
                .ForMember(x => x.SupplierCode, o => o.MapFrom(s => s.Bpsinv0))
                .ForMember(x => x.SupplierName, o => o.MapFrom(s => s.Bponam0))
                .ForMember(x => x.TypePoH, o => o.MapFrom(s => s.Zpo210))
                .ForMember(x => x.TypePoHString, o => o.MapFrom(s =>
                     s.Zpo210 == 1 ? "จัดซื้อในประเทศ" :
                        (s.Zpo210 == 2 ? "จัดจ้าง" :
                            (s.Zpo210 == 3 ? "Oversea Purchasing" :
                                (s.Zpo210 == 4 ? "Mat Stock" :
                                    (s.Zpo210 == 5 ? "Surplus" : "Consumable Stock"))))))
                .ForMember(x => x.WorkGroupCode, o => o.MapFrom(s => s.Zpo020))
                .ForMember(x => x.WorkItemCode, o => o.MapFrom(s => s.Zpo010));

            #endregion

            #region PurchaseOrderDetail

            //CreateMap<Porderq, >()
            //    .ForMember(x => x.Rowid, o => o.MapFrom(s => s.Rowid))
            //    .ForMember(x => x.ProjectCode, o => o.MapFrom(s => s.Cce0))
            //    .ForMember(x => x.ProjectName, o => o.MapFrom(s => s.Des0));

            #endregion

            #region ProjectCode

            CreateMap<Cacce, ProjectCodeViewModel>()
                .ForMember(x => x.Rowid, o => o.MapFrom(s => s.Rowid))
                .ForMember(x => x.ProjectCode, o => o.MapFrom(s => s.Cce0))
                .ForMember(x => x.ProjectName, o => o.MapFrom(s => s.Des0));

            #endregion

            #region BomLevel

            CreateMap<Cacce, BomLevelViewModel>()
                .ForMember(x => x.Rowid, o => o.MapFrom(s => s.Rowid))
                .ForMember(x => x.BomLevelCode, o => o.MapFrom(s => s.Cce0))
                .ForMember(x => x.BomLevelName, o => o.MapFrom(s => s.Des0));

            #endregion

            #region WorkGroup

            CreateMap<Cacce, WorkGroupViewModel>()
                .ForMember(x => x.Rowid, o => o.MapFrom(s => s.Rowid))
                .ForMember(x => x.WorkGroupCode, o => o.MapFrom(s => s.Cce0))
                .ForMember(x => x.WorkGroupName, o => o.MapFrom(s => s.Des0));

            CreateMap<Atextra, WorkGroupViewModel>()
                .ForMember(x => x.Rowid, o => o.MapFrom(s => s.Rowid))
                .ForMember(x => x.WorkGroupCode, o => o.MapFrom(s => s.Ident20))
                .ForMember(x => x.WorkGroupName, o => o.MapFrom(s => s.Texte0));

            #endregion

            #region Payment

            CreateMap<Paymenth, PaymentViewModel>()
                .ForMember(x => x.RefNo, o => o.MapFrom(s => s.Ref0))
                .ForMember(x => x.PayBy, o => o.MapFrom(s => s.Bpr0))
                .ForMember(x => x.BankNo, o => o.MapFrom(s => s.Ban0))
                .ForMember(x => x.Currency, o => o.MapFrom(s => s.Cur0))
                .ForMember(x => x.Amount, o => o.MapFrom(s => s.Amtban0))
                .ForMember(x => x.PaymentNo, o => o.MapFrom(s => s.Num0))
                .ForMember(x => x.CheckNo, o => o.MapFrom(s => s.Chqnum0))
                .ForMember(x => x.Description, o => o.MapFrom(s => s.Des0))
                .ForMember(x => x.Amount2, o => o.MapFrom(s => s.Banpaytpy0))
                .ForMember(x => x.SupplierNo, o => o.MapFrom(s => s.Bpainv0))
                .ForMember(x => x.PaymentDate, o => o.MapFrom(s => s.Accdat0))
                .ForMember(x => x.SupplierName, o => o.MapFrom(s => s.Bpanam0));

            #endregion

            #region Bank
            CreateMap<Bank, BankViewModel>()
                .ForMember(x => x.BankNumber, o => o.MapFrom(s => s.Ban0))
                .ForMember(x => x.Description, o => o.MapFrom(s => s.Des0))
                .ForMember(x => x.Rowid, o => o.MapFrom(s => s.Rowid));
            #endregion

            #region Supplier
            CreateMap<Bpartner, SupplierViewModel>()
                .ForMember(x => x.SupplierName, o => o.MapFrom(s => s.Bprnam0))
                .ForMember(x => x.SupplierNo, o => o.MapFrom(s => s.Bprnum0))
                .ForMember(x => x.RowId, o => o.MapFrom(s => s.Rowid));
            #endregion

            #region PurchaseRequestAndOrder

            //.ForMember(x => x.PrNumber, o => o.MapFrom(s => s.Pshnum0))
            //.ForMember(x => x.PrLine, o => o.MapFrom(s => s.Psdlin0))
            //.ForMember(x => x.PurUom, o => o.MapFrom(s => s.Puu0))
            //.ForMember(x => x.StkUom, o => o.MapFrom(s => s.Stu0))
            //.ForMember(x => x.QuantityPur, o => o.MapFrom(s => s.Qtypuu0))
            //.ForMember(x => x.QuantityStk, o => o.MapFrom(s => s.Qtystu0))
            // PurchaseRequest Header
            CreateMap<Prequis, PurchaseRequestAndOrderViewModel>()
                // .ForMember(x => x.RequestDate,o => o.MapFrom(s => s.Prqdat0))
                .ForMember(x => x.PrCloseStatus, o => o.MapFrom(s => s.Cleflg0 == 1 ? "Not Close" : "Close"))
                .ForMember(x => x.CreateBy, o => o.MapFrom(s => s.Creusr0))
                .ForMember(x => x.PRDate, o => o.MapFrom(s => s.Prqdat0))
                .ForMember(x => x.PRDateString, o => o.MapFrom(s => s.Prqdat0.ToString("dd/MM/yy")));
            // PurchaseRequest Detail
            CreateMap<Prequisd, PurchaseRequestAndOrderViewModel>()
                // EXTRCPDAT 
                .ForMember(x => x.RequestDate,o => o.MapFrom(s => s.Extrcpdat0))
                .ForMember(x => x.PrNumber, o => o.MapFrom(s => s.Pshnum0))
                .ForMember(x => x.PrLine, o => o.MapFrom(s => s.Psdlin0))
                .ForMember(x => x.ItemCode, o => o.MapFrom(s => s.Itmref0))
                .ForMember(x => x.ItemName, o => o.MapFrom(s => s.Itmdes0))
                .ForMember(x => x.PurUom, o => o.MapFrom(s => s.Puu0))
                .ForMember(x => x.StkUom, o => o.MapFrom(s => s.Stu0))
                .ForMember(x => x.QuantityPur, o => o.MapFrom(s => s.Qtypuu0))
                .ForMember(x => x.QuantityStk, o => o.MapFrom(s => s.Qtystu0));
            // PurchaseRequest link PurchasOrder
            CreateMap<Prequiso, PurchaseRequestAndOrderViewModel>()
                .ForMember(x => x.LinkPoNumber, o => o.MapFrom(s => s.Pohnum0))
                .ForMember(x => x.LinkPoLine, o => o.MapFrom(s => s.Poplin0))
                .ForMember(x => x.LinkPoSEQ, o => o.MapFrom(s => s.Poqseq0));
            // PurchaseOrder Header
            CreateMap<Porder, PurchaseRequestAndOrderViewModel>()
                .ForMember(x => x.CloseStatus, o => o.MapFrom(s => s.Cleflg0 == 1 ? "Not Close" : "Close"))
                .ForMember(x => x.PoStatus, o => o.MapFrom(s =>
                    s.Zpo210 == 1 ? "จัดซื้อในประเทศ" :
                    (s.Zpo210 == 2 ? "จัดจ้าง" :
                        (s.Zpo210 == 3 ? "Oversea Purchasing" :
                            (s.Zpo210 == 4 ? "Mat Stock" :
                                (s.Zpo210 == 5 ? "Surplus" : "Consumable Stock"))))));
            // PurchaseOrder Detail
            CreateMap<Porderq, PurchaseRequestAndOrderViewModel>()
                .ForMember(x => x.PoNumber, o => o.MapFrom(s => s.Pohnum0))
                .ForMember(x => x.PoLine, o => o.MapFrom(s => s.Poplin0))
                .ForMember(x => x.PoSequence, o => o.MapFrom(s => s.Poqseq0))
                .ForMember(x => x.PoDate, o => o.MapFrom(s => s.Orddat0))
                .ForMember(x => x.PoDateString, o => o.MapFrom(s => s.Orddat0.ToString("dd/MM/yy")))
                .ForMember(x => x.DueDate, o => o.MapFrom(s => s.Extrcpdat0))
                .ForMember(x => x.DueDateString, o => o.MapFrom(s => s.Extrcpdat0.ToString("dd/MM/yy")))
                .ForMember(x => x.PoPurUom, o => o.MapFrom(s => s.Puu0))
                .ForMember(x => x.PoStkUom, o => o.MapFrom(s => s.Stu0))
                .ForMember(x => x.PoQuantityPur, o => o.MapFrom(s => s.Qtypuu0))
                .ForMember(x => x.PoQuantityStk, o => o.MapFrom(s => s.Qtystu0))
                .ForMember(x => x.PoQuantityWeight, o => o.MapFrom(s => s.Qtyweu0));
            #endregion

            #region PurchaseReceipt
            CreateMap<Preceiptd, PurchaseReceiptViewModel>()
                .ForMember(x => x.RcNumber, o => o.MapFrom(s => s.Pthnum0))
                .ForMember(x => x.RcLine, o => o.MapFrom(s => s.Ptdlin0))
                .ForMember(x => x.RcDate, o => o.MapFrom(s => s.Rcpdat0))
                //.ForMember(x => x.RcDateString, o => o.MapFrom(s => s.Rcpdat0.ToString("dd/MM/yy")))
                .ForMember(x => x.RcPurUom, o => o.MapFrom(s => s.Puu0))
                .ForMember(x => x.RcStkUom, o => o.MapFrom(s => s.Stu0))
                .ForMember(x => x.RcUom, o => o.MapFrom(s => s.Uom0))
                .ForMember(x => x.RcQuantityPur, o => o.MapFrom(s => s.Qtypuu0))
                .ForMember(x => x.RcQuantityStk, o => o.MapFrom(s => s.Qtystu0))
                .ForMember(x => x.RcQuantityUom, o => o.MapFrom(s => s.Qtyuom0))
                .ForMember(x => x.RcQuantityWeight, o => o.MapFrom(s => s.Qtyweu0))
                .ForMember(x => x.RcQuantityInvPur, o => o.MapFrom(s => s.Invqtypuu0))
                .ForMember(x => x.RcQuantityInvStk, o => o.MapFrom(s => s.Invqtystu0));
            #endregion

            #region StockMovement

            #endregion
        }
    }
}
