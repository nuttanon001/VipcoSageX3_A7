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

using AutoMapper;

namespace VipcoSageX3.Controllers.SageX3Extends
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchaseExtendsController : GenericSageX3ExtendController<PurchaseExtend>
    {
        private readonly IRepositorySageX3Extend<PurchaseLineExtend> repositoryPurchaseLine;
        private readonly IRepositoryDapperSageX3<PurchaseRequestPureViewModel> repositoryPrPure;

        // GET: api/PurchaseExtends
        public PurchaseExtendsController(IRepositorySageX3Extend<PurchaseExtend> repo,
            IRepositorySageX3Extend<PurchaseLineExtend> repoPurchaseLine,
            IRepositoryDapperSageX3<PurchaseRequestPureViewModel> repoPrPure,
            IMapper mapper,
            IHelperService helper,
            JsonSerializerService jsonService) : base(repo, mapper, helper, jsonService)
        {
            this.repositoryPurchaseLine = repoPurchaseLine;
            this.repositoryPrPure = repoPrPure;
        }

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
                        case  "PrNumber":
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

    }
}
