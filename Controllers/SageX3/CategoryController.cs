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
using VipcoSageX3.Models.SageX3;

using AutoMapper;
using VipcoSageX3.Helper;
using RtfPipe;

namespace VipcoSageX3.Controllers.SageX3
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : GenericSageX3Controller<Itmcateg>
    {
        private readonly IRepositorySageX3<Atextra> repositoryText;
        private readonly SageX3Context Context;

        public CategoryController(IRepositorySageX3<Itmcateg> repo,
            IRepositorySageX3<Atextra> repoText,
            SageX3Context context,
            IMapper mapper) : base(repo, mapper) {
            // IReposiorySageX3
            this.repositoryText = repoText;
            // Context
            this.Context = context;
        }

        // GET: api/Category
        [HttpGet]
        public override async Task<IActionResult> Get()
        {
            var ListData = await this.repository.GetToListAsync(x => x, null, x => x.OrderBy(z => z.Tclcod0));
            var ListMap = new List<CategoryViewModel>();
            foreach (var item in ListData)
            {
                var mapData = new CategoryViewModel
                {
                    CategoryCode = item.Tclcod0,
                    Rowid = (int)item.Rowid,
                    CategoryName = (await this.repositoryText.GetFirstOrDefaultAsync(x => x.Texte0,x => x.Ident10 == item.Tclcod0 && x.Zone0 == "TCLAXX")) ?? "-"
                };
                ListMap.Add(mapData);
            }
            return new JsonResult(ListMap, this.DefaultJsonSettings);
        }

        // POST: api/Category/GetScroll
        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            var Query = (from Category in this.Context.Itmcateg
                             join aText in this.Context.Atextra on new { code1 = Category.Tclcod0, code2 = "TCLAXX" } equals new { code1 = aText.Ident10, code2 = aText.Zone0 } into AText
                             from nAText in AText.DefaultIfEmpty()
                             select new
                             {
                                 Category,
                                 nAText
                             }).AsQueryable();

            // Filter
            var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                : Scroll.Filter.Split(null);

            foreach (string temp in filters)
            {
                string keyword = temp.ToLower().Trim();
                Query = Query.Where(x => x.Category.Tclcod0.ToLower().Trim().Contains(keyword) ||
                                                 x.nAText.Texte0.ToLower().Trim().Contains(keyword));
            }

            // Order
            switch (Scroll.SortField)
            {
                case "BankNumber":
                    if (Scroll.SortOrder == -1)
                        Query = Query.OrderByDescending(x => x.Category.Tclcod0);
                    else
                        Query = Query.OrderBy(x => x.Category.Tclcod0);
                    break;
                case "Description":
                    if (Scroll.SortOrder == -1)
                        Query = Query.OrderByDescending(x => x.nAText.Texte0);
                    else
                        Query = Query.OrderBy(x => x.nAText.Texte0);
                    break;
                default:
                    Query = Query.OrderBy(x => x.Category.Tclcod0);
                    break;
            }

            // Get TotalRow
            Scroll.TotalRow = await Query.CountAsync();
            // Query Data
            var QueryData = await Query.Skip(Scroll.Skip ?? 0).Take(Scroll.Take ?? 15).AsNoTracking().ToListAsync();

            var mapDatas = new List<CategoryViewModel>();
            foreach (var item in QueryData)
            {
                var mapData = new CategoryViewModel
                {
                    CategoryCode = item.Category.Tclcod0,
                    Rowid = (int)item.Category.Rowid,
                    CategoryName = item.nAText.Texte0,
                };
                mapDatas.Add(mapData);
            }
            return new JsonResult(new ScrollDataViewModel<CategoryViewModel>(Scroll, mapDatas), this.DefaultJsonSettings);
        }
    }
}
