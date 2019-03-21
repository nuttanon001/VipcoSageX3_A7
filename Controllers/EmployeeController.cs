using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

using VipcoSageX3.Services;
using VipcoSageX3.ViewModels;
using VipcoSageX3.Models.Machines;

using AutoMapper;
using Newtonsoft.Json;
using VipcoSageX3.Services.ExcelExportServices;
using Microsoft.AspNetCore.Authorization;

namespace VipcoSageX3.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class EmployeeController : GenericMachineController<Employee>
    {
        // Mapper
        private readonly JsonSerializerService jsonService;
        private readonly IHostingEnvironment hostingEnv;

        public EmployeeController(IRepositoryMachine<Employee> repo,
            JsonSerializerService jsonService,
            IHostingEnvironment hostingEnv,
            IMapper mapper) : base(repo,mapper,null)
        {
            this.jsonService = jsonService;
            this.hostingEnv = hostingEnv;
        }

        // POST: api/Employee/GetScroll
        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel Scroll)
        {
            var QueryData = this.repository.GetAllAsQueryable();
            // Where
            if (!string.IsNullOrEmpty(Scroll.Where))
            {
                QueryData = QueryData.Where(x => x.GroupCode == Scroll.Where);
            }
            // Filter
            var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                : Scroll.Filter.ToLower().Split(null);
            foreach (var keyword in filters)
            {
                QueryData = QueryData.Where(x => x.NameEng.ToLower().Contains(keyword) ||
                                                 x.NameThai.ToLower().Contains(keyword) ||
                                                 x.EmpCode.ToLower().Contains(keyword) ||
                                                 x.GroupCode.ToLower().Contains(keyword) ||
                                                 x.GroupName.ToLower().Contains(keyword));
            }

            // Order
            switch (Scroll.SortField)
            {
                case "EmpCode":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(e => e.EmpCode);
                    else
                        QueryData = QueryData.OrderBy(e => e.EmpCode);
                    break;

                case "NameThai":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(e => e.NameThai);
                    else
                        QueryData = QueryData.OrderBy(e => e.NameThai);
                    break;

                case "GroupName":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(e => e.GroupName);
                    else
                        QueryData = QueryData.OrderBy(e => e.GroupName);
                    break;

                default:
                    QueryData = QueryData.OrderBy(e => e.EmpCode.Length)
                                         .ThenBy(e => e.EmpCode);
                    break;
            }
            // Get TotalRow
            Scroll.TotalRow = await QueryData.CountAsync();
            // Skip and Take
            QueryData = QueryData.Skip(Scroll.Skip ?? 0).Take(Scroll.Take ?? 50);
            var ListData = new List<EmployeeViewModel>();
            // Foreach
            foreach (var Employee in await QueryData.ToListAsync())
                ListData.Add(this.mapper.Map<Employee, EmployeeViewModel>(Employee));
            
            return new JsonResult(new ScrollDataViewModel<EmployeeViewModel>
                (Scroll, ListData), this.DefaultJsonSettings);
        }

        // POST: api/Employee/GetAllowedEmployeeScroll
        [Authorize]
        [HttpPost("GetAllowedEmployeeScroll")]
        public async Task<IActionResult> GetAllowedEmployeeScroll([FromBody] ScrollViewModel Scroll)
        {
            string contentRootPath = this.hostingEnv.ContentRootPath;
            // get activity code from json file
            var allowedEmployees = JsonConvert.DeserializeObject<List<AllowedEmployeeViewModel>>
                (await System.IO.File.ReadAllTextAsync(contentRootPath + "/Data/allowed_emp.json"));

            var emps = await this.repository.GetToListAsync(
                                    x => new { x.EmpCode, x.NameThai },
                                    x => allowedEmployees.Select(z => z.EmpCode).Contains(x.EmpCode));

            foreach (var item in allowedEmployees)
                item.NameThai = emps.FirstOrDefault(x => x.EmpCode == item.EmpCode)?.NameThai ?? "-";

            // Filter
            var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                : Scroll.Filter.ToLower().Split(null);

            foreach (var keyword in filters)
            {
                allowedEmployees = allowedEmployees.Where(x =>  x.NameThai.ToLower().Contains(keyword) ||
                                                                x.EmpCode.ToLower().Contains(keyword)).ToList();
            }

            // Order
            switch (Scroll.SortField)
            {
                case "EmpCode":
                    if (Scroll.SortOrder == -1)
                        allowedEmployees = allowedEmployees.OrderByDescending(e => e.EmpCode).ToList();
                    else
                        allowedEmployees = allowedEmployees.OrderBy(e => e.EmpCode).ToList();
                    break;

                case "NameThai":
                    if (Scroll.SortOrder == -1)
                        allowedEmployees = allowedEmployees.OrderByDescending(e => e.NameThai).ToList();
                    else
                        allowedEmployees = allowedEmployees.OrderBy(e => e.NameThai).ToList();
                    break;

                default:
                    allowedEmployees = allowedEmployees.OrderBy(e => e.EmpCode.Length)
                                                        .ThenBy(e => e.EmpCode).ToList();
                    break;
            }
            // Get TotalRow
            Scroll.TotalRow = allowedEmployees.Count();
            // Skip and Take
            allowedEmployees = allowedEmployees.Skip(Scroll.Skip ?? 0).Take(Scroll.Take ?? 50).ToList();

            return new JsonResult(new ScrollDataViewModel<AllowedEmployeeViewModel>
                (Scroll, allowedEmployees), this.DefaultJsonSettings);
        }

        // POST: api/Employee/UpdateAllowedEmployee
        [Authorize]
        [HttpPost("UpdateAllowedEmployee")]
        public async Task<IActionResult> UpdateAllowedEmployee([FromBody] AllowedEmployeeViewModel allowed)
        {
            var message = "Update allowed failed";
            try
            {
                if (allowed != null)
                {
                    string contentRootPath = this.hostingEnv.ContentRootPath;
                    // get activity code from json file
                    var allowedEmployees = JsonConvert.DeserializeObject<List<AllowedEmployeeViewModel>>
                        (await System.IO.File.ReadAllTextAsync(contentRootPath + "/Data/allowed_emp.json"));

                    // get activity code from somchai database server
                    if (allowedEmployees.Any())
                    {
                        if (allowedEmployees.Any(x => x.EmpCode == allowed.EmpCode))
                        {
                            var index = allowedEmployees.FindIndex(x => x.EmpCode == allowed.EmpCode);
                            if (index >= 0)
                            {
                                allowedEmployees[index].EmpCode = allowed.EmpCode;
                                allowedEmployees[index].SubLevel = allowed.SubLevel;
                            }
                        }
                        else
                        {
                            allowedEmployees.Add(new AllowedEmployeeViewModel()
                            {
                                EmpCode = allowed.EmpCode,
                                SubLevel = allowed.SubLevel,
                            });
                        }

                        await System.IO.File.WriteAllTextAsync(contentRootPath + "/Data/allowed_emp.json", JsonConvert.SerializeObject(allowedEmployees));
                        return new JsonResult(allowed,this.DefaultJsonSettings);
                    }
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
