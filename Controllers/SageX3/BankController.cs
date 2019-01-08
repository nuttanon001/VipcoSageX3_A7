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
    public class BankController : GenericSageX3Controller<Bank>
    {
        public BankController(IRepositorySageX3<Bank> repo, IMapper mapper) : base(repo, mapper) { }

        // GET: api/Bank
        [HttpGet]
        public override async Task<IActionResult> Get()
        {
            var ListData = await this.repository.GetToListAsync(x => x,null, x => x.OrderBy(z => z.Ban0));
            var ListMap = new List<BankViewModel>();
            foreach (var item in ListData)
            {
                var mapData = this.mapper.Map<Bank, BankViewModel>(item);
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

            var predicate = PredicateBuilder.False<Bank>();

            foreach (string temp in filters)
            {
                string keyword = temp;
                predicate = predicate.And(x => FindFunc(x.Ban0, keyword) ||
                                              FindFunc(x.Des0, keyword));
            }

            //Order by
            Func<IQueryable<Bank>, IOrderedQueryable<Bank>> order;
            // Order
            switch (Scroll.SortField)
            {
                case "BankNumber":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Ban0);
                    else
                        order = o => o.OrderBy(x => x.Ban0);
                    break;
                case "Description":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Des0);
                    else
                        order = o => o.OrderBy(x => x.Des0);
                    break;
                default:
                    order = o => o.OrderBy(x => x.Ban0);
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

            var mapDatas = new List<BankViewModel>();
            foreach (var item in QueryData)
            {
                var MapItem = this.mapper.Map<Bank, BankViewModel>(item);
                mapDatas.Add(MapItem);
            }

            return new JsonResult(new ScrollDataViewModel<BankViewModel>(Scroll, mapDatas), this.DefaultJsonSettings);
        }

    }
}
