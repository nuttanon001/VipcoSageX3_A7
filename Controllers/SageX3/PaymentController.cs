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
using VipcoSageX3.ViewModels.PaymentModels;


namespace VipcoSageX3.Controllers.SageX3
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : GenericSageX3Controller<Paymenth>
    {
        private readonly IRepositorySageX3<Bank> repositoryBank;
        private readonly IRepositoryDapperSageX3<PaymentViewModel> repositoryPayment;
        private readonly IRepositoryDapperSageX3<PaymentRetentionViewModel> repositoryRetention;
        private readonly ExcelWorkBookService excelWbService;
        private readonly IHelperService helperService;
        private readonly IHostingEnvironment hosting;
        public PaymentController(IRepositorySageX3<Paymenth> repo,
            IRepositoryDapperSageX3<PaymentViewModel> repoPayment,
            IRepositoryDapperSageX3<PaymentRetentionViewModel> repoRetention,
            IRepositorySageX3<Bank> repoBank,
            ExcelWorkBookService exWbService,
            IHelperService _helperService,
            IHostingEnvironment hosting,
            IMapper mapper) : base(repo, mapper)
        {
            this.repositoryBank = repoBank;
            // DapperSageX3
            this.repositoryPayment = repoPayment;
            this.repositoryRetention = repoRetention;
            // Helper
            this.excelWbService = exWbService;
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
        private async Task<List<PaymentRetentionViewModel>> GetDataRetentionSub(ScrollViewModel Scroll)
        {
            if (Scroll != null)
            {
                // ACC_0 212403 Only Retention
                string sWhere = "PYD.ACC_0 = '212403' AND PYD.DENCOD_0 IN ('GE' ,'RET')";
                string sSort = "";

                #region Where

                // Filter
                var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                    : Scroll.Filter.Split(null);

                foreach (string temp in filters)
                {
                    if (string.IsNullOrEmpty(temp))
                        continue;

                    string keyword = temp.ToLower();
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") +
                                                    $@"(LOWER(PYH.NUM_0) LIKE '%{keyword}%'
                                                        OR LOWER(PYH.BPR_0) LIKE '%{keyword}%'
                                                        OR LOWER(PYD.DESLIN_0) LIKE '%{keyword}%')
                                                        OR LOWER(PAN.BPRNAM_0) LIKE '%{keyword}%')";
                }

                // Where Customer
                if (Scroll.WhereBanks.Any())
                {
                    var list = new List<string>();

                    foreach (var item in Scroll.WhereBanks)
                        list.Add($"'{item}'");

                    var partners = string.Join(',', list);
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"PYH.BPR_0 IN ({partners})";
                }

                // Where Project
                if (Scroll.WhereProjects.Any())
                {
                    var list = new List<string>();

                    foreach (var item in Scroll.WhereProjects)
                        list.Add($"'{item}'");

                    var projects = string.Join(',', list);
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"PYA.CCE_2 IN ({projects})";
                }

                if (!string.IsNullOrEmpty(Scroll.WhereBranch))
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"LOWER(PYA.CCE_0) LIKE '%{Scroll.WhereBranch.ToLower()}%'";

                if (Scroll.SDate.HasValue)
                {
                    sWhere +=
                       (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"PYH.ACCDAT_0 >= '{Scroll.SDate.Value.ToString("yyyy-MM-dd")}'";
                }

                if (Scroll.EDate.HasValue)
                {
                    sWhere +=
                       (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"PYH.ACCDAT_0 <= '{Scroll.EDate.Value.ToString("yyyy-MM-dd")}'";
                }

                #endregion

                #region Sort

                switch (Scroll.SortField)
                {
                    case "PartnerNo":
                        if (Scroll.SortOrder == -1)
                            sSort = $"PYH.BPR_0  DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pshnum0);
                        else
                            sSort = $"PYH.BPR_0  ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pshnum0);
                        break;

                    case "PartnerName":
                        if (Scroll.SortOrder == -1)
                            sSort = $"PAN.BPRNAM_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pjth0);
                        else
                            sSort = $"PAN.BPRNAM_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pjth0);
                        break;

                    case "PaymentNo":
                        if (Scroll.SortOrder == -1)
                            sSort = $"PYH.NUM_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"PYH.NUM_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "PaymentDateString":
                        if (Scroll.SortOrder == -1)
                            sSort = $"PYH.ACCDAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"PYH.ACCDAT_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "DescriptionLine":
                        if (Scroll.SortOrder == -1)
                            sSort = $"PYD.DESLIN_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"PYD.DESLIN_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "Project":
                        if (Scroll.SortOrder == -1)
                            sSort = $"PYA.CCE_2 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"PYA.CCE_2 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    default:
                        sSort = $"PYA.CCE_2,PYH.ACCDAT_0 ASC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        break;
                }

                #endregion

                var sqlCommnad = new SqlCommandViewModel()
                {
                    SelectCommand = $@"	PYH.BPR_0 AS PartnerNo,
                                        PAN.BPRNAM_0 AS PartnerName,
                                        PYH.NUM_0 AS PaymentNo,
                                        PYH.ACCDAT_0 AS PaymentDate,
                                        PYD.DESLIN_0 AS DescriptionLine,
                                        PYA.CCE_0 AS Branch,
                                        PYA.CCE_1 AS WorkItem,
                                        PYA.CCE_2 AS Project,
                                        PYA.CCE_3 AS WorkGroup,
                                        PYD.COA_0 AS Currency,
                                        PYD.DENCOD_0 AS Attribute,
                                        (CASE WHEN PYD.DENCOD_0 = 'GE'
                                            THEN PYD.AMTLIN_0
                                            ELSE NULL END) AS AmountRetenion,
                                        (CASE WHEN PYD.DENCOD_0 = 'RET'
                                            THEN PYD.AMTLIN_0
                                            ELSE NULL END) AS AmountDeduct,
                                        '' AS Comment",
                    FromCommand = $@" [VIPCO].[PAYMENTH] PYH
                                        LEFT OUTER JOIN [VIPCO].[PAYMENTD] PYD
                                        ON PYH.NUM_0 = PYD.NUM_0
                                        LEFT OUTER JOIN [VIPCO].[PAYMENTA] PYA
                                        ON PYD.NUM_0 = PYA.NUM_0
                                            AND PYD.LIN_0 = PYA.LIN_0
                                        LEFT OUTER JOIN [VIPCO].[BPARTNER] PAN
                                        ON PYH.BPR_0 = PAN.BPRNUM_0 ",
                    WhereCommand = sWhere,
                    OrderCommand = sSort
                };

                var result = await this.repositoryRetention.GetEntitiesAndTotal(sqlCommnad, new { Skip = Scroll.Skip ?? 0, Take = Scroll.Take ?? 50 });
                var dbData = result.Entities;
                Scroll.TotalRow = result.TotalRow;

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


        [HttpPost("RetentionSubGetScroll")]
        public async Task<IActionResult> RetentionSubGetScroll([FromBody] ScrollViewModel Scroll)
        {
            var message = "Data not been found.";
            try
            {
                var MapDatas = await this.GetDataRetentionSub(Scroll);
                return new JsonResult(new ScrollDataViewModel<PaymentRetentionViewModel>(Scroll, MapDatas), this.DefaultJsonSettings);
            }
            catch (Exception ex)
            {
                message = $"Has error {ex.ToString()}";
            }
            return BadRequest(new { message });
        }

        // POST: api/Invoice/GetReport
        [HttpPost("RetentionSubGetReport")]
        public async Task<IActionResult> RetentionSubGetReport([FromBody] ScrollViewModel Scroll)
        {
            var Message = "Data not been found.";
            try
            {
                if (Scroll != null)
                {
                    var MapDatas = await this.GetDataRetentionSub(Scroll);

                    if (MapDatas.Any())
                    {
                        var memory = new MemoryStream();
                        using (var wb = this.excelWbService.Create())
                        {
                            var ws = wb.Worksheets.Add("RetentionSub");

                            ws.Cell(1, 1).Value = "RETENTION SUB CONTRACTOR";
                            ws.Cell(1, 1).DataType = XLDataType.Text;
                            ws.Range(1, 1, 1, 8).Merge().AddToNamed("Titles");

                            // Move to the next row (it now has the titles)
                            var hasData = MapDatas.GroupBy(z => new { z.PartnerNo, z.PartnerName }).ToList();

                            var indexRow = 2;
                            foreach (var partner in hasData.OrderBy(x => x.Key.PartnerName))
                            {
                                // Add Partner Name
                                ws.Cell(indexRow, 1).Value = $"{partner.Key.PartnerNo} | {partner.Key.PartnerName}";
                                ws.Range(indexRow, 1, indexRow, 8).Merge().AddToNamed("Titles");
                                ws.Row(indexRow).Height = 30;
                                // Plus index
                                indexRow += 2;

                                var jobs = partner.GroupBy(x => new { x.Project,x.Branch } ).ToList();
                                foreach (var job in jobs.OrderBy(x => x.Key.Project))
                                {
                                    // Add Group Job and Branch
                                    ws.Cell(indexRow, 1).Value = $"Job No: {job.Key.Project}/ Branch : {job.Key.Branch}";
                                    ws.Range(indexRow, 1, indexRow, 8).Merge().AddToNamed("Titles");
                                    // Plus index
                                    indexRow++;

                                    // Add list of data
                                    var rowData = job.Select(x => new
                                    {
                                        Date = x.PaymentDate,
                                        x.PaymentNo,
                                        job.Key.Branch,
                                        JobNo = job.Key.Project,
                                        Retention10Per = x.AmountRetenion >= 0 ? x.AmountRetenion : x.AmountRetenion * -1,
                                        DeductRetention = x.AmountDeduct >= 0 ? x.AmountDeduct : x.AmountDeduct * -1,
                                        Period = x.DescriptionLine,
                                        x.Comment
                                    }).ToList();
                                    // Add table by list of data
                                    var tableData = ws.Cell(indexRow, 1).InsertTable(rowData);
                                    tableData.ShowTotalsRow = true;
                                    // Set table header style
                                    var tableHeader = tableData.HeadersRow();
                                    tableHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    tableHeader.Style.Font.Bold = true;

                                    tableData.Field("Retention10Per").TotalsRowFunction = XLTotalsRowFunction.Sum;
                                    tableData.Field("DeductRetention").TotalsRowFunction = XLTotalsRowFunction.Sum;
                                    tableData.Field("Period").TotalsRowLabel = "Balance ";
                                    tableData.Field("Comment").TotalsRowFormulaA1 = "SUM([Retention10Per])-SUM([DeductRetention])";

                                    // Just for fun let's add the text "Sum Of Income" to the totals row
                                    tableData.Field(0).TotalsRowLabel = $"Total Retention of Job {job.Key.Project}";
                                    tableData.Theme = XLTableTheme.TableStyleMedium9;

                                    indexRow += tableData.RowCount() + 2;
                                }
                            }

                            // Prepare the style for the titles
                            var titlesStyle = wb.Style;
                            titlesStyle.Font.Bold = true;
                            titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            titlesStyle.Fill.BackgroundColor = XLColor.LightSteelBlue;
                            // Format all titles in one shot
                            wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;

                            ws.Columns(5, 8).Style.NumberFormat.Format = "#,##0.00";
                            ws.Columns().AdjustToContents();

                            wb.SaveAs(memory);
                        }

                        memory.Position = 0;
                        return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Retention.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                Message = $"Has error{ex.ToString()}";
            }
            return BadRequest(new { Error = Message });
        }
    }
}
