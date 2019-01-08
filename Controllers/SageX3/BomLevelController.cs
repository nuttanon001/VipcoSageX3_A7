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
    public class BomLevelController : GenericSageX3Controller<Cacce>
    {
        // GET: api/BomLevel
        public BomLevelController(IRepositorySageX3<Cacce> repo, IMapper mapper) : base(repo, mapper)
        { }

        // GET: api/BomLevel
        [HttpGet]
        public override async Task<IActionResult> Get()
        {
            var ListData = await this.repository.GetToListAsync(x => x, x => x.Die0 == "BOM", x => x.OrderBy(z => z.Cce0));
            var ListMap = new List<BomLevelViewModel>();
            foreach (var item in ListData)
            {
                var mapData = this.mapper.Map<Cacce, BomLevelViewModel>(item);
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

            Expression<Func<Cacce, bool>> predicate = e => e.Die0 == "BOM";

            // Filter
            var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                : Scroll.Filter.Split(null);

            foreach (string temp in filters)
            {
                string keyword = temp.ToLower();
                predicate = predicate.And(x => x.Cce0.ToLower().Contains(keyword) ||
                                              x.Des0.ToLower().Contains(keyword));
            }

            //Order by
            Func<IQueryable<Cacce>, IOrderedQueryable<Cacce>> order;
            // Order
            switch (Scroll.SortField)
            {
                case "BomLevelCode":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Cce0);
                    else
                        order = o => o.OrderBy(x => x.Cce0);
                    break;
                case "BomLevelName":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Des0);
                    else
                        order = o => o.OrderBy(x => x.Des0);
                    break;
                default:
                    order = o => o.OrderBy(x => x.Cce0);
                    break;
            }

            var QueryData = await this.repository.GetToListAsync(
                                    selector: selected => selected,  // Selected
                                    predicate: predicate, // Where
                                    orderBy: order, // Order
                                    include: null, // Include
                                    skip: Scroll.Skip ?? 0, // Skip
                                    take: Scroll.Take ?? 10); // Take

            // Get TotalRow
            Scroll.TotalRow = await this.repository.GetLengthWithAsync(predicate: predicate);

            var mapDatas = new List<BomLevelViewModel>();
            foreach (var item in QueryData)
            {
                var MapItem = this.mapper.Map<Cacce, BomLevelViewModel>(item);
                mapDatas.Add(MapItem);
            }

            return new JsonResult(new ScrollDataViewModel<BomLevelViewModel>(Scroll, mapDatas), this.DefaultJsonSettings);
        }
    }
}
