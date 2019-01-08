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


namespace VipcoSageX3.Controllers.SageX3
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkGroupController : GenericSageX3Controller<Cacce>
    {
        // GET: api/WorkGroup
        private readonly IRepositorySageX3<Atextra> repositoryDim2;
        public WorkGroupController(IRepositorySageX3<Cacce> repo,
            IRepositorySageX3<Atextra> repoDim2,
            IMapper mapper) : base(repo, mapper)
        {
            this.repositoryDim2 = repoDim2;
        }

        // GET: api/WorkGroup
        [HttpGet]
        public override async Task<IActionResult> Get()
        {
            //var ListData = await this.repository.GetToListAsync(x => x, x => x.Die0 == "WG",x => x.OrderBy(z => z.Cce0));
            //var ListMap = new List<WorkGroupViewModel>();
            //foreach (var item in ListData)
            //{
            //    var mapData = this.mapper.Map<Cacce, WorkGroupViewModel>(item);
            //    ListMap.Add(mapData);
            //}
            //return new JsonResult(ListMap, this.DefaultJsonSettings);

            var ListData = await this.repositoryDim2.GetToListAsync(
                x => x, 
                x => x.Zone0 == "LNGDES" && x.Ident10 == "3002" && x.Ident20.Length > 1);
            var ListMap = new List<WorkGroupViewModel>();
            foreach (var item in ListData)
            {
                var mapData = this.mapper.Map<Atextra, WorkGroupViewModel>(item);
                ListMap.Add(mapData);
            }
            return new JsonResult(ListMap, this.DefaultJsonSettings);
        }

        // POST: api/PurchaseOrder/GetScroll
        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();


            // Filter
            var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                : Scroll.Filter.Split(null);
            Expression<Func<Atextra,bool>> predicate = x => x.Zone0 == "LNGDES" && x.Ident10 == "3002" && x.Ident20.Length > 1;
            foreach (string temp in filters)
            {
                string keyword = temp;
                predicate = predicate.And(x => x.Ident20.ToLower().Contains(keyword.ToLower()) ||
                                               x.Texte0.ToLower().Contains(keyword.ToLower()));
            }

            //Order by
            Func<IQueryable<Atextra>, IOrderedQueryable<Atextra>> order;
            // Order
            switch (Scroll.SortField)
            {
                case "WorkGroupCode":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Ident20);
                    else
                        order = o => o.OrderBy(x => x.Ident20);
                    break;
                case "WorkGroupName":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Texte0);
                    else
                        order = o => o.OrderBy(x => x.Texte0);
                    break;
                default:
                    order = o => o.OrderBy(x => x.Ident20);
                    break;
            }


            var QueryData = await this.repositoryDim2.GetToListAsync(
                                    selector: selected => selected,  // Selected
                                    predicate: predicate, // Where
                                    orderBy: order, // Order
                                    include: null, // Include
                                    skip: Scroll.Skip ?? 0, // Skip
                                    take: Scroll.Take ?? 10); // Take

            // Get TotalRow
            Scroll.TotalRow = await this.repositoryDim2.GetLengthWithAsync(predicate: predicate);

            var mapDatas = new List<WorkGroupViewModel>();
            foreach (var item in QueryData)
            {
                var MapItem = this.mapper.Map<Atextra, WorkGroupViewModel>(item);
                mapDatas.Add(MapItem);
            }

            return new JsonResult(new ScrollDataViewModel<WorkGroupViewModel>(Scroll, mapDatas), this.DefaultJsonSettings);
        }
    }
}
