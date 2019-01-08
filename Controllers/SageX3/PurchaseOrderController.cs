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
    // api/PurchaseOrder
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : GenericSageX3Controller<Porder>
    {
        private readonly IRepositorySageX3<Cacce> repositoryDim;
        private readonly IRepositorySageX3<Bpsupplier> repositorySup;
        public PurchaseOrderController(
            IRepositorySageX3<Porder> repo,
            IRepositorySageX3<Cacce> repoDim,
            IRepositorySageX3<Bpsupplier> repoSup,
            IMapper mapper) : base(repo, mapper, null)
        {
            // Repository
            this.repositoryDim = repoDim;
            this.repositorySup = repoSup;
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

            var predicate = PredicateBuilder.False<Porder>();

            foreach (string temp in filters)
            {
                string keyword = temp;
                predicate = predicate.Or(x => FindFunc(x.Bponam0,keyword) ||
                                              FindFunc(x.Pohnum0,keyword) ||
                                              FindFunc(x.Bprnam0,keyword) ||
                                              FindFunc(x.Cce0,keyword) ||
                                              FindFunc(x.Cce1,keyword) ||
                                              FindFunc(x.Cce2,keyword) ||
                                              FindFunc(x.Zpo010, keyword) ||
                                              FindFunc(x.Zpo020, keyword) ||
                                              FindFunc(x.Zpo030, keyword) ||
                                              FindFunc(x.Zpo040, keyword));
            }

            if (!string.IsNullOrEmpty(Scroll.Where))
            {
                var whereCon = Scroll.Where.Split(";");
                if (whereCon.Any())
                {
                    if (whereCon[0] != "-")
                        predicate = predicate.And(x => x.Zpo020 == whereCon[0]);
                    if (whereCon[1] != "-")
                        predicate = predicate.And(x => x.Zpo010 == whereCon[1]);
                    if (whereCon[2] != "-")
                        predicate = predicate.And(x => x.Pjth0 == whereCon[2]);
                }
            }

            //Order by
            Func<IQueryable<Porder>, IOrderedQueryable<Porder>> order;
            // Order
            switch (Scroll.SortField)
            {
                case "PurchaseOrderNo":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Pohnum0);
                    else
                        order = o => o.OrderBy(x => x.Pohnum0);
                    break; 
                 case "ProjectName":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Pjth0);
                    else
                        order = o => o.OrderBy(x => x.Pjth0);
                    break;
                case "SupplierName":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Bponam0);
                    else
                        order = o => o.OrderBy(x => x.Bponam0);
                    break;
                case "WorkItemName":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Zpo010);
                    else
                        order = o => o.OrderBy(x => x.Zpo010);
                    break;
                case "WorkGroupName":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Zpo020);
                    else
                        order = o => o.OrderBy(x => x.Zpo020);
                    break;
                case "OrderDateString":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Orddat0);
                    else
                        order = o => o.OrderBy(x => x.Orddat0);
                    break;
                case "TypePoHString":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Zpo210);
                    else
                        order = o => o.OrderBy(x => x.Zpo210);
                    break;
                default:
                    order = o => o.OrderByDescending(x => x.Orddat0);
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

            var mapDatas = new List<PoHeaderViewModel>();
            foreach (var item in QueryData)
            {
                var MapItem = this.mapper.Map<Porder, PoHeaderViewModel>(item);
                //Project Name
                if (!string.IsNullOrEmpty(item.Pjth0))
                {
                    MapItem.ProjectName = MapItem.ProjectCode + " ";
                    MapItem.ProjectName += await this.repositoryDim.GetFirstOrDefaultAsync(z => z.Des0, z => z.Cce0 == item.Pjth0);
                }
                if (!string.IsNullOrEmpty(item.Zpo020))
                    MapItem.WorkGroupName = await this.repositoryDim.GetFirstOrDefaultAsync(z => z.Des0, z => z.Cce0 == item.Zpo020);
                if (!string.IsNullOrEmpty(item.Zpo010))
                    MapItem.WorkItemName = await this.repositoryDim.GetFirstOrDefaultAsync(z => z.Des0, z => z.Cce0 == item.Zpo010);

                mapDatas.Add(MapItem);
            }

            return new JsonResult(new ScrollDataViewModel<PoHeaderViewModel>(Scroll, mapDatas), this.DefaultJsonSettings);
        }
    }
}
