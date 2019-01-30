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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskStatusDetailController : GenericSageX3ExtendController<TaskStatusDetail>
    {
        // GET: api/TaskStatusDetail
        public TaskStatusDetailController(IRepositorySageX3Extend<TaskStatusDetail> repo,
            IMapper mapper,
            IHelperService helper,
            JsonSerializerService jsonService) : base(repo, mapper, helper, jsonService)
        { }

        // GET: api/TaskStatusDetail/GetByMaster/5
        [HttpGet("GetByMaster")]
        public async Task<IActionResult> GetByMaster(int key)
        {
            var HasData = await this.repository.GetToListAsync(
                            x => x, e => e.TaskStatusMasterId == key, z => z.OrderBy(x => x.Name));
            if (HasData.Any())
                return new JsonResult(HasData, this.DefaultJsonSettings);
            else
                return NoContent();
        }
    }
}
