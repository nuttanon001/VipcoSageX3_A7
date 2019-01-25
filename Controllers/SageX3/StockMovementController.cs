using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

using VipcoSageX3.Helper;
using VipcoSageX3.Services;
using VipcoSageX3.Services.ExcelExportServices;
using VipcoSageX3.ViewModels;
using VipcoSageX3.Models.SageX3;
//3rd Party
using RtfPipe;
using System.IO;
using AutoMapper;
using ClosedXML.Excel;

namespace VipcoSageX3.Controllers.SageX3
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockMovementController : GenericSageX3Controller<Stojou> // STOJOU
    {
        // GET: api/StockMovement
        //Context
        private readonly SageX3Context sageContext;
        private readonly IRepositorySageX3<Preceiptd> repositoryReceipt; // PRECEIPTD
        private readonly IRepositoryDapperSageX3<StockMovementViewModel> repositoryStock;
        private readonly IRepositoryDapperSageX3<StockMovement2ViewModel> repositoryStock2;
        private readonly IHelperService helperService;
        private readonly IHostingEnvironment hosting;

        public StockMovementController(IRepositorySageX3<Stojou> repo,
            IRepositorySageX3<Preceiptd> repoReceipt,
            IRepositoryDapperSageX3<StockMovementViewModel> repoStock,
            IRepositoryDapperSageX3<StockMovement2ViewModel> repoStock2,
            IHelperService helperService,
            IHostingEnvironment hosting,
            IMapper mapper, SageX3Context x3Context) : base(repo, mapper)
        {
            // Repoistory
            this.repositoryReceipt = repoReceipt;
            this.repositoryStock = repoStock;
            this.repositoryStock2 = repoStock2;
            // Helper
            this.helperService = helperService;
            // Host
            this.hosting = hosting;
            // Context
            this.sageContext = x3Context;
        }

        private async Task<List<StockMovementViewModel>> GetData(ScrollViewModel Scroll)
        {
            #region Query

            var QueryData = (from ProductMaster in this.sageContext.Itmmaster
                             //join StockJou in this.sageContext.Stojou on ProductMaster.Itmref0 equals StockJou.Itmref0 into StockJou2
                             //from nStockJou in StockJou2.DefaultIfEmpty()
                             join ProductCate in this.sageContext.Itmcateg on ProductMaster.Tclcod0 equals ProductCate.Tclcod0 into ProductCate2
                             from nProductCate in ProductCate2.DefaultIfEmpty()
                             join aText in this.sageContext.Atextra on new { code1 = nProductCate.Tclcod0, code2 = "TCLAXX" } equals new { code1 = aText.Ident10, code2 = aText.Zone0 } into AText
                             from nAText in AText.DefaultIfEmpty()
                             join bText in this.sageContext.Texclob on new { Code0 = ProductMaster.Purtex0 } equals new { bText.Code0 } into bText2
                             from fullText in bText2.DefaultIfEmpty()
                             select new
                             {
                                 // nStockJou,
                                 ProductMaster,
                                 nProductCate,
                                 nAText,
                                 fullText,
                             }).Where(x => this.sageContext.Stojou.Any(z => z.Itmref0 == x.ProductMaster.Itmref0)).AsQueryable();
            // .Where(x => x.nStockJou != null)

            #endregion Query

            #region Filter

            // Filter
            var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                : Scroll.Filter.Split(null);

            foreach (string temp in filters)
            {
                string keyword = temp.ToLower();
                QueryData = QueryData.Where(x => x.nAText.Texte0.ToLower().Contains(keyword) ||
                                                 x.ProductMaster.Itmdes10.ToLower().Contains(keyword) ||
                                                 x.ProductMaster.Itmdes20.ToLower().Contains(keyword) ||
                                                 x.ProductMaster.Itmdes30.ToLower().Contains(keyword) ||
                                                 x.ProductMaster.Itmref0.ToLower().Contains(keyword));
            }

            // Product Category
            if (Scroll.WhereBanks.Any())
                QueryData = QueryData.Where(x => Scroll.WhereBanks.Contains(x.ProductMaster.Tclcod0));

            #endregion Filter

            #region Scroll

            switch (Scroll.SortField)
            {
                case "ItemCode":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.ProductMaster.Itmref0);
                    else
                        QueryData = QueryData.OrderBy(x => x.ProductMaster.Itmref0);
                    break;

                case "ItemDescFull":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.ProductMaster.Itmdes10);
                    else
                        QueryData = QueryData.OrderBy(x => x.ProductMaster.Itmdes10);
                    break;

                case "Uom":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.ProductMaster.Stu0);
                    else
                        QueryData = QueryData.OrderBy(x => x.ProductMaster.Stu0);
                    break;

                case "Category":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.nProductCate.Tclcod0);
                    else
                        QueryData = QueryData.OrderBy(x => x.nProductCate.Tclcod0);
                    break;

                case "CategoryDesc":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.nAText.Texte0);
                    else
                        QueryData = QueryData.OrderBy(x => x.nAText.Texte0);
                    break;

                default:
                    QueryData = QueryData.OrderBy(x => x.ProductMaster.Itmref0);
                    break;
            }

            #endregion Scroll

            Scroll.TotalRow = await QueryData.CountAsync();
            var Message = "";
            try
            {
                var Datasource = await QueryData.Skip(Scroll.Skip ?? 0).Take(Scroll.Take ?? 15).AsNoTracking().ToListAsync();

                var MapDatas = new List<StockMovementViewModel>();

                var Purchase = new List<int>() { 6, 8 };
                var Stock = new List<int>() { 19, 20 , 31 , 32};
                var Sale = new List<int>() { 5, 13 };

                foreach (var item in Datasource)
                {
                    var MapData = new StockMovementViewModel()
                    {
                        Category = item?.nProductCate?.Tclcod0 ?? "",
                        CategoryDesc = item?.nAText?.Texte0 ?? "",
                        ItemCode = item?.ProductMaster?.Itmref0,
                        ItemDesc = item?.ProductMaster?.Itmdes10,
                        Uom = string.IsNullOrEmpty(item?.ProductMaster?.Pcu0.Trim()) ? item?.ProductMaster?.Stu0 : item?.ProductMaster?.Pcu0,
                    };

                    //ItemName
                    if (item?.fullText?.Texte0 != null)
                    {
                        if (item.fullText.Texte0.StartsWith("{\\rtf1"))
                            MapData.ItemDescFull = Rtf.ToHtml(item?.fullText?.Texte0);
                        else
                            MapData.ItemDescFull = item?.fullText?.Texte0;
                    }
                    else
                        MapData.ItemDescFull = item?.fullText?.Texte0 ?? "-";

                    var StockJoc = await this.repository.GetToListAsync(x => x, x => x.Itmref0 == MapData.ItemCode && x.Regflg0 == 1);
                    foreach(var item2 in StockJoc.GroupBy(x => new {
                        x.Vcrnum0,
                        x.Vcrtyp0,
                        x.Iptdat0,
                        x.Loc0,
                    })) {
                        MapData.StockMovement2s.Add(new StockMovement2ViewModel
                        {
                            Bom = item2?.FirstOrDefault()?.Cce1 ?? "",
                            DocNo = item2?.Key?.Vcrnum0 ?? "",
                            Location = item2?.FirstOrDefault()?.Loc0 ?? "",
                            MovementDate = item2?.Key?.Iptdat0,
                            MovementType = Purchase.Contains(item2.Key.Vcrtyp0) ? "Purchase" :
                                       (Stock.Contains(item2.Key.Vcrtyp0) ? "Stock" :
                                       (Sale.Contains(item2.Key.Vcrtyp0) ? "Sale" : "Stock")),
                            Project = item2?.FirstOrDefault()?.Cce2 ?? "",
                            WorkGroup = item2?.FirstOrDefault()?.Cce3 ?? "",
                            QuantityIn = (double)item2?.Where(x => x.Qtypcu0 > 0)?.Sum(x => x?.Qtypcu0 ?? (decimal)0),
                            QuantityOut = (double)item2?.Where(x => x.Qtypcu0 <= 0)?.Sum(x => x?.Qtypcu0 ?? (decimal)0),
                        });
                    }

                    MapDatas.Add(MapData);
                }

                return MapDatas;
            }
            catch (Exception ex)
            {
                Message = $"{ex.ToString()}";
            }

            return null;
        }


        private async Task<List<StockMovementViewModel>> GetData2(ScrollViewModel scroll)
        {
            if (scroll != null)
            {
                string sWhere = "";
                string sSort = "";
                string sQuery = "";

                #region Where

                var filters = string.IsNullOrEmpty(scroll.Filter) ? new string[] { "" }
                                : scroll.Filter.Split(null);
                foreach (string temp in filters)
                {
                    if (string.IsNullOrEmpty(temp))
                        continue;

                    string keyword = temp.ToLower();
                    sWhere += (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") +
                                                    $@"(LOWER(ITM.ITMREF_0) LIKE '%{keyword}%'
                                                        OR LOWER(ITM.ITMDES1_0) LIKE '%{keyword}%'
                                                        OR LOWER(ITM.ITMDES2_0) LIKE '%{keyword}%'
                                                        OR LOWER(ITM.ITMDES3_0) LIKE '%{keyword}%''
                                                        OR LOWER(ATE.TEXTE_0) LIKE '%{keyword}%')";
                }

                // Product Category
                if (scroll.WhereBanks.Any())
                {
                    var list = new List<string>();

                    foreach (var item in scroll.WhereBanks)
                        list.Add($"'{item}'");

                    var banks = string.Join(',', list);
                    sWhere += (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"ITM.TCLCOD_0 IN ({banks})";
                    // predicate = predicate.And(x => Scroll.WhereBanks.Contains(x.Ban0));
                }

                #endregion Where

                #region Sort

                switch (scroll.SortField)
                {
                    case "ItemCode":
                        if (scroll.SortOrder == -1)
                            sSort = $"ITM.ITMREF_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pshnum0);
                        else
                            sSort = $"ITM.ITMREF_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pshnum0);
                        break;

                    case "ItemDescFull":
                        if (scroll.SortOrder == -1)
                            sSort = $"ITM.ITMDES1_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pjth0);
                        else
                            sSort = $"ITM.ITMDES1_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pjth0);
                        break;

                    case "Uom":
                        if (scroll.SortOrder == -1)
                            sSort = $"ITM.STU_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"ITM.STU_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "Category":
                        if (scroll.SortOrder == -1)
                            sSort = $"ITM.TCLCOD_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.prd.Itmdes0);
                        else
                            sSort = $"ITM.TCLCOD_0 ASC";//QueryData = QueryData.OrderBy(x => x.prd.Itmdes0);
                        break;

                    case "CategoryDesc":
                        if (scroll.SortOrder == -1)
                            sSort = $"ATE.TEXTE_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Cce0);
                        else
                            sSort = $"ATE.TEXTE_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Cce0);
                        break;

                    default:
                        sSort = $"ITM.ITMREF_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        break;
                }

                #endregion Sort

                #region Query
                // Query mulitple command
                sQuery = $@"SELECT	ITM.ITMREF_0 AS [ItemCode],
                                    ITM.ITMDES1_0 AS [ItemDesc],
                                    TXT.TEXTE_0 AS [ItemDescFull],
                                    CAT.TCLCOD_0 AS [Category],
                                    ATE.TEXTE_0 AS [CategoryDesc],
                                    ISNULL(NULLIF(ITM.PCU_0,''),ITM.STU_0) AS [Uom]
                            FROM	VIPCO.ITMMASTER ITM
                                    LEFT JOIN VIPCO.TEXCLOB TXT 
                                        ON TXT.CODE_0 = ITM.PURTEX_0
                                    LEFT JOIN VIPCO.ITMCATEG CAT 
                                        ON ITM.TCLCOD_0 = CAT.TCLCOD_0
                                    LEFT JOIN VIPCO.ATEXTRA ATE 
                                        ON CAT.TCLCOD_0 = ATE.IDENT1_0
                                        AND	ATE.ZONE_0 = 'TCLAXX'
                            {sWhere}
                            ORDER BY    {sSort}
                            OFFSET      @Skip ROWS       -- skip 10 rows
                            FETCH NEXT  @Take ROWS ONLY; -- take 10 rows;
                            SELECT	    COUNT(*)
                            FROM	    VIPCO.ITMMASTER ITM
                                        LEFT JOIN VIPCO.TEXCLOB TXT 
                                            ON TXT.CODE_0 = ITM.PURTEX_0
                                        LEFT JOIN VIPCO.ITMCATEG CAT 
                                            ON ITM.TCLCOD_0 = CAT.TCLCOD_0
                                        LEFT JOIN VIPCO.ATEXTRA ATE 
                                            ON CAT.TCLCOD_0 = ATE.IDENT1_0
                                            AND	ATE.ZONE_0 = 'TCLAXX'
                            {sWhere};";

                #endregion Query

                var result = await this.repositoryStock.GetListEntitesAndTotalRow(sQuery, new { Skip = scroll.Skip ?? 0, Take = scroll.Take ?? 15 });
                var dbData = result.Entities;
                scroll.TotalRow = result.TotalRow;

                string stockJournal = "";
                foreach (var item in dbData)
                {
                    if (item.ItemDescFull.StartsWith("{\\rtf1"))
                        item.ItemDescFull = Rtf.ToHtml(item?.ItemDescFull);
                    else
                        item.ItemDescFull = item?.ItemDesc;


                    #region stockJournal

                    stockJournal = $@"SELECT	STO.VCRNUM_0 AS DocNo,
                                                STO.VCRTYP_0 AS MovementType,
                                                STO.IPTDAT_0 AS MovementDate,
                                                (CASE 
                                                    WHEN SUM(STO.QTYPCU_0) > 0 
                                                        THEN SUM(STO.QTYPCU_0)
                                                END) AS QuantityIn,
                                                (CASE 
                                                    WHEN SUM(STO.QTYPCU_0) <= 0 
                                                        THEN SUM(STO.QTYPCU_0)
                                                END) AS QuantityOut,
                                                STO.LOC_0 AS [Location],
                                                STO.CCE_1 AS Bom,
                                                STO.CCE_2 AS Project,
                                                STO.CCE_3 AS WorkGroup

                                        FROM	VIPCO.STOJOU STO
                                        WHERE   STO.ITMREF_0 = @ItemCode AND STO.REGFLG_0 = 1
                                        GROUP BY	STO.VCRNUM_0,
                                                    STO.VCRTYP_0,
                                                    STO.IPTDAT_0,
                                                    STO.LOC_0,
                                                    STO.CCE_1,
                                                    STO.CCE_2,
                                                    STO.CCE_3
                                    ORDER BY    STO.IPTDAT_0";

                    var stockMoves = await this.repositoryStock2.GetListEntites(stockJournal, new { item.ItemCode });
                    item.StockMovement2s.AddRange(stockMoves);

                    #endregion stockJournal
                }

                return dbData;
            }
            return null;
        }

        #region BackUp

        /*
        private async Task<List<StockMovementViewModel>> GetData2(ScrollViewModel Scroll)
        {
            #region Query
            var QueryData = (from StockJou in this.sageContext.Stojou
                             join ProductMaster in this.sageContext.Itmmaster on StockJou.Itmref0 equals ProductMaster.Itmref0 into ProductMaster2
                             from nProductMaster in ProductMaster2.DefaultIfEmpty()
                             join ProductCate in this.sageContext.Itmcateg on nProductMaster.Tclcod0 equals ProductCate.Tclcod0 into ProductCate2
                             from nProductCate in ProductCate2.DefaultIfEmpty()
                             join aText in this.sageContext.Atextra on new { code1 = nProductCate.Tclcod0, code2 = "TCLAXX" } equals new { code1 = aText.Ident10, code2 = aText.Zone0 } into AText
                             from nAText in AText.DefaultIfEmpty()
                             join bText in this.sageContext.Texclob on new { Code0 = nProductMaster.Purtex0 } equals new { bText.Code0 } into bText2
                             from fullText in bText2.DefaultIfEmpty()
                             select new
                             {
                                 StockJou,
                                 nProductMaster,
                                 nProductCate,
                                 nAText,
                                 fullText,
                             }).AsQueryable();
            #endregion Query
            #region Filter
            // Filter
            var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                : Scroll.Filter.Split(null);
            foreach (string temp in filters)
            {
                string keyword = temp.ToLower();
                QueryData = QueryData.Where(x => x.nAText.Texte0.ToLower().Contains(keyword) ||
                                                 x.nProductMaster.Itmdes10.ToLower().Contains(keyword) ||
                                                 x.nProductMaster.Itmdes20.ToLower().Contains(keyword) ||
                                                 x.nProductMaster.Itmdes30.ToLower().Contains(keyword) ||
                                                 x.StockJou.Itmref0.ToLower().Contains(keyword));
            }

            // Product Category
            if (Scroll.WhereBanks.Any())
                QueryData = QueryData.Where(x => Scroll.WhereBanks.Contains(x.nProductMaster.Tclcod0));

            #endregion Filter

            #region Scroll

            switch (Scroll.SortField)
            {
                case "ItemCode":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.nProductMaster.Itmref0);
                    else
                        QueryData = QueryData.OrderBy(x => x.nProductMaster.Itmref0);
                    break;

                case "ItemDescFull":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.nProductMaster.Itmdes10);
                    else
                        QueryData = QueryData.OrderBy(x => x.nProductMaster.Itmdes10);
                    break;

                case "Uom":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.nProductMaster.Stu0);
                    else
                        QueryData = QueryData.OrderBy(x => x.nProductMaster.Stu0);
                    break;

                case "Category":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.nProductCate.Tclcod0);
                    else
                        QueryData = QueryData.OrderBy(x => x.nProductCate.Tclcod0);
                    break;

                case "CategoryDesc":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.nAText.Texte0);
                    else
                        QueryData = QueryData.OrderBy(x => x.nAText.Texte0);
                    break;

                case "DocNo":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.StockJou.Vcrnum0);
                    else
                        QueryData = QueryData.OrderBy(x => x.StockJou.Vcrnum0);
                    break;

                case "Location":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.StockJou.Loc0);
                    else
                        QueryData = QueryData.OrderBy(x => x.StockJou.Loc0);
                    break;

                case "Bom":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.StockJou.Cce1);
                    else
                        QueryData = QueryData.OrderBy(x => x.StockJou.Cce1);
                    break;

                case "Project":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.StockJou.Cce2);
                    else
                        QueryData = QueryData.OrderBy(x => x.StockJou.Cce2);
                    break;

                case "WorkGroup":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.StockJou.Cce3);
                    else
                        QueryData = QueryData.OrderBy(x => x.StockJou.Cce3);
                    break;

                case "MovementType":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.StockJou.Vcrtyp0);
                    else
                        QueryData = QueryData.OrderBy(x => x.StockJou.Vcrtyp0);
                    break;

                case "MovementDateString":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.StockJou.Iptdat0);
                    else
                        QueryData = QueryData.OrderBy(x => x.StockJou.Iptdat0);
                    break;

                case "QuantityString":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.StockJou.Qtypcu0);
                    else
                        QueryData = QueryData.OrderBy(x => x.StockJou.Qtypcu0);
                    break;

                default:
                    QueryData = QueryData.OrderByDescending(x => x.nProductMaster.Itmref0).ThenBy(x => x.StockJou.Iptdat0);
                    break;
            }

            #endregion Scroll

            Scroll.TotalRow = await QueryData.CountAsync();
            var Message = "";
            try
            {
                var Datasource = await QueryData.GroupBy(x => new
                {
                    x.StockJou.Vcrnum0,
                    x.StockJou.Vcrtyp0,
                    x.StockJou.Itmref0,
                    x.StockJou.Iptdat0,
                }).OrderBy(x => x.Key.Itmref0).ThenBy(x => x.Key.Iptdat0)
                .Skip(Scroll.Skip ?? 0).Take(Scroll.Take ?? 15).AsNoTracking().ToListAsync();

                var MapDatas = new List<StockMovementViewModel>();

                var Purchase = new List<int>() { 6, 8 };
                var Stock = new List<int>() { 19, 20, 31, 32 };
                var Sale = new List<int>() { 5, 13 };

                foreach (var item in Datasource)
                {
                    var MapData = new StockMovementViewModel()
                    {
                        Category = item?.FirstOrDefault()?.nProductCate?.Tclcod0 ?? "",
                        CategoryDesc = item?.FirstOrDefault()?.nAText?.Texte0 ?? "",
                        Bom = item?.FirstOrDefault()?.StockJou?.Cce1 ?? "",
                        DocNo = item?.Key?.Vcrnum0 ?? "",
                        Location = item?.FirstOrDefault()?.StockJou?.Loc0 ?? "",
                        MovementDate = item?.Key?.Iptdat0,
                        Project = item?.FirstOrDefault()?.StockJou?.Cce2 ?? "",
                        QuantityIn = (double)item?.Where(x => x.StockJou.Qtypcu0 > 0)?.Sum(x => x?.StockJou?.Qtypcu0 ?? (decimal)0),
                        QuantityOut = (double)item?.Where(x => x.StockJou.Qtypcu0 <= 0)?.Sum(x => x?.StockJou?.Qtypcu0 ?? (decimal)0),
                        WorkGroup = item?.FirstOrDefault()?.StockJou?.Cce3 ?? "",
                        ItemCode = item?.FirstOrDefault()?.nProductMaster?.Itmref0,
                        ItemDesc = item?.FirstOrDefault()?.nProductMaster?.Itmdes10,
                        Uom = item?.FirstOrDefault()?.nProductMaster?.Stu0,
                        MovementType = Purchase.Contains(item.Key.Vcrtyp0) ? "Purchase" :
                                       (Stock.Contains(item.Key.Vcrtyp0) ? "Stock" :
                                       (Sale.Contains(item.Key.Vcrtyp0) ? "Sale" : "Stock"))
                    };

                    //ItemName
                    if (item?.FirstOrDefault()?.fullText?.Texte0 != null)
                    {
                        if (item.FirstOrDefault().fullText.Texte0.StartsWith("{\\rtf1"))
                            MapData.ItemDescFull = Rtf.ToHtml(item?.FirstOrDefault()?.fullText?.Texte0);
                        else
                            MapData.ItemDescFull = item?.FirstOrDefault()?.fullText?.Texte0;
                    }
                    else
                        MapData.ItemDescFull = item?.FirstOrDefault()?.fullText?.Texte0 ?? "-";

                    MapDatas.Add(MapData);
                }

                return MapDatas;
            }
            catch (Exception ex)
            {
                Message = $"{ex.ToString()}";
            }

            return null;
        }
        */

        #endregion
        // POST: api/StockMovement/GetScroll
        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            var Message = "";
            try
            {
                var HasData = await this.GetData2(Scroll);
                return new JsonResult(new ScrollDataViewModel<StockMovementViewModel>(Scroll, HasData), this.DefaultJsonSettings);
            }
            catch (Exception ex)
            {
                Message = $"{ex.ToString()}";
            }
            return BadRequest(new { Error = Message });
        }

        [HttpPost("GetReport")]
        public async Task<IActionResult> GetReport([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();
            var Message = "";
            var MapDatas = await this.GetData2(Scroll);
            try
            {
                if (MapDatas.Any())
                {
                    var table = new DataTable();
                    //Adding the Columns
                    table.Columns.AddRange(new DataColumn[]
                    {
                        new DataColumn("ItemCode", typeof(string)),
                        new DataColumn("Description", typeof(string)),
                        new DataColumn("CategoryDescription",typeof(string)),
                        new DataColumn("Uom",typeof(string)),

                        new DataColumn("DocNo", typeof(string)),
                        new DataColumn("MovementType", typeof(string)),
                        new DataColumn("MovementDate",typeof(string)),
                        new DataColumn("QuantityIn",typeof(string)),
                        new DataColumn("QuantityOut", typeof(string)),
                        new DataColumn("Location", typeof(string)),
                        new DataColumn("Bom",typeof(string)),
                        new DataColumn("Project",typeof(string)),
                        new DataColumn("WorkGroup",typeof(string)),
                    });

                    //Adding the Rows
                    foreach (var item in MapDatas)
                    {
                        foreach (var item2 in item.StockMovement2s)
                        {
                            item.ItemDescFull = this.helperService.ConvertHtmlToText(item.ItemDescFull);
                            item.ItemDescFull = item.ItemDescFull.Replace("\r\n", "");
                            item.ItemDescFull = item.ItemDescFull.Replace("\n", "");

                            table.Rows.Add(
                               item.ItemCode,
                               item.ItemDescFull,
                               item.CategoryDesc,
                               item.Uom,

                               item2.DocNo,
                               item2.MovementType,
                               item2.MovementDateString,
                               item2.QuantityInString,
                               item2.QuantityOutString,
                               item2.Location,
                               item2.Bom,
                               item2.Project,
                               item2.WorkGroup
                            );
                        }
                    }

                    return File(this.helperService.CreateExcelFilePivotTables(table,"StockMovement","StockMovementPivot"), 
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export.xlsx");
                }
            }
            catch (Exception ex)
            {
                Message = $"{ex.ToString()}";
            }

            return BadRequest();
        }
    }
}
