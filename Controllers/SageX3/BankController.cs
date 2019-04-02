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
using VipcoSageX3.ViewModels.Addition;

using AutoMapper;
using VipcoSageX3.Helper;
using RtfPipe;

namespace VipcoSageX3.Controllers.SageX3
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : GenericSageX3Controller<Bank>
    {
        private readonly IRepositoryDapperSageX3<CustomerViewModel> repositoryCustomer;
        private readonly IRepositoryDapperSageX3<BranchViewModel> repositoryBranch;
        private readonly IRepositoryDapperSageX3<PartnerViewModel> repositoryPartner;

        public BankController(IRepositorySageX3<Bank> repo,
            IRepositoryDapperSageX3<CustomerViewModel> repoCustomer,
            IRepositoryDapperSageX3<BranchViewModel> repoBranch,
            IRepositoryDapperSageX3<PartnerViewModel> repoPartner,
            IMapper mapper) : base(repo, mapper)
        {
            this.repositoryBranch = repoBranch;
            this.repositoryCustomer = repoCustomer;
            this.repositoryPartner = repoPartner;
        }

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

        // POST: api/Bank/GetScroll
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
                predicate = predicate.Or(x => FindFunc(x.Ban0, keyword) ||
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

        // POST: api/Bank/GetBranchScroll
        [HttpPost("GetBranchScroll")]
        public async Task<IActionResult> GetBranchScroll([FromBody] ScrollViewModel Scroll)
        {
            var message = "Data not been found.";

            try
            {
                if (Scroll == null)
                    return BadRequest();

                string sWhere = $@" BCH.IDENT1_0 = 'BCH' AND 
                                    BCH.ZONE_0 = 'DESTRA' AND 
                                    BCH.CODFIC_0 = 'CACCE'";
                string sSort = "";

                #region Where

                // Filter
                var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                    : Scroll.Filter.Split(null);

                foreach (string temp in filters)
                {
                    if (string.IsNullOrEmpty(temp))
                        continue;

                    string keyword = temp.ToLower();
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $@"(LOWER(BCH.TEXTE_0) LIKE '%{keyword}%'
                                                                                OR LOWER(BCH.IDENT2_0) LIKE '%{keyword}%')";
                }

                // Where Branch

                #endregion

                #region Sort

                switch (Scroll.SortField)
                {
                    case "BranchName":
                        if (Scroll.SortOrder == -1)
                            sSort = $"BCH.TEXTE_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pshnum0);
                        else
                            sSort = $"BCH.TEXTE_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pshnum0);
                        break;

                    case "BranchCode":
                        if (Scroll.SortOrder == -1)
                            sSort = $"BCH.IDENT2_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pjth0);
                        else
                            sSort = $"BCH.IDENT2_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pjth0);
                        break;

                    default:
                        sSort = $"BCH.IDENT2_0 ASC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        break;
                }

                #endregion

                var sqlCommnad = new SqlCommandViewModel()
                {
                    SelectCommand = $@"	BCH.TEXTE_0 AS [BranchName],
                                        BCH.IDENT2_0 AS [BranchCode],
                                        ROWID AS [RowId]",
                    FromCommand = $@" VIPCO.ATEXTRA BCH",
                    WhereCommand = sWhere,
                    OrderCommand = sSort
                };

                var result = await this.repositoryBranch.GetEntitiesAndTotal(sqlCommnad, new { Skip = Scroll.Skip ?? 0, Take = Scroll.Take ?? 50 });
                var dbData = result.Entities;
                Scroll.TotalRow = result.TotalRow;

                return new JsonResult(new ScrollDataViewModel<BranchViewModel>(Scroll, dbData), this.DefaultJsonSettings);
            }
            catch (Exception ex)
            {
                message = $"Has error {ex.ToString()}";
            }

            return BadRequest(new { message });
        }

        // POST: api/Bank/GetCustomerScroll
        [HttpPost("GetCustomerScroll")]
        public async Task<IActionResult> GetCustomerScroll([FromBody] ScrollViewModel Scroll)
        {
            var message = "Data not been found.";

            try
            {
                if (Scroll == null)
                    return BadRequest();

                string sWhere = "";
                string sSort = "";

                #region Where

                // Filter
                var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                    : Scroll.Filter.Split(null);

                foreach (string temp in filters)
                {
                    if (string.IsNullOrEmpty(temp))
                        continue;

                    string keyword = temp.ToLower();
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") +
                                                    $@"(LOWER(CUS.BPCNUM_0) LIKE '%{keyword}%'
                                                        OR LOWER(CUS.ZCOMPNAME_0) LIKE '%{keyword}%')";
                }

                // Where Customer

                #endregion

                #region Sort

                switch (Scroll.SortField)
                {
                    case "CustomerNo":
                        if (Scroll.SortOrder == -1)
                            sSort = $"CUS.BPCNUM_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pshnum0);
                        else
                            sSort = $"CUS.BPCNUM_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pshnum0);
                        break;

                    case "CustomerName":
                        if (Scroll.SortOrder == -1)
                            sSort = $"CUS.ZCOMPNAME_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pjth0);
                        else
                            sSort = $"CUS.ZCOMPNAME_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pjth0);
                        break;

                    default:
                        sSort = $"CUS.BPCNUM_0 ASC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        break;
                }

                #endregion

                var sqlCommnad = new SqlCommandViewModel()
                {
                    SelectCommand = $@"	CUS.BPCNUM_0 AS [CustomerNo],
                                        CUS.ZCOMPNAME_0 AS [CustomerName],
                                        CUS.ROWID AS [Rowid]",
                    FromCommand = $@" [VIPCO].[BPCUSTOMER] CUS",
                    WhereCommand = sWhere,
                    OrderCommand = sSort
                };

                var result = await this.repositoryCustomer.GetEntitiesAndTotal(sqlCommnad, new { Skip = Scroll.Skip ?? 0, Take = Scroll.Take ?? 50 });
                var dbData = result.Entities;
                Scroll.TotalRow = result.TotalRow;

                return new JsonResult(new ScrollDataViewModel<CustomerViewModel>(Scroll, dbData), this.DefaultJsonSettings);
            }
            catch (Exception ex)
            {
                message = $"Has error {ex.ToString()}";
            }

            return BadRequest(new { message });
        }

        // POST: api/Bank/GetCustomerScroll
        [HttpPost("GetPartnerScroll")]
        public async Task<IActionResult> GetPartnerScroll([FromBody] ScrollViewModel Scroll)
        {
            var message = "Data not been found.";

            try
            {
                if (Scroll == null)
                    return BadRequest();

                string sWhere = "";
                string sSort = "";

                #region Where

                // Filter
                var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                    : Scroll.Filter.Split(null);

                foreach (string temp in filters)
                {
                    if (string.IsNullOrEmpty(temp))
                        continue;

                    string keyword = temp.ToLower();
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") +
                                                    $@"(LOWER(PAR.BPRNUM_0) LIKE N'%{keyword}%'
                                                        OR LOWER(PAR.BPRNAM_0) LIKE N'%{keyword}%')";
                }

                // Where Customer

                #endregion

                #region Sort

                switch (Scroll.SortField)
                {
                    case "PartnerNo":
                        if (Scroll.SortOrder == -1)
                            sSort = $"PAR.BPRNUM_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pshnum0);
                        else
                            sSort = $"PAR.BPRNUM_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pshnum0);
                        break;

                    case "PartnerName":
                        if (Scroll.SortOrder == -1)
                            sSort = $"PAR.BPRNAM_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pjth0);
                        else
                            sSort = $"PAR.BPRNAM_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pjth0);
                        break;

                    default:
                        sSort = $"PAR.BPRNUM_0 ASC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        break;
                }

                #endregion

                var sqlCommnad = new SqlCommandViewModel()
                {
                    SelectCommand = $@"	[PAR].[BPRNUM_0] AS [PartnerNo],
                                        [PAR].[BPRNAM_0] AS [PartnerName],
                                        [PAR].[ROWID] AS [Rowid]",
                    FromCommand = $@" [VIPCO].[BPARTNER] PAR",
                    WhereCommand = sWhere,
                    OrderCommand = sSort
                };

                var result = await this.repositoryPartner.GetEntitiesAndTotal(sqlCommnad, new { Skip = Scroll.Skip ?? 0, Take = Scroll.Take ?? 50 });
                var dbData = result.Entities;
                Scroll.TotalRow = result.TotalRow;

                return new JsonResult(new ScrollDataViewModel<PartnerViewModel>(Scroll, dbData), this.DefaultJsonSettings);
            }
            catch (Exception ex)
            {
                message = $"Has error {ex.ToString()}";
            }

            return BadRequest(new { message });
        }
    }
}
