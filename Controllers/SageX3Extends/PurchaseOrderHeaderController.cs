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
    // GET: api/PurchaseOrderHeader
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderHeaderController : GenericSageX3ExtendController<PurchaseOrderHeader>
    {
        private readonly IRepositorySageX3Extend<PurchaseExtend> repositoryPurchase;
        private readonly IRepositorySageX3Extend<PurchaseLineExtend> repositoryPurchaseLine;

        public PurchaseOrderHeaderController(IRepositorySageX3Extend<PurchaseOrderHeader> repo,
            IRepositorySageX3Extend<PurchaseExtend> repoPurchase,
            IRepositorySageX3Extend<PurchaseLineExtend> repoPurchaseLine,
            IMapper mapper,
            IHelperService helper,
            JsonSerializerService jsonService) : base(repo, mapper, helper, jsonService)
        {
            this.repositoryPurchase = repoPurchase;
            this.repositoryPurchaseLine = repoPurchaseLine;
        }

        // POST: api/PurchaseOrderHeader/GetScroll
        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel2 Scroll)
        {
            if (Scroll == null)
                return BadRequest();
            // Filter
            var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                : Scroll.Filter.Split(null);

            var predicate = PredicateBuilder.False<PurchaseOrderHeader>();

            foreach (string temp in filters)
            {
                string keyword = temp.Trim().ToLower();
                predicate = predicate.Or(x => x.Remark.ToLower().Contains(keyword));
            }
            if (!string.IsNullOrEmpty(Scroll.Where))
                predicate = predicate.And(p => p.Creator == Scroll.Where);
            //Order by
            Func<IQueryable<PurchaseOrderHeader>, IOrderedQueryable<PurchaseOrderHeader>> order;
            // Order
            switch (Scroll)
            {
                case ScrollViewModel2 s when s.SortField == "PrReceivedDate":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.PrReceivedDate);
                    else
                        order = o => o.OrderBy(x => x.PrReceivedDate);
                    break;
              
                default:
                    order = o => o.OrderByDescending(x => x.PrReceivedDate);
                    break;
            }
            var QueryData = await this.repository.GetToListAsync(
                                    selector: x => new PurchaseOrderHeader
                                    {
                                        PrReceivedDate = x.PrReceivedDate,
                                        PrReceivedTime = x.PrReceivedTime,
                                        PurchaseOrderHeaderId = x.PurchaseOrderHeaderId,
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

            return new JsonResult(new ScrollDataViewModel2<PurchaseOrderHeader>(Scroll, QueryData), this.DefaultJsonSettings);
        }

        // POST: api/PurchaseOrderHeader/
        [HttpPost()]
        public override async Task<IActionResult> Create([FromBody] PurchaseOrderHeader record)
        {
            var message = "Data not been found";

            try
            {
                if (record != null)
                {
                    // record = this.helperService.AddHourMethod(record);

                    if (record.GetType().GetProperty("CreateDate") != null)
                        record.GetType().GetProperty("CreateDate").SetValue(record, DateTime.Now);

                    if (record.PurchaseExtends != null && record.PurchaseExtends.Any())
                    {
                        foreach (var item in record.PurchaseExtends)
                        {
                            item.CreateDate = DateTime.Now;
                            item.Creator = record.Creator;

                            /*
                            //Set value from overheader
                            if (!string.IsNullOrEmpty(item.Remark))
                                item.Remark = record.Remark;
                            // Set date and time
                            if (item.PrReceivedDate == null)
                            {
                                item.PrReceivedDate = record.PrReceivedDate;
                                item.PrReceivedTime = record.PrReceivedTime;
                            }
                            */
                            item.Remark = record.Remark;
                            item.PrReceivedDate = record.PrReceivedDate;
                            item.PrReceivedTime = record.PrReceivedTime;

                            if (item.PurchaseLineExtends != null && item.PurchaseLineExtends.Any())
                            {
                                foreach (var itemLine in item.PurchaseLineExtends)
                                {
                                    itemLine.CreateDate = DateTime.Now;
                                    itemLine.Creator = record.Creator;
                                    itemLine.Remark = item.Remark;
                                }
                            }
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

        // PUT: api/PurchaseOrderHeader/
        [HttpPut]
        public override async Task<IActionResult> Update(int key, [FromBody] PurchaseOrderHeader record)
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

                    if (record.PurchaseExtends != null && record.PurchaseExtends.Any())
                    {
                        foreach (var item in record.PurchaseExtends)
                        {
                            if (item.PurchaseExtendId > 0)
                            {
                                item.CreateDate = DateTime.Now;
                                item.Creator = record.Modifyer;
                            }
                            else
                            {
                                item.ModifyDate = DateTime.Now;
                                item.Modifyer = record.Modifyer;
                            }

                            //Set value from overheader
                            item.Remark = record.Remark;
                            // Set date and time
                            item.PrReceivedDate = record.PrReceivedDate;
                            item.PrReceivedTime = record.PrReceivedTime;

                            // LiftingHasCheckLists
                            if (item.PurchaseLineExtends != null && item.PurchaseLineExtends.Any())
                            {
                                foreach (var itemLine in item.PurchaseLineExtends)
                                {
                                    if (itemLine.PurchaseLineExtendId > 0)
                                    {
                                        itemLine.CreateDate = DateTime.Now;
                                        itemLine.Creator = record.Modifyer;
                                        itemLine.Remark = item.Remark;
                                    }
                                    else
                                    {
                                        itemLine.ModifyDate = DateTime.Now;
                                        itemLine.Modifyer = record.Modifyer;
                                    }
                                }
                            }
                        }
                    }
                    
                    #endregion

                    if (await this.repository.UpdateAsync(record, key) != null)
                    {
                        if (record != null)
                        {
                            #region InsertOrUpdate Purchase extend
                            // Remove PurchaseExtend not in purchaseOverHeader
                            var dbPurchases = await this.repositoryPurchase.GetToListAsync(x => x, x => x.PurchaseOrderHeaderId == key);
                            foreach (var dbPurchase in dbPurchases)
                            {
                                if (!record.PurchaseExtends.Any(x => x.PurchaseExtendId == dbPurchase.PurchaseExtendId))
                                {
                                    // Befor remove PurchaseExtend delete PurchaseLineExtend first
                                    var dbPurchaseLines1 = await this.repositoryPurchaseLine.GetToListAsync(x => x, x => x.PurchaseExtendId == dbPurchase.PurchaseExtendId);
                                    foreach (var itemLine in dbPurchaseLines1.ToList())
                                        await this.repositoryPurchaseLine.DeleteAsync(itemLine.PurchaseLineExtendId);
                                    // Remove PurchaseExtend
                                    await this.repositoryPurchase.DeleteAsync(dbPurchase.PurchaseExtendId);
                                }
                            }

                            foreach (var uPurchase in record.PurchaseExtends)
                            {
                                if (uPurchase.PurchaseExtendId > 0)
                                {
                                    #region InsertOrUpdate Purchase extend line 
                                    // filter
                                    var dbPurchaseLines = await this.repositoryPurchaseLine.GetToListAsync(x => x, x => x.PurchaseExtendId == uPurchase.PurchaseExtendId);
                                    //Remove PurchaseExtendLine if edit remove it
                                    foreach (var dbPurchaseLine in dbPurchaseLines.ToList())
                                    {
                                        if (!uPurchase.PurchaseLineExtends.Any(x => x.PurchaseLineExtendId == dbPurchaseLine.PurchaseLineExtendId))
                                            await this.repositoryPurchaseLine.DeleteAsync(dbPurchaseLine.PurchaseLineExtendId);
                                    }
                                    //Update PurchaseExtendLine or Insert PurchaseExtendLine
                                    foreach (var uPurchaseLine in uPurchase.PurchaseLineExtends)
                                    {
                                        if (uPurchaseLine.PurchaseLineExtendId > 0)
                                            await this.repositoryPurchaseLine.UpdateAsync(uPurchaseLine, uPurchaseLine.PurchaseLineExtendId);
                                        else
                                        {
                                            uPurchaseLine.PurchaseExtendId = uPurchase.PurchaseExtendId;
                                            await this.repositoryPurchaseLine.AddAsync(uPurchaseLine);
                                        }
                                    }
                                    #endregion

                                    var dbPurhcaseHere = await this.repositoryPurchase.UpdateAsync(uPurchase, uPurchase.PurchaseExtendId);
                                }
                                else
                                {
                                    uPurchase.PurchaseOrderHeaderId = record.PurchaseOrderHeaderId;
                                    await this.repositoryPurchase.AddAsync(uPurchase);
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

        // DELETE: api/PurchaseOrderHeader/
        [HttpDelete()]
        public override async Task<IActionResult> Delete(int key)
        {
            var message = "Data not been found.";
            try
            {
                if (key > 0)
                {
                    var purchaseExtend = await this.repositoryPurchase.GetToListAsync(x => x.PurchaseExtendId, x => x.PurchaseOrderHeaderId == key);
                    if (purchaseExtend != null && purchaseExtend.Any())
                    {
                        foreach (var item in purchaseExtend)
                        {
                            var purchaseLines = await this.repositoryPurchaseLine.GetToListAsync(x => x.PurchaseLineExtendId, x => x.PurchaseExtendId == item);

                            if (purchaseLines != null && purchaseLines.Any())
                            {
                                foreach (var item2 in purchaseLines.ToList())
                                    await this.repositoryPurchaseLine.DeleteAsync(item2);
                            }
                            await this.repositoryPurchase.DeleteAsync(item);
                        }
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
    }
}
