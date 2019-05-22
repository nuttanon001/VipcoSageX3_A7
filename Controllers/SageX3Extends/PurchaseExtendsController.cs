using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using VipcoSageX3.Helper;
using VipcoSageX3.Services;
using VipcoSageX3.ViewModels;
using VipcoSageX3.Models.SageX3Extends;
using VipcoSageX3.ViewModels.PurchasesModels;
using VipcoSageX3.Services.ExcelExportServices;

using RtfPipe;
using AutoMapper;
using System.Data;

namespace VipcoSageX3.Controllers.SageX3Extends
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchaseExtendsController : GenericSageX3ExtendController<PurchaseExtend>
    {
        private readonly IRepositorySageX3Extend<PurchaseLineExtend> repositoryPurchaseLine;
        private readonly IRepositoryDapperSageX3<PurchaseRequestPureViewModel> repositoryPrPure;
        private readonly IRepositoryDapperSageX3<PrOutStandingViewModel> repositoryPrOutStanding;

        // GET: api/PurchaseExtends
        public PurchaseExtendsController(IRepositorySageX3Extend<PurchaseExtend> repo,
            IRepositorySageX3Extend<PurchaseLineExtend> repoPurchaseLine,
            IRepositoryDapperSageX3<PurchaseRequestPureViewModel> repoPrPure,
            IRepositoryDapperSageX3<PrOutStandingViewModel> repoPrOutStanding,
            IMapper mapper,
            IHelperService helper,
            JsonSerializerService jsonService) : base(repo, mapper, helper, jsonService)
        {
            this.repositoryPurchaseLine = repoPurchaseLine;
            this.repositoryPrPure = repoPrPure;
            // Dapper
            this.repositoryPrOutStanding = repoPrOutStanding;
        }

        #region PrivateMethods
        private async Task<List<PrOutStandingViewModel>> GetPrOutStanding(ScrollViewModel Scroll)
        {
            if (Scroll != null)
            {
                var listPr = (await this.repository.GetToListAsync(x => new
                        {
                            x.PRNumber,
                            x.PrReceivedDate,
                            x.PrReceivedTime,
                            PrString = $"'{x.PRNumber}'"
                        },
                    x => x.PrReceivedDate.Value.Date >= Scroll.SDate.Value.Date && 
                         x.PrReceivedDate.Value.Date <= Scroll.EDate.Value.Date)).Distinct().ToList();
                
                if (listPr.Any())
                {
                    // ACC_0 ลูกหนี้ในประเทศ 113101 และ ลูกหนี้ต่างประเทศ 113201
                    string sWhere = "";
                    string sSort = "";

                    #region Where

                    // Where PR-Number
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"PRH.PSHNUM_0 IN ({string.Join(',', listPr.Select(x => x.PrString).ToList())})";

                    #endregion Where

                    #region Sort

                    sSort = $"PRH.PSHNUM_0,PRD.PSDLIN_0 ASC";

                    #endregion Sort

                    var sqlCommnad = new SqlCommandViewModel()
                    {
                        SelectCommand = $@"	PRD.PSHNUM_0 AS [PrNumber],
                                        PRD.PSDLIN_0 AS [PrLine],
                                        PRH.PJTH_0 AS [Project],
                                        PRH.PRQDAT_0 AS [PrDate],
                                        PRD.EXTRCPDAT_0 AS [RequestDate],
                                        PRH.ZPR11_0 AS [Other],
                                        PRH.ZPR30_0 AS [PrType],
                                        PRD.ITMREF_0 AS [ItemNo],
                                        PRD.ITMDES_0 AS [ItemName],
                                        TXT.TEXTE_0 AS [TextName],
                                        PRD.PUU_0 AS [Uom],
                                        DIM.CCE_0 AS [Branch],
                                        BOM.TEXTE_0 AS [WorkItem],
                                        DIM.CCE_2 AS [ProjectLine],
                                        WG.TEXTE_0 AS [WorkGroup],
                                        PRD.QTYPUU_0 AS [Quantity],
                                        PRH.CLEFLG_0 AS [StatusClose],
                                        PRH.ORDFLG_0 AS [StatusOrder],
                                        PRH.CREUSR_0 AS [CreateBy],
                                        ITM.ITMWEI_0 AS [ItemWeigth]",
                        FromCommand = $@" VIPCO.PREQUISD PRD
                                        LEFT OUTER JOIN VIPCO.PREQUIS PRH
                                        ON PRD.PSHNUM_0 = PRH.PSHNUM_0
                                        LEFT OUTER JOIN VIPCO.TEXCLOB TXT
                                        ON PRD.LINTEX_0 = TXT.CODE_0
                                        LEFT OUTER JOIN VIPCO.CPTANALIN DIM
                                        ON DIM.ABRFIC_0 = 'PSD'
                                            AND DIM.VCRTYP_0 = 0
                                            AND DIM.VCRSEQ_0 = 0
                                            AND DIM.CPLCLE_0 = ''
                                            AND DIM.ANALIG_0 = 1
                                            AND PRD.PSHNUM_0 = DIM.VCRNUM_0
                                            AND PRD.PSDLIN_0 = DIM.VCRLIN_0
                                        LEFT OUTER JOIN [VIPCO].[ATEXTRA] BOM
                                        ON DIM.CCE_1 = BOM.IDENT2_0
                                            AND BOM.ZONE_0 = 'LNGDES'
                                            AND BOM.IDENT1_0 = '3000'
                                        LEFT OUTER JOIN [VIPCO].[ATEXTRA] WG
                                        ON DIM.CCE_3 = WG.IDENT2_0
                                            AND WG.ZONE_0 = 'DESTRA'
                                            AND WG.IDENT1_0 = 'WG'
                                        LEFT OUTER JOIN [VIPCO].[ITMMASTER] ITM
                                        ON PRD.ITMREF_0 = ITM.ITMREF_0",
                        WhereCommand = sWhere,
                        OrderCommand = sSort
                    };

                    var dbData = await this.repositoryPrOutStanding.GetEntities(sqlCommnad);

                    // Get purchase request no
                    // New requirement 03/04/19

                    foreach (var item in dbData)
                    {
                        if (!string.IsNullOrEmpty(item.TextName))
                        {
                            if (item.TextName.StartsWith("{\\rtf1"))
                                item.TextName = Rtf.ToHtml(item.TextName);
                        }
                        else
                            item.TextName = item.ItemName;

                        // New requirement 03/04/19
                        var prExline = listPr.FirstOrDefault(x => x.PRNumber.ToLower() == item.PrNumber.ToLower());
                        if (prExline != null)
                            item.ReceivedDate = prExline.PrReceivedDate == null ? "" : prExline.PrReceivedDate.Value.ToString("dd/MM/yy ") + prExline.PrReceivedTime;
                        else
                        {
                            item.ReceivedDate = "";
                            item.PurchaseComment = "";
                        }
                    }
                    return dbData;
                }
            }
            return null;
        }

        #endregion

        #region Post Methods
        // POST: api/PurchaseExtends/GetScroll
        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel2 Scroll)
        {
            if (Scroll == null)
                return BadRequest();
            // Filter
            var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                : Scroll.Filter.Split(null);

            var predicate = PredicateBuilder.False<PurchaseExtend>();

            foreach (string temp in filters)
            {
                string keyword = temp.Trim().ToLower();
                predicate = predicate.Or(x => x.PRNumber.ToLower().Contains(keyword) ||
                                              x.Remark.ToLower().Contains(keyword));
            }
            if (!string.IsNullOrEmpty(Scroll.Where))
                predicate = predicate.And(p => p.Creator == Scroll.Where);

            //Order by
            Func<IQueryable<PurchaseExtend>, IOrderedQueryable<PurchaseExtend>> order;

            // Order
            switch (Scroll.SortField)
            {
                case "PRNumber":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.PRNumber);
                    else
                        order = o => o.OrderBy(x => x.PRNumber);
                    break;

                case "PrReceivedDate":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.PrReceivedDate);
                    else
                        order = o => o.OrderBy(x => x.PrReceivedDate);
                    break;

                case "Remark":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Remark);
                    else
                        order = o => o.OrderBy(x => x.Remark);
                    break;

                default:
                    order = o => o.OrderByDescending(x => x.PrReceivedDate);
                    break;
            }

            var QueryData = await this.repository.GetToListAsync(
                                    selector: x => new PurchaseExtend
                                    {
                                        PRNumber = x.PRNumber,
                                        PrReceivedDate = x.PrReceivedDate,
                                        PrReceivedTime = x.PrReceivedTime,
                                        PurchaseExtendId = x.PurchaseExtendId,
                                        Remark = x.Remark
                                    },  // Selected
                                    predicate: predicate, // Where
                                    orderBy: order, // Order
                                    include: null, // Include
                                    skip: Scroll.Skip ?? 0, // Skip
                                    take: Scroll.Take ?? 50); // Take
            // Get TotalRow
            Scroll.TotalRow = await this.repository.GetLengthWithAsync(predicate: predicate);

            //var mapDatas = new List<TaskStatusMaster>();
            //foreach (var item in QueryData)
            //{
            //    var MapItem = this.mapper.Map<TaskStatusMaster, OverTimeMasterViewModel>(item);
            //    mapDatas.Add(MapItem);
            //}

            return new JsonResult(new ScrollDataViewModel2<PurchaseExtend>(Scroll, QueryData), this.DefaultJsonSettings);
        }

        // POST: api/PurchaseExtends/
        [HttpPost()]
        public override async Task<IActionResult> Create([FromBody] PurchaseExtend record)
        {
            var message = "Data not been found";

            try
            {
                if (record != null)
                {
                    // record = this.helperService.AddHourMethod(record);

                    if (record.GetType().GetProperty("CreateDate") != null)
                        record.GetType().GetProperty("CreateDate").SetValue(record, DateTime.Now);

                    if (record.PurchaseLineExtends != null && record.PurchaseLineExtends.Any())
                    {
                        foreach (var item in record.PurchaseLineExtends)
                        {
                            item.CreateDate = DateTime.Now;
                            item.Creator = record.Creator;

                            if (string.IsNullOrEmpty(item.Remark))
                                item.Remark = record.Remark;
                        }
                    }

                    if (await this.repository.AddAsync(record) != null)
                        return new JsonResult(record, this.DefaultJsonSettings);
                }
            }
            catch (Exception ex)
            {
                message = $"Has error {ex.ToString()}";
            }

            return BadRequest(new { message });
        }

        // POST: api/PurchaseExtends/PurchaseExtendScroll
        [HttpPost("PurchaseExtendScroll")]
        public async Task<IActionResult> PurchaseExtendScroll([FromBody] ScrollViewModel Scroll)
        {
            var message = "Data not been found.";
            try
            {
                if (Scroll != null)
                {
                    string sWhere = "";
                    string sSort = "";

                    #region Where

                    var rowIds = await this.repository.GetToListAsync(x => x.PrSageHeaderId);
                    if (rowIds != null && rowIds.Any())
                    {
                        var rowid = string.Join(',', rowIds);
                        sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"PRH.ROWID NOT IN ({rowid})";
                    }

                    // Filter
                    var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                        : Scroll.Filter.Split(null);

                    foreach (string temp in filters)
                    {
                        if (string.IsNullOrEmpty(temp))
                            continue;

                        string keyword = temp.ToLower();
                        sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $@"(LOWER(PRH.PSHNUM_0) LIKE '%{keyword}%')";
                    }

                    #endregion Where

                    #region Sort

                    //switch(Scroll)
                    //{
                    //    case ScrollViewModel s when s.SortField == "Test":
                    //        break;
                    //}

                    switch (Scroll.SortField)
                    {
                        case "PrNumber":
                            if (Scroll.SortOrder == -1)
                                sSort = $"PRH.PSHNUM_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pshnum0);
                            else
                                sSort = $"PRH.PSHNUM_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pshnum0);
                            break;

                        case "PrDate":
                            if (Scroll.SortOrder == -1)
                                sSort = $"PRH.PRQDAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                            else
                                sSort = $"PRH.PRQDAT_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                            break;

                        default:
                            sSort = $"PRH.PRQDAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                            break;
                    }

                    #endregion Sort

                    #region Query

                    var sqlCommnad = new SqlCommandViewModel()
                    {
                        SelectCommand = $@"	PRH.ROWID AS [PrSageHeaderId],
                                            PRH.PSHNUM_0 AS [PrNumber],
                                            PRH.PRQDAT_0 AS [PrDate],
                                            (CASE 
                                                WHEN PRH.ZPR30_0 = 1 THEN 'Purchase'
                                                WHEN PRH.ZPR30_0 = 2 THEN 'Hire'
                                                ELSE '-'
                                            END) AS [PrType],
                                            (CASE
                                                WHEN PRH.CLEFLG_0 = 1 THEN 'No'
                                                WHEN PRH.CLEFLG_0 = 2 THEN 'Yes'
                                                ELSE '-'
                                            END) AS [StatusClose],
                                            (CASE
                                                WHEN PRH.ORDFLG_0 = 1 THEN 'No'
                                                WHEN PRH.ORDFLG_0 = 2 THEN 'Yes'
                                                ELSE '-'
                                            END) AS [StatusOrder]",
                        FromCommand = $@" [VIPCO].[PREQUIS] PRH ",
                        WhereCommand = sWhere,
                        OrderCommand = sSort
                    };

                    #endregion

                    var result = await this.repositoryPrPure.GetEntitiesAndTotal(sqlCommnad, new { Skip = Scroll.Skip ?? 0, Take = Scroll.Take ?? 50 });
                    var dbData = result.Entities;
                    Scroll.TotalRow = result.TotalRow;

                    return new JsonResult(
                        new ScrollDataViewModel<PurchaseRequestPureViewModel>(Scroll, dbData),
                        this.DefaultJsonSettings);
                }
            }
            catch (Exception ex)
            {
                message = $"Has error {ex.ToString()}";
            }

            return BadRequest(new { message });
        }
        #endregion

        // PUT: api/PurchaseExtends/
        [HttpPut]
        public override async Task<IActionResult> Update(int key, [FromBody] PurchaseExtend record)
        {
            var message = "Data not been found.";

            try
            {
                if (key > 0 && record != null)
                {
                    // +7 Hour
                    // record = this.helperService.AddHourMethod(record);

                    // Set date for CrateDate Entity
                    if (record.GetType().GetProperty("ModifyDate") != null)
                        record.GetType().GetProperty("ModifyDate").SetValue(record, DateTime.Now);

                    #region Set create or modified
                    // LiftingHasCheckLists
                    if (record.PurchaseLineExtends != null && record.PurchaseLineExtends.Any())
                    {
                        foreach (var item in record.PurchaseLineExtends)
                        {
                            if (item.PurchaseLineExtendId > 0)
                            {
                                item.CreateDate = DateTime.Now;
                                item.Creator = record.Modifyer;

                                if (string.IsNullOrEmpty(item.Remark))
                                    item.Remark = record.Remark;
                            }
                            else
                            {
                                item.ModifyDate = DateTime.Now;
                                item.Modifyer = record.Modifyer;
                            }
                        }
                    }

                    #endregion

                    if (await this.repository.UpdateAsync(record, key) != null)
                    {
                        if (record != null)
                        {
                            #region InsertOrUpdate Task Status Detail 
                            // filter
                            var dbPurchaseLines = await this.repositoryPurchaseLine.GetToListAsync(x => x, x => x.PurchaseExtendId == key);
                            //Remove TaskStatusDetails if edit remove it
                            foreach (var dbPurchaseLine in dbPurchaseLines)
                            {
                                if (!record.PurchaseLineExtends.Any(x => x.PurchaseLineExtendId == dbPurchaseLine.PurchaseLineExtendId))
                                    await this.repositoryPurchaseLine.DeleteAsync(dbPurchaseLine.PurchaseLineExtendId);
                            }
                            //Update TaskStatusDetails or Insert TaskStatusDetails
                            foreach (var uPurchaseLine in record.PurchaseLineExtends)
                            {
                                if (uPurchaseLine.PurchaseLineExtendId > 0)
                                    await this.repositoryPurchaseLine.UpdateAsync(uPurchaseLine, uPurchaseLine.PurchaseLineExtendId);
                                else
                                {
                                    uPurchaseLine.PurchaseExtendId = record.PurchaseExtendId;
                                    await this.repositoryPurchaseLine.AddAsync(uPurchaseLine);
                                }
                            }
                            #endregion
                        }
                    }
                }
                return new JsonResult(record, this.DefaultJsonSettings);
            }
            catch (Exception ex)
            {
                message = $"Has error {ex.ToString()}";
            }
            return BadRequest(new { message });
        }

        // DELETE: api/PurchaseExtends/
        [HttpDelete()]
        public override async Task<IActionResult> Delete(int key)
        {
            var message = "Data not been found.";
            try
            {
                if (key > 0)
                {
                    var purchaseLines = await this.repositoryPurchaseLine.
                        GetToListAsync(x => x.PurchaseLineExtendId, x => x.PurchaseExtendId == key);

                    if (purchaseLines.Any())
                    {
                        foreach (var item in purchaseLines.ToList())
                            await this.repositoryPurchaseLine.DeleteAsync(item);
                    }

                    if (await this.repository.DeleteAsync(key) == 0)
                        return BadRequest();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                message = $"Has error {ex.ToString()}";
            }

            return BadRequest(new { message });
        }

        #region Report

        [HttpPost("PrReportOnlyReceivedGetReport")]
        public async Task<IActionResult> PrReportOnlyReceivedGetReport([FromBody] ScrollViewModel Scroll)
        {
            var Message = "Data not been found.";
            try
            {
                if (Scroll != null)
                {
                    var MapDatas = await this.GetPrOutStanding(Scroll);

                    if (MapDatas.Any())
                    {
                        var table = new DataTable();
                        //Adding the Columns
                        foreach (var field in MapDatas[0].GetType().GetProperties()) // Loop through fields
                        {
                            string name = field.Name; // Get string name
                            var value = field.GetValue(MapDatas[0], null);

                            if (value is DateTime || value is double || value is int || value is null)
                                continue;

                            if (name == "ItemName" || name == "ProjectLine")
                                continue;

                            table.Columns.Add(field.Name.Replace("String", ""), typeof(string));
                        }

                        //Adding the Rows
                        // Table1
                        foreach (var item in MapDatas)
                        {
                            item.TextName = this.helperService.ConvertHtmlToText(item.TextName);
                            item.TextName = item.TextName.Replace("\r\n", "");
                            item.TextName = item.TextName.Replace("\n", "");

                            table.Rows.Add(
                                item.PrNumber,
                                item.Project,
                                item.PrDateString,
                                item.RequestDateString,
                                item.Other,
                                item.PrTypeString,
                                item.ReceivedDate,
                                item.ItemNo,
                                (string.IsNullOrEmpty(item.TextName) ? item.ItemName : item.TextName),
                                item.Uom,
                                item.Branch,
                                item.WorkItem,
                                item.WorkGroup,
                                item.QuantityString,
                                item.ItemWeigthString,
                                item.WeightPerQtyString,
                                item.StatusCloseString,
                                item.StatusOrderString,
                                item.CreateBy,
                                item.NowDateString,
                                item.DIFFString);
                        }

                        var file = this.helperService.CreateExcelFile(table, "PRReceived");
                        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                Message = $"Has error{ex.ToString()}";
            }
            return BadRequest(new { Error = Message });
        }

        #endregion
    }
}
