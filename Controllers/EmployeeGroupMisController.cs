using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using VipcoSageX3.Services;
using VipcoSageX3.ViewModels;
using VipcoSageX3.Models.Machines;
using AutoMapper;

namespace VipcoSageX3.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class EmployeeGroupMisController : GenericMachineController<EmployeeGroupMis>
    {
        public EmployeeGroupMisController(IRepositoryMachine<EmployeeGroupMis> repo,
            IMapper mapper) : base(repo,mapper,null) { }

        [HttpGet("GroupMisByEmpCode")]
        public async Task<IActionResult> GetGroupMisByEmpCode(string EmpCode)
        {
            var HasData = await this.repository.GetAllAsQueryable()
                                    .Where(x => !x.GroupMis.StartsWith("00"))
                                    .FirstOrDefaultAsync(x => x.Employee.Any(z => z.EmpCode == EmpCode));
            return new JsonResult(HasData, this.DefaultJsonSettings);
        }
        // POST: api/EmployeeGroupMis/GetScroll
        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel Scroll)
        {
            var QueryData = this.repository.GetAllAsQueryable().Where(x => !x.GroupMis.StartsWith("00"));
            // Where
            if (!string.IsNullOrEmpty(Scroll.Where))
            {
                // QueryData = QueryData.Where(x => x.GroupCode == Scroll.Where);
            }
            // Filter
            var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                : Scroll.Filter.ToLower().Split(null);
            foreach (var keyword in filters)
            {
                QueryData = QueryData.Where(x => x.GroupDesc.ToLower().Contains(keyword) ||
                                                 x.GroupMis.ToLower().Contains(keyword) ||
                                                 x.Remark.ToLower().Contains(keyword));
            }

            // Order
            switch (Scroll.SortField)
            {
                case "GroupMis":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(e => e.GroupMis);
                    else
                        QueryData = QueryData.OrderBy(e => e.GroupMis);
                    break;

                case "GroupDesc":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(e => e.GroupDesc);
                    else
                        QueryData = QueryData.OrderBy(e => e.GroupDesc);
                    break;

                default:
                    QueryData = QueryData.OrderByDescending(e => e.GroupDesc);
                    break;
            }
            // Get TotalRow
            Scroll.TotalRow = await QueryData.CountAsync();
            // Skip and Take
            QueryData = QueryData.Skip(Scroll.Skip ?? 0).Take(Scroll.Take ?? 50);

            var ListData = new List<GroupMisViewModel>();
            foreach (var item in await QueryData.ToListAsync())
            {
                var MapItem = this.mapper.Map<EmployeeGroupMis, GroupMisViewModel>(item);
                ListData.Add(MapItem);
            }

            return new JsonResult(new ScrollDataViewModel<GroupMisViewModel>
                (Scroll, ListData), this.DefaultJsonSettings);
        }
    }
}
