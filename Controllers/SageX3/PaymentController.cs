using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

using VipcoSageX3.Helper;
using VipcoSageX3.Services;
using VipcoSageX3.ViewModels;
using VipcoSageX3.Models.SageX3;
using VipcoSageX3.Services.ExcelExportServices;
//3rd Party
using RtfPipe;
using System.IO;
using AutoMapper;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;

namespace VipcoSageX3.Controllers.SageX3
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : GenericSageX3Controller<Paymenth>
    {
        private readonly IRepositorySageX3<Bank> repositoryBank;
        private readonly IRepositoryDapperSageX3<PaymentViewModel> repositoryPayment;
        private readonly IHelperService helperService;
        private readonly IHostingEnvironment hosting;
        public PaymentController(IRepositorySageX3<Paymenth> repo,
            IRepositoryDapperSageX3<PaymentViewModel> repoPayment,
            IRepositorySageX3<Bank> repoBank,
            IHelperService _helperService,
            IHostingEnvironment hosting,
            IMapper mapper) : base(repo, mapper)
        {
            this.repositoryBank = repoBank;
            // DapperSageX3
            this.repositoryPayment = repoPayment;
            // Helper
            this.helperService = _helperService;
            this.hosting = hosting;
        }

        private async Task<List<PaymentViewModel>> GetData(ScrollViewModel scroll)
        {
            if (scroll != null)
            {
                string sWhere = "";
                string sSort = "";
                string sQuery = "";

                #region Where

                var filters = string.IsNullOrEmpty(scroll.Filter) ? new string[] { "" }
                                : scroll.Filter.Split(null);
                foreach (string temp in filters)
                {
                    if (string.IsNullOrEmpty(temp))
                        continue;

                    string keyword = temp.ToLower();
                    sWhere += (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") +
                                                    $@"(LOWER(PAYM.CHQNUM_0) LIKE '%{keyword}%'
                                                        OR LOWER(PAYM.CUR_0) LIKE '%{keyword}%'
                                                        OR LOWER(PAYM.DES_0) LIKE '%{keyword}%'
                                                        OR LOWER(PAYM.NUM_0) LIKE '%{keyword}%'
                                                        OR LOWER(PAYM.REF_0) LIKE '%{keyword}%'
                                                        OR LOWER(PAYM.BPAINV_0) LIKE '%{keyword}%'
                                                        OR LOWER(PAYM.BPANAM_0) LIKE '%{keyword}%'
                                                        OR LOWER(PAYM.BPR_0) LIKE '%{keyword}%')";
                }

                // Bank
                if (!string.IsNullOrEmpty(scroll.WhereBank))
                    sWhere += (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"PAYM.BAN_0 = '{scroll.WhereBank}'";

                    //predicate = predicate.And(x => x.Ban0 == Scroll.WhereBank);

                // Supplier
                if (!string.IsNullOrEmpty(scroll.WhereSupplier))
                    sWhere += (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"PAYM.BPR_0 = '{scroll.WhereSupplier}'";
                    // predicate = predicate.And(x => x.Bpr0 == Scroll.WhereSupplier);

                if (scroll.WhereBanks.Any())
                {
                    var list = new List<string>();

                    foreach (var item in scroll.WhereBanks)
                        list.Add($"'{item}'");

                    var banks = string.Join(',', list);
                    sWhere += (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"PAYM.BAN_0 IN ({banks})";
                    // predicate = predicate.And(x => Scroll.WhereBanks.Contains(x.Ban0));
                }

                if (scroll.SDate.HasValue)
                {
                    sWhere +=
                       (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"PAYM.ACCDAT_0 >= '{scroll.SDate.Value.ToString("yyyy-MM-dd")}'";
                }

                if (scroll.EDate.HasValue)
                {
                    sWhere +=
                       (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"PAYM.ACCDAT_0 <= '{scroll.EDate.Value.ToString("yyyy-MM-dd")}'";
                }
               

                #endregion Where

                #region Sort

                switch (scroll.SortField)
                {
                    case "AmountString":
                        if (scroll.SortOrder == -1)
                            sSort = $"PAYM.AMTBAN_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pshnum0);
                        else
                            sSort = $"PAYM.AMTBAN_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pshnum0);
                        break;

                    case "BankNo":
                        if (scroll.SortOrder == -1)
                            sSort = $"PAYM.BAN_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pjth0);
                        else
                            sSort = $"PAYM.BAN_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pjth0);
                        break;

                    case "CheckNo":
                        if (scroll.SortOrder == -1)
                            sSort = $"PAYM.CHQNUM_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"PAYM.CHQNUM_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "Currency":
                        if (scroll.SortOrder == -1)
                            sSort = $"PAYM.CUR_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.prd.Itmdes0);
                        else
                            sSort = $"PAYM.CUR_0 ASC";//QueryData = QueryData.OrderBy(x => x.prd.Itmdes0);
                        break;

                    case "Description":
                        if (scroll.SortOrder == -1)
                            sSort = $"PAYM.DES_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Cce0);
                        else
                            sSort = $"PAYM.DES_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Cce0);
                        break;

                    case "PayBy":
                        if (scroll.SortOrder == -1)
                            sSort = $"PAYM.BPR_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Cce1);
                        else
                            sSort = $"PAYM.BPR_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Cce1);
                        break;

                    case "PaymentDateString":
                        if (scroll.SortOrder == -1)
                            sSort = $"PAYM.ACCDAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Cce3);
                        else
                            sSort = $"PAYM.ACCDAT_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Cce3);
                        break;

                    case "PaymentNo":
                        if (scroll.SortOrder == -1)
                            sSort = $"PAYM.NUM_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Cce3);
                        else
                            sSort = $"PAYM.NUM_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Cce3);
                        break;

                    case "SupplierNo":
                        if (scroll.SortOrder == -1)
                            sSort = $"PAYM.BPAINV_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Cce3);
                        else
                            sSort = $"PAYM.BPAINV_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Cce3);
                        break;

                    case "SupplierName":
                        if (scroll.SortOrder == -1)
                            sSort = $"PAYM.BPANAM_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Cce3);
                        else
                            sSort = $"PAYM.BPANAM_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Cce3);
                        break;

                    default:
                        sSort = $"PAYM.ACCDAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        break;
                }

                #endregion Sort

                #region Query
                // Query mulitple command
                sQuery = $@"SELECT	PAYM.NUM_0 AS PaymentNo,
                                    PAYM.ACCDAT_0 AS PaymentDate,
                                    PAYM.BAN_0 AS BankNo,
                                    BNK.DES_0 AS BankName,
                                    PAYM.BPR_0 AS PayBy,
                                    PAYM.CUR_0 AS Currency,
                                    PAYM.AMTBAN_0 AS Amount,
                                    PAYM.BANPAYTPY_0 AS Amount2,
                                    PAYM.DES_0 AS [Description],
                                    PAYM.CHQNUM_0 AS CheckNo,
                                    PAYM.BPAINV_0 AS SupplierNo,
                                    PAYM.BPANAM_0 AS SupplierName,
                                    PAYM.REF_0 AS RefNo
                            FROM	VIPCO.PAYMENTH PAYM
                                    LEFT OUTER JOIN VIPCO.BANK BNK
                                        ON PAYM.BAN_0 = BNK.BAN_0
                            {sWhere}
                            ORDER BY    {sSort}
                            OFFSET      @Skip ROWS       -- skip 10 rows
                            FETCH NEXT  @Take ROWS ONLY; -- take 10 rows;
                            SELECT	    COUNT(*)
                            FROM	    VIPCO.PAYMENTH PAYM
                                        LEFT OUTER JOIN VIPCO.BANK BNK
                                        ON PAYM.BAN_0 = BNK.BAN_0
                            {sWhere};";

                #endregion Query

                var result = await this.repositoryPayment.GetListEntitesAndTotalRow(sQuery, new { Skip = scroll.Skip ?? 0, Take = scroll.Take ?? 15 });
                var dbData = result.Entities;
                scroll.TotalRow = result.TotalRow;

                return dbData;
            }
            return null;
        }

        // POST: api/Payment/GetScroll
        //[Authorize(Policy = "SuperUser")]
        //[Authorize(Policy = "Admin")]
        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel Scroll)
        {
            var message = "Data not been found.";

            try
            {
                if (Scroll != null)
                {
                    var mapDatas = await this.GetData(Scroll);
                    return new JsonResult(new ScrollDataViewModel<PaymentViewModel>(Scroll, mapDatas), this.DefaultJsonSettings);
                }
            }
            catch(Exception ex)
            {
                message = $"Has error {ex.ToString()}";
            }
            return BadRequest(new { message });
        }

        //[Authorize(Policy = "SuperUser")]
        //[Authorize(Policy = "Admin")]
        [HttpPost("GetReport")]
        public async Task<IActionResult> GetReport([FromBody] ScrollViewModel Scroll)
        {
            var Message = "Data not been found";
            try
            {
                if (Scroll != null)
                {
                    var mapDatas = await this.GetData(Scroll);
                    if (mapDatas.Any())
                    {
                        var table = new DataTable();
                        //Adding the Columns
                        table.Columns.AddRange(new DataColumn[]
                        {
                        new DataColumn("PaymentDate", typeof(string)),
                        new DataColumn("PaymentNo.", typeof(string)),
                        new DataColumn("SupplierName",typeof(string)),
                        new DataColumn("BankAmount",typeof(string)),
                        new DataColumn("BankAmount2",typeof(string)),
                        new DataColumn("CheckNo",typeof(string)),
                        new DataColumn("Description",typeof(string)),
                        new DataColumn("BankName",typeof(string)),
                        new DataColumn("BankNo",typeof(string)),
                        new DataColumn("RefNo",typeof(string)),
                        new DataColumn("PayBy",typeof(string)),
                        new DataColumn("SupplierNo",typeof(string)),
                        });

                        //Adding the Rows
                        foreach (var item in mapDatas)
                        {
                            table.Rows.Add(
                                item.PaymentDateString,
                                item.PaymentNo,
                                item.SupplierName,
                                item.AmountString,
                                item.Amount2String,
                                item.CheckNo,
                                item.Description,
                                item.BankName,
                                item.BankNo,
                                item.RefNo,
                                item.PayBy,
                                item.SupplierNo
                            );
                        }
                        var report = this.helperService.CreateExcelFile(table, "PaymentReport");
                        return File(report, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export.xlsx");
                    }
                }
            }
            catch(Exception ex)
            {
                Message = $"Has error {ex.ToString()}";
            }

            return BadRequest(new { Message });
        }
    }
}
