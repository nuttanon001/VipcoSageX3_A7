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
using VipcoSageX3.ViewModels.PurchasesModels;
using VipcoSageX3.Services.ExcelExportServices;

using AutoMapper;
using RtfPipe;

namespace VipcoSageX3.Controllers.SageX3Extends
{
    // GET: api/PurchaseLineExtend
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchaseLineExtendController : GenericSageX3ExtendController<PurchaseLineExtend>
    {
        private readonly IRepositoryDapperSageX3<PurchaseRequestLinePureViewModel> repositoryPrLinePure;
        // GET: api/PurchaseLineExtend
        public PurchaseLineExtendController(IRepositorySageX3Extend<PurchaseLineExtend> repo,
            IRepositoryDapperSageX3<PurchaseRequestLinePureViewModel> repoPrLinePure,
            IMapper mapper,
            IHelperService helper,
            JsonSerializerService jsonService) : base(repo, mapper, helper, jsonService)
        {
            this.repositoryPrLinePure = repoPrLinePure;
        }

        // GET: api/PurchaseLineExtend/GetByMaster/5
        [HttpGet("GetByMaster")]
        public async Task<IActionResult> GetByMaster(int key)
        {
            var HasData = await this.repository.GetToListAsync(
                            x => x, e => e.PurchaseExtendId == key, z => z.OrderByDescending(x => x.PrNumber).ThenBy(x => x.PrLine));
            if (HasData.Any())
                return new JsonResult(HasData, this.DefaultJsonSettings);
            else
                return NoContent();
        }

        // POST: api/PurchaseLineExtend/PurchaseReqLineExtendByPurchaseRequest
        [HttpGet("PurchaseReqLineExtendByPurchaseRequest")]
        public async Task<IActionResult> PurchaseReqLineExtendByPurchaseRequest(string key)
        {
            var message = "Data not been found.";
            try
            {
                if (!string.IsNullOrEmpty(key))
                {

                    #region Where

                    string sWhere = $" LOWER(PRD.PSHNUM_0) = '{key.Trim().ToLower()}'";

                    #endregion Where

                    #region Sort

                    string sSort = $" PRD.PSHNUM_0,PRD.PSDLIN_0 ASC ";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);

                    #endregion Sort

                    #region Query

                    var sqlCommnad = new SqlCommandViewModel()
                    {
                        SelectCommand = $@"	PRD.ROWID AS [PrSageLineId],
                                            PRD.PSHNUM_0 AS [PrNumber],
                                            PRD.PSDLIN_0 AS [PrLine],
                                            PRD.ITMREF_0 AS [ItemCode],
                                            ISNULL(TXT.TEXTE_0,PRD.ITMDES_0)  AS [ItemName],
                                            PRD.QTYPUU_0 AS [Quantity] ",
                        FromCommand = $@" [x3v11].[VIPCO].PREQUISD PRD
                                            LEFT OUTER JOIN [x3v11].[VIPCO].TEXCLOB TXT
                                            ON PRD.LINTEX_0 = TXT.CODE_0 ",
                        WhereCommand = sWhere,
                        OrderCommand = sSort
                    };

                    #endregion

                    var result = await this.repositoryPrLinePure.GetEntities(sqlCommnad);

                    foreach (var item in result)
                    {
                        if (item.ItemName.StartsWith("{\\rtf1"))
                            item.ItemName = Rtf.ToHtml(item.ItemName);
                    }


                    return new JsonResult(result, this.DefaultJsonSettings);
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
