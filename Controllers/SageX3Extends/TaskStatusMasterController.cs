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
using VipcoSageX3.Services.ExcelExportServices;

using AutoMapper;

namespace VipcoSageX3.Controllers.SageX3Extends
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskStatusMasterController : GenericSageX3ExtendController<TaskStatusMaster>
    {
        private readonly IRepositorySageX3Extend<TaskStatusDetail> repositoryDetail;
        // GET: api/TaskStatusMaster
        public TaskStatusMasterController (IRepositorySageX3Extend<TaskStatusMaster> repo,
            IRepositorySageX3Extend<TaskStatusDetail> repoDetail,
            IMapper mapper,
            IHelperService helper,
            JsonSerializerService jsonService
            ) : base(repo, mapper, helper, jsonService)
        {
            this.repositoryDetail = repoDetail;
        }

        // POST: api/TaskStatusMaster/GetScroll
        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel2 Scroll)
        {
            if (Scroll == null)
                return BadRequest();
            // Filter
            var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                : Scroll.Filter.Split(null);

            var predicate = PredicateBuilder.False<TaskStatusMaster>();

            foreach (string temp in filters)
            {
                string keyword = temp.Trim().ToLower();
                predicate = predicate.Or(x => x.WorkGroupCode.ToLower().Contains(keyword) ||
                                            x.WorkGroupName.ToLower().Contains(keyword) ||
                                            x.Remark.ToLower().Contains(keyword));
            }
            if (!string.IsNullOrEmpty(Scroll.Where))
                predicate = predicate.And(p => p.Creator == Scroll.Where);
            //Order by
            Func<IQueryable<TaskStatusMaster>, IOrderedQueryable<TaskStatusMaster>> order;
            // Order
            switch (Scroll.SortField)
            {
                case "WorkGroupCode":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.WorkGroupCode);
                    else
                        order = o => o.OrderBy(x => x.WorkGroupCode);
                    break;

                case "WorkGroupName":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.WorkGroupName);
                    else
                        order = o => o.OrderBy(x => x.WorkGroupName);
                    break;
                case "Remark":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Remark);
                    else
                        order = o => o.OrderBy(x => x.Remark);
                    break;
                default:
                    order = o => o.OrderByDescending(x => x.WorkGroupCode);
                    break;
            }
            var QueryData = await this.repository.GetToListAsync(
                                    selector: selected => selected,  // Selected
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

            return new JsonResult(new ScrollDataViewModel2<TaskStatusMaster>(Scroll, QueryData), this.DefaultJsonSettings);
        }

        // POST: api/TaskStatusMaster/
        [HttpPost()]
        public override async Task<IActionResult> Create([FromBody] TaskStatusMaster record)
        {
            var message = "Data not been found";

            try
            {
                if (record != null)
                {
                    // record = this.helperService.AddHourMethod(record);

                    if (record.GetType().GetProperty("CreateDate") != null)
                        record.GetType().GetProperty("CreateDate").SetValue(record, DateTime.Now);

                    if (record.TaskStatusDetails != null && record.TaskStatusDetails.Any())
                    {
                        foreach (var item in record.TaskStatusDetails)
                        {
                            item.CreateDate = DateTime.Now;
                            item.Creator = record.Creator;
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

        // PUT: api/TaskStatusMaster/
        [HttpPut]
        public override async Task<IActionResult> Update(int key, [FromBody] TaskStatusMaster record)
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
                    if (record.TaskStatusDetails != null && record.TaskStatusDetails.Any())
                    {
                        foreach (var item in record.TaskStatusDetails)
                        {
                            if (item.TaskStatusDetailId > 0)
                            {
                                item.CreateDate = DateTime.Now;
                                item.Creator = record.Modifyer;
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
                            var dbTaskStatusDetails = await this.repositoryDetail.GetToListAsync(x => x, x => x.TaskStatusMasterId == key);
                            //Remove TaskStatusDetails if edit remove it
                            foreach (var dbTaskStatusDetail in dbTaskStatusDetails)
                            {
                                if (!record.TaskStatusDetails.Any(x => x.TaskStatusDetailId == dbTaskStatusDetail.TaskStatusDetailId))
                                    await this.repositoryDetail.DeleteAsync(dbTaskStatusDetail.TaskStatusDetailId);
                            }
                            //Update TaskStatusDetails or Insert TaskStatusDetails
                            foreach (var uTaskDetail in record.TaskStatusDetails)
                            {
                                if (uTaskDetail.TaskStatusDetailId > 0)
                                    await this.repositoryDetail.UpdateAsync(uTaskDetail, uTaskDetail.TaskStatusDetailId);
                                else
                                {
                                    uTaskDetail.TaskStatusMasterId = record.TaskStatusMasterId;
                                    await this.repositoryDetail.AddAsync(uTaskDetail);
                                }
                            }
                            #endregion
                        }
                    }
                }
                return new JsonResult(record, this.DefaultJsonSettings);
            }
            catch(Exception ex)
            {
                message = $"Has error {ex.ToString()}";
            }
            return BadRequest(new { message });
        }

        // DELETE: api/TaskStatusMaster/
        [HttpDelete()]
        public override async Task<IActionResult> Delete(int key)
        {
            var message = "Data not been found.";
            try
            {
                if (key > 0)
                {
                    var details = await this.repositoryDetail.
                        GetToListAsync(x => x.TaskStatusDetailId, x => x.TaskStatusMasterId == key);

                    if (details.Any())
                    {
                        foreach (var item in details.ToList())
                            await this.repositoryDetail.DeleteAsync(item);
                    }

                    if (await this.repository.DeleteAsync(key) == 0)
                        return BadRequest();
                }
                return NoContent();
            }
            catch(Exception ex)
            {
                message = $"Has error {ex.ToString()}";
            }

            return BadRequest(new { message });
        }
    }
}
