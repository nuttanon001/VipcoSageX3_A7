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
    // GET: api/Supplier
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : GenericSageX3Controller<Bpartner>
    {
        public SupplierController(IRepositorySageX3<Bpartner> repo,IMapper mapper) : base(repo, mapper) { }

        // GET: api/Bank
        [HttpGet]
        public override async Task<IActionResult> Get()
        {
            var ListData = await this.repository.GetToListAsync(x => x, null, x => x.OrderBy(z => z.Bprnam0));
            var ListMap = new List<SupplierViewModel>();
            foreach (var item in ListData)
            {
                var mapData = this.mapper.Map<Bpartner, SupplierViewModel>(item);
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

            var predicate = PredicateBuilder.False<Bpartner>();

            foreach (string temp in filters)
            {
                string keyword = temp;
                predicate = predicate.Or(x => FindFunc(x.Bprnum0, keyword) ||
                                              FindFunc(x.Bprnam0, keyword) ||
                                              FindFunc(x.Bprnam1, keyword));
            }

            //Order by
            Func<IQueryable<Bpartner>, IOrderedQueryable<Bpartner>> order;
            // Order
            switch (Scroll.SortField)
            {
                case "SupplierNo":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Bprnum0);
                    else
                        order = o => o.OrderBy(x => x.Bprnum0);
                    break;
                case "SupplierName":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Bprnam0);
                    else
                        order = o => o.OrderBy(x => x.Bprnam0);
                    break;
                default:
                    order = o => o.OrderBy(x => x.Bprnam0);
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

            var mapDatas = new List<SupplierViewModel>();
            foreach (var item in QueryData)
            {
                var MapItem = this.mapper.Map<Bpartner, SupplierViewModel>(item);
                mapDatas.Add(MapItem);
            }

            return new JsonResult(new ScrollDataViewModel<SupplierViewModel>(Scroll, mapDatas), this.DefaultJsonSettings);
        }
    }
}
