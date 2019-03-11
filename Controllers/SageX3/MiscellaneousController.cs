using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RtfPipe;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using VipcoSageX3.Models.SageX3;
using VipcoSageX3.Services;
using VipcoSageX3.ViewModels;
using VipcoSageX3.Services.ExcelExportServices;

namespace VipcoSageX3.Controllers.SageX3
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MiscellaneousController : GenericSageX3Controller<Stojou>
    {
        private readonly IRepositoryDapperSageX3<MiscAndAcountViewModel> repositoryMiscAndAcc;
        private readonly IRepositoryDapperSageX3<IssueViewModel> repositoryIssue;
        private readonly IRepositoryDapperSageX3<JournalViewModel> repositoryJournal;
        private readonly IHelperService helperService;

        // GET: api/Miscellaneous
        public MiscellaneousController(
            IRepositorySageX3<Stojou> repo,
            IRepositoryDapperSageX3<MiscAndAcountViewModel> repoMiscAndAcc,
            IRepositoryDapperSageX3<IssueViewModel> repoIssue,
            IRepositoryDapperSageX3<JournalViewModel> repoJournal,
            IHelperService _helperService,
            IMapper mapper)
            : base(repo, mapper)
        {
            //Repository
            this.repositoryMiscAndAcc = repoMiscAndAcc;
            this.repositoryIssue = repoIssue;
            this.repositoryJournal = repoJournal;
            //Helper
            this.helperService = _helperService;
        }

        private async Task<List<MiscAndAcountViewModel>> GetData(ScrollViewModel scroll, bool option = false)
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
                                                    $@"(LOWER(SMH.VCRNUM_0) LIKE '%{keyword}%'
                                                        OR LOWER(SMH.VCRDES_0) LIKE '%{keyword}%'
                                                        OR LOWER(ACH.NUM_0) LIKE '%{keyword}%'
                                                        OR LOWER(ACH.BPRVCR_0) LIKE '%{keyword}%')";
                }

                if (!string.IsNullOrEmpty(scroll.WhereProject))
                    sWhere += (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"SMH.PJT_0 = '{scroll.WhereProject}'";

                if (scroll.SDate.HasValue)
                    sWhere +=
                        (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"SMH.IPTDAT_0 >= '{scroll.SDate.Value.ToString("yyyy-MM-dd")}'";

                if (scroll.EDate.HasValue)
                    sWhere +=
                        (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"SMH.IPTDAT_0 <= '{scroll.EDate.Value.ToString("yyyy-MM-dd")}'";

                #endregion Where

                #region Sort

                switch (scroll.SortField)
                {
                    case "MiscNumber":
                        if (scroll.SortOrder == -1)
                            sSort = $"SMH.VCRNUM_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.SMH.Pshnum0);
                        else
                            sSort = $"SMH.VCRNUM_0 ASC";//QueryData = QueryData.OrderBy(x => x.SMH.Pshnum0);
                        break;

                    case "MiscDateString":
                        if (scroll.SortOrder == -1)
                            sSort = $"SMH.IPTDAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.SMH.Pjth0);
                        else
                            sSort = $"SMH.IPTDAT_0 ASC";//QueryData = QueryData.OrderBy(x => x.SMH.Pjth0);
                        break;

                    case "ProjectCode":
                        if (scroll.SortOrder == -1)
                            sSort = $"SMH.PJT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.SMH.Prqdat0);
                        else
                            sSort = $"SMH.PJT_0 ASC";//QueryData = QueryData.OrderBy(x => x.SMH.Prqdat0);
                        break;

                    case "Description":
                        if (scroll.SortOrder == -1)
                            sSort = $"SMH.VCRDES_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.prd.Itmdes0);
                        else
                            sSort = $"SMH.VCRDES_0 ASC";//QueryData = QueryData.OrderBy(x => x.prd.Itmdes0);
                        break;

                    case "AccNumber":
                        if (scroll.SortOrder == -1)
                            sSort = $"ACH.NUM_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.SMH.Cce0);
                        else
                            sSort = $"ACH.NUM_0 ASC";//QueryData = QueryData.OrderBy(x => x.SMH.Cce0);
                        break;

                    case "AccDateString":
                        if (scroll.SortOrder == -1)
                            sSort = $"ACH.ACCDAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.SMH.Cce1);
                        else
                            sSort = $"ACH.ACCDAT_0 ASC";//QueryData = QueryData.OrderBy(x => x.SMH.Cce1);
                        break;

                    case "AccIssue":
                        if (scroll.SortOrder == -1)
                            sSort = $"ACH.DESVCR_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.SMH.Cce3);
                        else
                            sSort = $"ACH.DESVCR_0 ASC";//QueryData = QueryData.OrderBy(x => x.SMH.Cce3);
                        break;

                    default:
                        sSort = $"SMH.IPTDAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.SMH.Prqdat0);
                        break;
                }

                #endregion Sort

                #region Query
                // Query mulitple command
                sQuery = $@"SELECT	    SMH.VCRNUM_0 AS [MiscNumber],
                                        SMH.IPTDAT_0 AS [MiscDate],
                                        SMH.PJT_0 AS [Project],
                                        SMH.VCRDES_0 AS [Description],
                                        ACH.NUM_0 AS [AccNumber],
                                        ACH.ACCDAT_0 AS [AccDate],
                                        ACH.BPRVCR_0 AS [MiscLink],
                                        ACH.JOU_0 AS [AccType],
                                        ACH.CAT_0 AS [AccCat],
                                        ACH.DESVCR_0 AS [AccIssue]
                            FROM	    VIPCO.SMVTH SMH
                                        INNER JOIN VIPCO.GACCENTRY ACH WITH(INDEX(GACCENTRY_ROWID))
                                            ON SMH.VCRNUM_0 = ACH.BPRVCR_0
                                            AND ACH.DESVCR_0 = 'Miscellaneous issue'
                                            AND ACH.JOU_0 = 'STOCK'
                            {sWhere}
                            ORDER BY    {sSort}
                            OFFSET      @Skip ROWS       -- skip 10 rows
                            FETCH NEXT  @Take ROWS ONLY; -- take 10 rows;
                            SELECT	    COUNT(*)
                            FROM	    VIPCO.SMVTH SMH
                                        INNER JOIN VIPCO.GACCENTRY ACH WITH(INDEX(GACCENTRY_ROWID))
                                            ON SMH.VCRNUM_0 = ACH.BPRVCR_0
                                            AND ACH.DESVCR_0 = 'Miscellaneous issue'
                                            AND ACH.JOU_0 = 'STOCK'
                            {sWhere};";

                #endregion Query

                var result = await this.repositoryMiscAndAcc.GetListEntitesAndTotalRow(sQuery, new { Skip = scroll.Skip ?? 0, Take = scroll.Take ?? 15 });
                var dbData = result.Entities;
                scroll.TotalRow = result.TotalRow;

                string sIssue = "";
                string sJournal = "";
                foreach (var item in dbData)
                {
                    #region Issue

                    sIssue = $@"SELECT	    SMD.VCRLIN_0 AS [MiscLine],
                                            SMD.ITMREF_0 AS [ItemCode],
                                            SMD.ITMDES1_0 AS [ItemName],
                                            SMD.PCU_0 AS [Uom],
                                            SJU.QTYPCU_0 * -1 AS [Qty],
                                            SJU.CCE_0 AS [Branch],
                                            SJU.CCE_1 AS [WorkItem],
                                            SJU.CCE_2 AS [Project],
                                            SJU.CCE_3 AS [WorkGroup],
                                            ITM.PURTEX_0 AS [ItemNameREF],
                                            TXT.TEXTE_0 AS [ItemNameRFT]
                                FROM	    VIPCO.SMVTD SMD
                                            LEFT OUTER JOIN VIPCO.ITMMASTER ITM
                                                ON ITM.ITMREF_0 = SMD.ITMREF_0
                                            LEFT OUTER JOIN VIPCO.STOJOU SJU WITH(INDEX(STOJOU_ROWID))
                                                ON SMD.VCRNUM_0 = SJU.VCRNUM_0
                                                AND SMD.VCRLIN_0 = SJU.VCRLIN_0
                                                AND SJU.TRSTYP_0 = 2
                                                AND SJU.REGFLG_0 = 1
                                            LEFT JOIN VIPCO.TEXCLOB TXT ON TXT.CODE_0 = ITM.PURTEX_0
                                WHERE	    SMD.VCRTYP_0 = 20 AND SMD.VCRNUM_0 = @MiscNumber
                                ORDER BY    SMD.VCRLIN_0";

                    var issues = await this.repositoryIssue.GetListEntites(sIssue, new { item.MiscNumber });
                    issues.GroupBy(x => new {
                        x.Uom,
                        x.WorkGroup,
                        x.WorkItem,
                        x.Project,
                        x.ItemCode,
                        x.ItemNameRFT,
                        x.MiscLine,
                        x.Branch,
                        x.ItemName
                    }).ToList().ForEach(issue =>
                    {
                        var newIssue = new IssueViewModel()
                        {
                            Branch = issue.Key.Branch,
                            ItemCode = issue.Key.ItemCode,
                            ItemName = issue.Key.ItemName,
                            ItemNameRFT = issue.Key.ItemNameRFT,
                            MiscLine = issue.Key.MiscLine,
                            Project = issue.Key.Project,
                            Qty = issue.Sum(x => x.Qty ?? 0),
                            Uom = issue.Key.Uom,
                            WorkGroup = issue.Key.WorkGroup,
                            WorkItem = issue.Key.WorkItem
                        };

                        if (string.IsNullOrEmpty(newIssue.ItemNameRFT))
                            newIssue.ItemNameRFT = newIssue.ItemName;
                        else
                        {
                            if (newIssue.ItemNameRFT.StartsWith("{\\rtf1") && !option)
                                newIssue.ItemNameRFT = Rtf.ToHtml(newIssue.ItemNameRFT);
                        }
                      
                         item.Issues.Add(newIssue);
                    });

                    #endregion Issue

                    #region Journal

                    if (!string.IsNullOrEmpty(item.AccNumber))
                    {
                        sJournal = $@"SELECT	ACA.LIN_0 AS [AccLine],
                                                ACA.SNS_0 AS [CurType],
                                                ACA.AMTCUR_0 AS [AmountCurrency],
                                                ACA.ACC_0 AS [AccountCode],
                                                ACA.CCE_0 AS [Branch],
                                                ACA.CCE_1 AS [WorkItem],
                                                ACA.CCE_2 AS [Project],
                                                ACA.CCE_3 AS [WorkGroup],
                                                ACD.ACCNUM_0 AS [AccountNumber],
                                                ACD.DES_0 AS [Description],
                                                ACD.FREREF_0 AS [FreeREF]
                                    FROM	    VIPCO.GACCENTRYA ACA
                                                LEFT OUTER JOIN VIPCO.GACCENTRYD ACD
                                                    ON ACA.NUM_0 = ACD.NUM_0
                                                    AND ACA.TYP_0 = ACD.TYP_0
                                                    AND ACA.LIN_0 = ACD.LIN_0
                                                    AND ACA.LEDTYP_0 = ACD.LEDTYP_0
                                    WHERE       ACA.NUM_0 = @AccNumber
                                    ORDER BY    ACA.CCE_2,ACA.LIN_0";

                        var journals = await this.repositoryJournal.GetListEntites(sJournal, new { item.AccNumber });
                        journals.ForEach(journal => item.Journals.Add(journal));
                    }

                    #endregion Journal
                }

                return dbData;
            }
            return null;
        }

        // POST: api/Miscellaneous/GetScroll
        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            var Message = "";
            try
            {
                var MapDatas = await this.GetData(Scroll);
                return new JsonResult(new ScrollDataViewModel<MiscAndAcountViewModel>(Scroll, MapDatas), this.DefaultJsonSettings);
            }
            catch (Exception ex)
            {
                Message = $"{ex.ToString()}";
            }
            return BadRequest();
        }

        // POST: api/Miscellaneous/GetReport
        [HttpPost("GetReport")]
        public async Task<IActionResult> GetReport([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            var Message = "Data not been found.";
            try
            {
                var MapDatas = await this.GetData(Scroll);

                if (MapDatas.Any())
                {
                    var table = new DataTable();
                    var table2 = new DataTable();
                    //Adding the Columns
                    table.Columns.AddRange(new DataColumn[]
                    {
                        new DataColumn("IssueNo", typeof(string)),
                        new DataColumn("IssueDate", typeof(string)),
                        new DataColumn("Description",typeof(string)),
                        new DataColumn("Line",typeof(string)),
                        new DataColumn("Code",typeof(string)),
                        new DataColumn("Name",typeof(string)),
                        new DataColumn("Qty",typeof(string)),
                        new DataColumn("Uom",typeof(string)),
                        new DataColumn("Branch",typeof(string)),
                        new DataColumn("Bom",typeof(string)),
                        new DataColumn("Project",typeof(string)),
                        new DataColumn("WorkGroup",typeof(string)),
                    });

                    table2.Columns.AddRange(new DataColumn[]
                    {
                        new DataColumn("IssueNo", typeof(string)),
                        new DataColumn("JournalNo", typeof(string)),
                        new DataColumn("JournalDate", typeof(string)),
                        new DataColumn("AccType",typeof(string)),
                        new DataColumn("AccRef",typeof(string)),
                        new DataColumn("Line",typeof(string)),
                        new DataColumn("Code",typeof(string)),
                        new DataColumn("Currency",typeof(string)),
                        new DataColumn("AccNumber",typeof(string)),
                        new DataColumn("Desc",typeof(string)),
                        new DataColumn("Branch",typeof(string)),
                        new DataColumn("Bom",typeof(string)),
                        new DataColumn("Project",typeof(string)),
                        new DataColumn("WorkGroup",typeof(string)),
                    });

                    //Adding the Rows
                    // Table1
                    foreach (var item in MapDatas.GroupBy(x => x.MiscNumber))
                    {
                        foreach (var item2 in item)
                        {
                            foreach (var item3 in item2.Issues.GroupBy(x =>
                            new {
                                x.Uom,
                                x.WorkGroup,
                                x.WorkItem,
                                x.Project,
                                x.ItemCode,
                                x.ItemNameRFT,
                                x.MiscLine,
                                x.Branch
                            }))
                            {
                                var name = this.helperService.ConvertHtmlToText(item3.Key.ItemNameRFT);
                                name = name.Replace("\r\n", "");
                                name = name.Replace("\n", "");

                                table.Rows.Add(
                                  item2.MiscNumber,
                                  item2.MiscDateString,
                                  item2.Description,
                                  item3.Key.MiscLine,
                                  item3.Key.ItemCode,
                                  name,
                                  string.Format("{0:#,##0.00}", item3.Sum(x => x.Qty ?? 0)),
                                  item3.Key.Uom,
                                  item3.Key.Branch,
                                  item3.Key.WorkItem,
                                  item3.Key.Project,
                                  item3.Key.WorkGroup
                              );
                            }
                        }
                    }

                    // Table2
                    foreach (var item in MapDatas.GroupBy(x => x.AccNumber))
                    {
                        foreach (var item2 in item)
                        {
                            foreach (var item3 in item2.Journals)
                            {
                                table2.Rows.Add(
                                  item2.MiscLink,
                                  item2.AccNumber,
                                  item2.AccDateString,
                                  item2.AccType,
                                  item2.AccIssue,
                                  item3.AccLine,
                                  item3.AccountCode,
                                  item3.AmountCurrencyString,
                                  item3.AccountNumber,
                                  item3.Description,
                                  item3.Branch,
                                  item3.WorkItem,
                                  item3.Project,
                                  item3.WorkGroup
                              );
                            }
                        }
                    }

                    var temp = new List<MuiltSheetViewModel>();

                    var file = this.helperService.CreateExcelFileMuiltSheets(new List<MuiltSheetViewModel>
                    {
                        new MuiltSheetViewModel{Tables = table,SheetName = "Issue" },
                        new MuiltSheetViewModel{Tables = table2,SheetName = "Journal" },
                    });

                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Journal.xlsx");
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