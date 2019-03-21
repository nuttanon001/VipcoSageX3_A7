//3rd Party
using AutoMapper;
using ClosedXML.Excel;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RtfPipe;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VipcoSageX3.Models.SageX3;
using VipcoSageX3.Services;
using VipcoSageX3.Services.ExcelExportServices;
using VipcoSageX3.ViewModels;
using VipcoSageX3.ViewModels.StockModels;

namespace VipcoSageX3.Controllers.SageX3
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockOnHandController : GenericSageX3Controller<Itmmvt>
    {
        // GET: api/StockOnHand
        //Context
        private readonly SageX3Context sageContext;

        private readonly IRepositorySageX3<Stock> repositoryStock;
        private readonly IRepositorySageX3<Stolot> repositoryStockLot;
        private readonly IRepositoryDapperSageX3<OnHandMk2ViewMode> repositoryOnHand;
        private readonly IRepositoryDapperSageX3<StockBalanceViewModel> repositoryBalance;
        private readonly IRepositoryDapperSageX3<IssusWorkGroupViewModel> repositoryIssus;
        private readonly ExcelWorkBookService excelWbService;
        private readonly IHostingEnvironment hosting;
        private readonly IHelperService helperService;

        public StockOnHandController(IRepositorySageX3<Itmmvt> repo,
            IRepositorySageX3<Stock> repoStock,
            IRepositorySageX3<Stolot> repoStockLot,
            IRepositoryDapperSageX3<OnHandMk2ViewMode> repoOnHand,
            IRepositoryDapperSageX3<StockBalanceViewModel> repoBalance,
            IRepositoryDapperSageX3<IssusWorkGroupViewModel> repoIssus,
            ExcelWorkBookService exWbService,
            IHostingEnvironment hosting,
            IHelperService helperService,
            IMapper mapper, SageX3Context x3Context) : base(repo, mapper)
        {
            // Repoistory
            this.repositoryStock = repoStock;
            this.repositoryStockLot = repoStockLot;
            this.repositoryOnHand = repoOnHand;
            this.repositoryBalance = repoBalance;
            this.repositoryIssus = repoIssus;
            // Host
            this.hosting = hosting;
            // Helper
            this.excelWbService = exWbService;
            this.helperService = helperService;
            // Context
            this.sageContext = x3Context;
        }

        #region PrivateMethod

        private async Task<List<StockOnHandViewModel>> GetData(ScrollViewModel Scroll)
        {
            #region Query

            var QueryData = (from ProductsSites in this.sageContext.Itmfacilit
                             join ProductTotal in this.sageContext.Itmmvt on ProductsSites.Itmref0 equals ProductTotal.Itmref0 into ProductStock
                             from nProductStock in ProductStock.DefaultIfEmpty()
                             join ProductMaster in this.sageContext.Itmmaster on ProductsSites.Itmref0 equals ProductMaster.Itmref0 into ProductMaster2
                             from nProductMaster in ProductMaster2.DefaultIfEmpty()
                             join ProductCate in this.sageContext.Itmcateg on nProductMaster.Tclcod0 equals ProductCate.Tclcod0 into ProductCate2
                             from nProductCate in ProductCate2.DefaultIfEmpty()
                             join aText in this.sageContext.Atextra on new { code1 = nProductCate.Tclcod0, code2 = "TCLAXX" } equals new { code1 = aText.Ident10, code2 = aText.Zone0 } into AText
                             from nAText in AText.DefaultIfEmpty()
                             join bText in this.sageContext.Texclob on new { Code0 = nProductMaster.Purtex0 } equals new { bText.Code0 } into bText2
                             from fullText in bText2.DefaultIfEmpty()
                             select new
                             {
                                 ProductsSites,
                                 nProductStock,
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
                                                 x.ProductsSites.Itmref0.ToLower().Contains(keyword));
            }

            // Product Category
            if (Scroll.WhereBanks.Any())
                QueryData = QueryData.Where(x => Scroll.WhereBanks.Contains(x.nProductMaster.Tclcod0));

            QueryData = QueryData.Where(x => x.nProductStock.Physto0 > 0 || x.ProductsSites.Ofs0 > 0);

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

                case "InternelStockString":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.nProductStock.Physto0);
                    else
                        QueryData = QueryData.OrderBy(x => x.nProductStock.Physto0);
                    break;

                case "OnOrderString":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.ProductsSites.Ofs0);
                    else
                        QueryData = QueryData.OrderBy(x => x.ProductsSites.Ofs0);
                    break;

                default:
                    QueryData = QueryData.OrderByDescending(x => x.nProductMaster.Itmref0);
                    break;
            }

            #endregion Scroll

            Scroll.TotalRow = await QueryData.CountAsync();
            var Message = "";
            try
            {
                var Datasource = await QueryData.Skip(Scroll.Skip ?? 0).Take(Scroll.Take != -1 ? (Scroll.Take ?? 15) : (Scroll.TotalRow ?? 15)).AsNoTracking().ToListAsync();
                var MapDatas = new List<StockOnHandViewModel>();

                foreach (var item in Datasource)
                {
                    var MapData = new StockOnHandViewModel()
                    {
                        Category = item?.nProductCate?.Tclcod0,
                        CategoryDesc = item?.nAText?.Texte0,
                        InternelStock = (double)(item?.nProductStock?.Physto0 ?? 0),
                        ItemCode = item?.nProductMaster?.Itmref0,
                        ItemDesc = item?.nProductMaster?.Itmdes10,
                        OnOrder = (double)(item?.nProductStock?.Ordsto0 ?? 0),
                        Uom = string.IsNullOrEmpty(item?.nProductMaster?.Pcu0.Trim()) ? item?.nProductMaster?.Stu0 : item?.nProductMaster?.Pcu0,
                    };

                    // Set Stock
                    var ListStock = await this.repositoryStock.GetToListAsync(x => x, x => x.Itmref0 == MapData.ItemCode);
                    if (ListStock != null && ListStock.Any())
                    {
                        foreach (var stock in ListStock.GroupBy(x => new { x.Loc0, x.Pcu0, x.Pjt0, x.Lot0, x.Bpslot0, x.Palnum0 }))
                        {
                            var itemStock = new StockLocationViewModel
                            {
                                LocationCode = stock.Key.Loc0,
                                Uom = stock.Key.Pcu0,
                                Project = stock.Key.Pjt0,
                                LotNo = stock.Key.Lot0,
                                HeatNo = stock.Key.Bpslot0,
                                Origin = stock.Key.Palnum0,
                                Quantity = (double)(stock?.Sum(z => z.Qtypcu0) ?? (decimal)0),
                            };

                            if (!string.IsNullOrEmpty(itemStock.LotNo))
                            {
                                var stock_lot = await this.repositoryStockLot.GetFirstOrDefaultAsync
                               (x => x, x => x.Itmref0 == MapData.ItemCode && x.Lot0 == itemStock.LotNo);

                                if (stock_lot != null)
                                {
                                    if (stock_lot.Shldat0.Year < 2600)
                                        itemStock.ExpDate = stock_lot.Shldat0;
                                }
                            }

                            MapData.StockLocations.Add(itemStock);
                        }
                    }

                    //ItemName
                    if (item.fullText?.Texte0 != null)
                    {
                        if (item.fullText.Texte0.StartsWith("{\\rtf1"))
                            MapData.ItemDescFull = Rtf.ToHtml(item.fullText?.Texte0);
                        else
                            MapData.ItemDescFull = item?.fullText?.Texte0;
                    }
                    else
                        MapData.ItemDescFull = item?.fullText?.Texte0 ?? "-";

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

        private async Task<List<OnHandMk2ViewMode>> GetDataOnHandMk2(ScrollViewModel Scroll)
        {
            if (Scroll != null)
            {
                // ACC_0 ลูกหนี้ในประเทศ 113101 และ ลูกหนี้ต่างประเทศ 113201
                string sWhere = "";
                string subSWhere = "";
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
                                                     $@"(LOWER(OnTable.ITMREF_0) LIKE '%{keyword}%'
                                                        OR LOWER(ITM.ITMDES1_0 ) LIKE '%{keyword}%'
                                                        OR LOWER(ITM.TCLCOD_0) LIKE '%{keyword}%')";
                }

                // Where Item Cate
                if (Scroll.WhereBanks.Any())
                {
                    var list = new List<string>();

                    foreach (var item in Scroll.WhereBanks)
                        list.Add($"'{item}'");

                    var customers = string.Join(',', list);
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"ITM.TCLCOD_0 IN ({customers})";
                }

                // Range Location
                if (!string.IsNullOrEmpty(Scroll.WhereRange11))
                {
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"OnTable.LOC_0 >= '{Scroll.WhereRange11}'";
                }

                if (!string.IsNullOrEmpty(Scroll.WhereRange12))
                {
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"OnTable.LOC_0 <= '{Scroll.WhereRange12}'";
                }
                // Range Lot
                if (!string.IsNullOrEmpty(Scroll.WhereRange21))
                {
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"OnTable.LOT_0 >= '{Scroll.WhereRange11}'";
                }

                if (!string.IsNullOrEmpty(Scroll.WhereRange22))
                {
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"OnTable.LOT_0 <= '{Scroll.WhereRange12}'";
                }

                // Where Date Range
                if (Scroll.SDate.HasValue)
                {
                    subSWhere += $"IPTDAT_0 <= '{Scroll.SDate.Value.ToString("yyyy-MM-dd")}'";
                }

                #endregion Where

                #region Sort

                switch (Scroll.SortField)
                {
                    case "LocationCode":
                        if (Scroll.SortOrder == -1)
                            sSort = $"OnTable.LOC_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pshnum0);
                        else
                            sSort = $"OnTable.LOC_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pshnum0);
                        break;

                    case "ItemNo":
                        if (Scroll.SortOrder == -1)
                            sSort = $"OnTable.ITMREF_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"OnTable.ITMREF_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "ItemName":
                        if (Scroll.SortOrder == -1)
                            sSort = $"ITM.ITMDES1_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pjth0);
                        else
                            sSort = $"ITM.ITMDES1_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pjth0);
                        break;

                    case "ItemCate":
                        if (Scroll.SortOrder == -1)
                            sSort = $"ITM.TCLCOD_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pjth0);
                        else
                            sSort = $"ITM.TCLCOD_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pjth0);
                        break;

                    case "LotCode":
                        if (Scroll.SortOrder == -1)
                            sSort = $"OnTable.LOT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"OnTable.LOT_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "QuantityString":
                        if (Scroll.SortOrder == -1)
                            sSort = $"OnTable.QtySum DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"OnTable.QtySum ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "Uom":
                        if (Scroll.SortOrder == -1)
                            sSort = $"OnTable.PCU_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"OnTable.PCU_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "UnitPriceString":
                        if (Scroll.SortOrder == -1)
                            sSort = $"JoTable.OrderPrice DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"JoTable.OrderPrice ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "OrderDateString":
                        if (Scroll.SortOrder == -1)
                            sSort = $"JoTable.AllocationDate DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"JoTable.AllocationDate ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    default:
                        sSort = $"OnTable.ITMREF_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        break;
                }

                #endregion Sort

                var sqlCommnad = new SqlCommandViewModel()
                {
                    SelectCommand = $@"	OnTable.LOC_0 AS [LocationCode],
                                        OnTable.ITMREF_0 AS [ItemNo],
                                        ITM.ITMDES1_0 AS [ItemName],
                                        ITM.TCLCOD_0 AS [ItemCate],
                                        TXT.TEXTE_0 AS [TextName],
                                        OnTable.LOT_0 AS [LotCode],
                                        OnTable.QtySum AS [Quantity],
                                        OnTable.PCU_0 AS [Uom],
                                        JoTable.OrderPrice AS [UnitPrice],
                                        JoTable.AllocationDate AS [OrderDate]",
                    FromCommand = $@" (SELECT	ITMREF_0,
                                                LOT_0,
                                                LOC_0,
                                                PCU_0,
                                                SUM(QTYPCU_0) AS QtySum
                                        FROM	[VIPCO].STOJOU
                                        {(string.IsNullOrEmpty(subSWhere) ? "" : "WHERE " + subSWhere)}
                                        GROUP BY LOT_0,LOC_0,ITMREF_0,PCU_0
                                        HAVING SUM(QTYPCU_0) > 0) AS OnTable
                                        LEFT OUTER JOIN
                                        (SELECT	MIN(IPTDAT_0) AS [AllocationDate],
                                                ITMREF_0,
                                                AVG(PRIORD_0) AS [OrderPrice],
                                                LOT_0,
                                                LOC_0
                                        FROM	[VIPCO].STOJOU
                                        WHERE	TRSTYP_0 = 3 {(string.IsNullOrEmpty(subSWhere) ? "" : "AND " + subSWhere)}
                                        GROUP BY LOT_0,LOC_0,ITMREF_0) AS JoTable
                                        ON OnTable.LOC_0 = JoTable.LOC_0
                                            AND OnTable.LOT_0 = JoTable.LOT_0
                                            AND OnTable.ITMREF_0 = JoTable.ITMREF_0
                                        LEFT OUTER JOIN [VIPCO].[ITMMASTER] ITM
                                            ON OnTable.ITMREF_0 = ITM.ITMREF_0
                                            LEFT OUTER JOIN VIPCO.TEXCLOB TXT
                                            ON TXT.CODE_0 = ITM.PURTEX_0",
                    WhereCommand = sWhere,
                    OrderCommand = sSort
                };

                var result = await this.repositoryOnHand.GetEntitiesAndTotal(sqlCommnad, new { Skip = Scroll.Skip ?? 0, Take = Scroll.Take ?? 50 });
                var dbData = result.Entities;
                Scroll.TotalRow = result.TotalRow;

                foreach (var item in dbData)
                {
                    if (!string.IsNullOrEmpty(item.TextName))
                    {
                        if (item.TextName.StartsWith("{\\rtf1"))
                            item.TextName = Rtf.ToHtml(item.TextName);
                    }
                    else
                        item.TextName = item.ItemName;
                }

                return dbData;
            }
            return null;
        }

        private async Task<List<OnHandMk2ViewMode>> GetDataOnHandMk2V2(ScrollViewModel Scroll)
        {
            if (Scroll != null)
            {
                // ACC_0 ลูกหนี้ในประเทศ 113101 และ ลูกหนี้ต่างประเทศ 113201
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
                                                     $@"(LOWER(STJ.ITMREF_0) LIKE '%{keyword}%'
                                                        OR LOWER(ITM.ITMDES1_0 ) LIKE '%{keyword}%'
                                                        OR LOWER(ITM.TCLCOD_0) LIKE '%{keyword}%')";
                }

                // Where Item Cate
                if (Scroll.WhereBanks.Any())
                {
                    var list = new List<string>();

                    foreach (var item in Scroll.WhereBanks)
                        list.Add($"'{item}'");

                    var customers = string.Join(',', list);
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"ITM.TCLCOD_0 IN ({customers})";
                }

                // Range Location
                if (!string.IsNullOrEmpty(Scroll.WhereRange11))
                {
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"STJ.LOC_0 >= '{Scroll.WhereRange11}'";
                }

                if (!string.IsNullOrEmpty(Scroll.WhereRange12))
                {
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"STJ.LOC_0 <= '{Scroll.WhereRange12}'";
                }
                // Range Lot
                if (!string.IsNullOrEmpty(Scroll.WhereRange21))
                {
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"STJ.LOT_0 >= '{Scroll.WhereRange11}'";
                }

                if (!string.IsNullOrEmpty(Scroll.WhereRange22))
                {
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"STJ.LOT_0 <= '{Scroll.WhereRange12}'";
                }

                // Where Date Range
                if (Scroll.SDate.HasValue)
                {
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"STJ.IPTDAT_0 <= '{Scroll.SDate.Value.ToString("yyyy-MM-dd")}'";
                }

                #endregion Where

                #region Sort

                switch (Scroll.SortField)
                {
                    case "LocationCode":
                        if (Scroll.SortOrder == -1)
                            sSort = $"STJ.LOC_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pshnum0);
                        else
                            sSort = $"STJ.LOC_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pshnum0);
                        break;

                    case "ItemNo":
                        if (Scroll.SortOrder == -1)
                            sSort = $"STJ.ITMREF_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"STJ.ITMREF_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "ItemName":
                        if (Scroll.SortOrder == -1)
                            sSort = $"ITM.ITMDES1_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pjth0);
                        else
                            sSort = $"ITM.ITMDES1_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pjth0);
                        break;

                    case "ItemCate":
                        if (Scroll.SortOrder == -1)
                            sSort = $"ITM.TCLCOD_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pjth0);
                        else
                            sSort = $"ITM.TCLCOD_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pjth0);
                        break;

                    case "LotCode":
                        if (Scroll.SortOrder == -1)
                            sSort = $"STJ.LOT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"STJ.LOT_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "QuantityString":
                        if (Scroll.SortOrder == -1)
                            sSort = $"SUM(STJ.QTYSTU_0) DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"SUM(STJ.QTYSTU_0) ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "Uom":
                        if (Scroll.SortOrder == -1)
                            sSort = $"STJ.STU_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"STJ.STU_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    default:
                        sSort = $"STJ.ITMREF_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        break;
                }

                #endregion Sort

                var sqlCommnad = new SqlCommandViewModel()
                {
                    SelectCommand = $@"	STJ.ITMREF_0 AS [ItemNo],
                                        STJ.LOC_0 AS [LocationCode],
                                        STJ.LOT_0 AS [LotCode],
                                        STJ.STU_0 AS [Uom],
                                        ITM.ITMDES1_0 AS [ItemName],
                                        ITM.TCLCOD_0 AS [ItemCate],
                                        TXT.TEXTE_0 AS [TextName],
                                        SUM(STJ.QTYSTU_0) AS [Quantity],
                                        SUM(QTYSTU_0*PRIVAL_0) AS [PriceTotal],
                                        SUM(STV.VMATTOT_0) AS [PriceAndVat],
                                        SUM(STV.VINVDTATOT_0) AS [Vat],
                                        (SELECT MIN(IPTDAT_0) FROM VIPCO.STOJOU STJ2
                                                    WHERE STJ2.TRSTYP_0 = 3 AND STJ.ITMREF_0 = STJ2.ITMREF_0
                                                        AND STJ.LOC_0 = STJ2.LOC_0
                                                        AND STJ.LOT_0 = STJ2.LOT_0) AS [OrderDate] ",
                    FromCommand = $@" VIPCO.STOJOU STJ
                                        LEFT OUTER JOIN VIPCO.STOJOUVAL STV
                                            ON STJ.STOFCY_0 = STV.STOFCY_0
                                            AND	STJ.UPDCOD_0 = STV.UPDCOD_0
                                            AND	STJ.ITMREF_0 = STV.ITMREF_0
                                            AND STJ.IPTDAT_0 = STV.IPTDAT_0
                                            AND	STJ.MVTSEQ_0 = STV.MVTSEQ_0
                                            AND STJ.MVTIND_0 = STV.MVTIND_0
                                        LEFT OUTER JOIN [VIPCO].[ITMMASTER] ITM
                                            ON STJ.ITMREF_0 = ITM.ITMREF_0
                                        LEFT OUTER JOIN VIPCO.TEXCLOB TXT
                                            ON TXT.CODE_0 = ITM.PURTEX_0 ",
                    WhereCommand = sWhere,
                    OrderCommand = sSort,
                    GroupCommand = $@" STJ.ITMREF_0,
                                        LOC_0,LOT_0,
                                        STJ.STU_0,
                                        ITM.ITMDES1_0,
                                        ITM.TCLCOD_0,
                                        TXT.TEXTE_0
                                        HAVING SUM(QTYPCU_0) != 0 "
                };

                var result = await this.repositoryOnHand.GetEntitiesAndTotal(sqlCommnad, new { Skip = Scroll.Skip ?? 0, Take = Scroll.Take ?? 50 });
                var dbData = result.Entities;
                Scroll.TotalRow = result.TotalRow;

                foreach (var item in dbData)
                {
                    item.UnitPrice = item.PriceAndVat != null && item.Vat != null && item.Quantity != null ?
                        (item.PriceAndVat + item.Vat) / item.Quantity : item.PriceTotal / item.Quantity;

                    if (!string.IsNullOrEmpty(item.TextName))
                    {
                        if (item.TextName.StartsWith("{\\rtf1"))
                            item.TextName = Rtf.ToHtml(item.TextName);
                    }
                    else
                        item.TextName = item.ItemName;
                }

                return dbData;
            }
            return null;
        }

        private async Task<List<StockBalanceViewModel>> GetDataBalance(ScrollViewModel Scroll)
        {
            if (Scroll != null)
            {
                // ACC_0 ลูกหนี้ในประเทศ 113101 และ ลูกหนี้ต่างประเทศ 113201
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
                                                     $@"(LOWER(STJ.ITMREF_0) LIKE '%{keyword}%'
                                                        OR LOWER(ITM.ITMDES1_0 ) LIKE '%{keyword}%'
                                                        OR LOWER(STJ.LOC_0 ) LIKE '%{keyword}%'
                                                        OR LOWER(ITM.TCLCOD_0) LIKE '%{keyword}%')";
                }

                // Where Item Cate
                if (Scroll.WhereBanks.Any())
                {
                    var list = new List<string>();

                    foreach (var item in Scroll.WhereBanks)
                        list.Add($"'{item}'");

                    var customers = string.Join(',', list);
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"ITM.TCLCOD_0 IN ({customers})";
                }

                // Range Location
                if (!string.IsNullOrEmpty(Scroll.WhereRange11))
                {
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"STJ.LOC_0 >= '{Scroll.WhereRange11}'";
                }

                if (!string.IsNullOrEmpty(Scroll.WhereRange12))
                {
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"STJ.LOC_0 <= '{Scroll.WhereRange12}'";
                }

                // Where Date Range
                if (Scroll.SDate.HasValue)
                {
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"STJ.IPTDAT_0 <= '{Scroll.SDate.Value.ToString("yyyy-MM-dd")}'";
                }

                #endregion Where

                #region Sort

                switch (Scroll.SortField)
                {
                    case "LocationCode":
                        if (Scroll.SortOrder == -1)
                            sSort = $"STJ.LOC_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pshnum0);
                        else
                            sSort = $"STJ.LOC_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pshnum0);
                        break;

                    case "ItemNo":
                        if (Scroll.SortOrder == -1)
                            sSort = $"STJ.ITMREF_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"STJ.ITMREF_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "ItemName":
                        if (Scroll.SortOrder == -1)
                            sSort = $"ITM.ITMDES1_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pjth0);
                        else
                            sSort = $"ITM.ITMDES1_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pjth0);
                        break;

                    case "ItemCate":
                        if (Scroll.SortOrder == -1)
                            sSort = $"ITM.TCLCOD_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pjth0);
                        else
                            sSort = $"ITM.TCLCOD_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pjth0);
                        break;

                    case "QuantityString":
                        if (Scroll.SortOrder == -1)
                            sSort = $"SUM(STJ.QTYSTU_0) DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"SUM(STJ.QTYSTU_0) ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "Uom":
                        if (Scroll.SortOrder == -1)
                            sSort = $"STJ.STU_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"STJ.STU_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    default:
                        sSort = $"STJ.ITMREF_0 ASC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        break;
                }

                #endregion Sort

                var sqlCommnad = new SqlCommandViewModel()
                {
                    SelectCommand = $@"	STJ.LOC_0 AS [LocationCode],
                                        STJ.ITMREF_0 AS [ItemNo],
                                        ITM.ITMDES1_0 AS [ItemName],
                                        ITM.TCLCOD_0 AS [ItemCate],
                                        TXT.TEXTE_0 AS [TextName],
                                        SUM(STJ.QTYSTU_0) AS [Quantity],
                                        STJ.STU_0 AS [Uom],
                                        (SELECT ORDSTO_0
                                            FROM VIPCO.ITMMVT ITV
                                                WHERE ITV.ITMREF_0 = STJ.ITMREF_0) AS [ReOrder] ",
                    FromCommand = $@" VIPCO.STOJOU STJ
                                        LEFT OUTER JOIN [VIPCO].[ITMMASTER] ITM
                                            ON STJ.ITMREF_0 = ITM.ITMREF_0
                                        LEFT OUTER JOIN VIPCO.TEXCLOB TXT
                                            ON TXT.CODE_0 = ITM.PURTEX_0 ",
                    WhereCommand = sWhere,
                    OrderCommand = sSort,
                    GroupCommand = $@" STJ.ITMREF_0,
                                        LOC_0,STJ.STU_0,
                                        ITM.ITMDES1_0,
                                        ITM.TCLCOD_0,
                                        TXT.TEXTE_0
                                        HAVING SUM(STJ.QTYSTU_0) != 0 "
                };

                var result = await this.repositoryBalance.GetEntitiesAndTotal(sqlCommnad, new { Skip = Scroll.Skip ?? 0, Take = Scroll.Take ?? 50 });
                var dbData = result.Entities;
                Scroll.TotalRow = result.TotalRow;

                foreach (var item in dbData)
                {
                    item.Available = " ";

                    if (!string.IsNullOrEmpty(item.TextName))
                    {
                        if (item.TextName.StartsWith("{\\rtf1"))
                            item.TextName = Rtf.ToHtml(item.TextName);
                    }
                    else
                        item.TextName = item.ItemName;
                }

                return dbData;
            }
            return null;
        }

        private async Task<List<IssusWorkGroupViewModel>> GetDataIssusWorkGroup(ScrollViewModel Scroll)
        {
            if (Scroll != null)
            {
                string sWhere = "STJ.TRSTYP_0 = 2 AND REGFLG_0 = 1";
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
                                                     $@"(LOWER(STJ.ITMREF_0) LIKE '%{keyword}%'
                                                        OR LOWER(ITM.ITMDES1_0 ) LIKE '%{keyword}%'
                                                        OR LOWER(STJ.STU_0 ) LIKE '%{keyword}%'
                                                        OR LOWER(STJ.CCE_0 ) LIKE '%{keyword}%'
                                                        OR LOWER(STJ.CCE_2 ) LIKE '%{keyword}%'
                                                        OR LOWER(STJ.CCE_3) LIKE '%{keyword}%')";
                }

                // Where Item WorkGroup
                if (Scroll.WhereBanks.Any())
                {
                    var list = new List<string>();

                    foreach (var item in Scroll.WhereBanks)
                        list.Add($"'{item}'");

                    var workGroups = string.Join(',', list);
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"STJ.CCE_3 IN ({workGroups})";
                }

                // Where Item Branch
                if (Scroll.WhereBranchs.Any())
                {
                    var list = new List<string>();

                    foreach (var item in Scroll.WhereBranchs)
                        list.Add($"'{item}'");

                    var branchs = string.Join(',', list);
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"STJ.CCE_0 IN ({branchs})";
                }

                // Where Item Project
                if (Scroll.WhereProjects.Any())
                {
                    var list = new List<string>();

                    foreach (var item in Scroll.WhereProjects)
                        list.Add($"'{item}'");

                    var projects = string.Join(',', list);
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"STJ.CCE_2 IN ({projects})";
                }

                // Range Item
                if (!string.IsNullOrEmpty(Scroll.WhereRange11))
                {
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"STJ.ITMREF_0 >= '{Scroll.WhereRange11}'";
                }

                if (!string.IsNullOrEmpty(Scroll.WhereRange12))
                {
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"STJ.ITMREF_0 <= '{Scroll.WhereRange12}'";
                }

                // Range Catetory
                if (!string.IsNullOrEmpty(Scroll.WhereRange21))
                {
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"ITM.TCLCOD_0 >= '{Scroll.WhereRange11}'";
                }

                if (!string.IsNullOrEmpty(Scroll.WhereRange22))
                {
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"ITM.TCLCOD_0 <= '{Scroll.WhereRange12}'";
                }

                // Where Date Range
                if (Scroll.SDate.HasValue)
                {
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"STJ.IPTDAT_0 >= '{Scroll.SDate.Value.ToString("yyyy-MM-dd")}'";
                }
                if (Scroll.EDate.HasValue)
                {
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"STJ.IPTDAT_0 <= '{Scroll.EDate.Value.ToString("yyyy-MM-dd")}'";
                }

                #endregion Where

                #region Sort

                switch (Scroll.SortField)
                {
                    case "ItemNo":
                        if (Scroll.SortOrder == -1)
                            sSort = $"STJ.ITMREF_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"STJ.ITMREF_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "ItemName":
                        if (Scroll.SortOrder == -1)
                            sSort = $"ITM.ITMDES1_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pjth0);
                        else
                            sSort = $"ITM.ITMDES1_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pjth0);
                        break;

                    case "QuantityString":
                        if (Scroll.SortOrder == -1)
                            sSort = $"SUM(STJ.QTYSTU_0) DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"SUM(STJ.QTYSTU_0) ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "Uom":
                        if (Scroll.SortOrder == -1)
                            sSort = $"STJ.STU_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"STJ.STU_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "Branch":
                        if (Scroll.SortOrder == -1)
                            sSort = $"STJ.CCE_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"STJ.CCE_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "Project":
                        if (Scroll.SortOrder == -1)
                            sSort = $"STJ.CCE_2 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"STJ.CCE_2 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "WorkGroup":
                        if (Scroll.SortOrder == -1)
                            sSort = $"STJ.CCE_3 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"STJ.CCE_3 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    default:
                        sSort = $"STJ.ITMREF_0 ASC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        break;
                }

                #endregion Sort

                var sqlCommnad = new SqlCommandViewModel()
                {
                    SelectCommand = $@"	STJ.ITMREF_0 AS [ItemNo],
                                        ITM.ITMDES1_0 AS [ItemName],
                                        TXT.TEXTE_0 AS [TextName],
                                        STJ.STU_0 AS [Uom],
                                        STJ.CCE_0 AS [Branch],
                                        STJ.CCE_2 AS [Project],
                                        STJ.CCE_3 AS [WorkGroup],
                                        (SELECT WG.TEXTE_0
                                         FROM VIPCO.ATEXTRA WG
                                         WHERE WG.IDENT1_0 = 'WG' AND WG.ZONE_0 = 'DESTRA' AND
                                               WG.CODFIC_0 = 'CACCE' AND WG.IDENT2_0 = STJ.CCE_3) AS WorkGroupName,
                                        SUM(STJ.QTYSTU_0) AS [Quantity],
                                        SUM(STJ.PRIVAL_0) AS [UnitPrice],
                                        SUM(STJ.QTYSTU_0 * STJ.PRIVAL_0) AS [TotalCost] ",
                    FromCommand = $@" VIPCO.STOJOU STJ
                                        LEFT OUTER JOIN [VIPCO].[ITMMASTER] ITM
                                            ON STJ.ITMREF_0 = ITM.ITMREF_0
                                        LEFT OUTER JOIN VIPCO.TEXCLOB TXT
                                            ON TXT.CODE_0 = ITM.PURTEX_0 ",
                    WhereCommand = sWhere,
                    OrderCommand = sSort,
                    GroupCommand = $@" STJ.ITMREF_0,ITM.ITMDES1_0,
                                        TXT.TEXTE_0, STJ.STU_0,
                                        STJ.CCE_0,STJ.CCE_2,
                                        STJ.CCE_3 "
                };

                var result = await this.repositoryIssus.GetEntitiesAndTotal(sqlCommnad, new { Skip = Scroll.Skip ?? 0, Take = Scroll.Take ?? 50 });
                var dbData = result.Entities;
                Scroll.TotalRow = result.TotalRow;

                foreach (var item in dbData)
                {
                    if (!string.IsNullOrEmpty(item.TextName))
                    {
                        if (item.TextName.StartsWith("{\\rtf1"))
                            item.TextName = Rtf.ToHtml(item.TextName);
                    }
                    else
                        item.TextName = item.ItemName;

                    item.TextName = this.helperService.ConvertHtmlToText(item.TextName);
                    item.TextName = item.TextName.Replace("\r\n", "");
                    item.TextName = item.TextName.Replace("\n", "");
                }

                return dbData;
            }
            return null;
        }

        private string ConvertHtmlToText(string Html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(Html);

            var htmlBody = htmlDoc.DocumentNode;
            // return htmlBody.OuterHtml;
            var sb = new StringBuilder();
            foreach (var node in htmlBody.DescendantsAndSelf())
            {
                if (!node.HasChildNodes)
                {
                    string text = node.InnerText;
                    text = text.Replace("&nbsp;", "");

                    if (!string.IsNullOrEmpty(text))
                        sb.AppendLine(text.Trim());
                }
            }
            return sb.ToString();
        }

        #endregion PrivateMethod

        // POST: api/StockOnHand/GetScroll
        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            var Message = "";

            try
            {
                var HasData = await this.GetData(Scroll);
                return new JsonResult(new ScrollDataViewModel<StockOnHandViewModel>(Scroll, HasData), this.DefaultJsonSettings);
            }
            catch (Exception ex)
            {
                Message = $"{ex.ToString()}";
            }
            return BadRequest(new { Error = Message });
        }

        // POST: api/StockOnHand/GetReport/
        [HttpPost("GetReport")]
        public async Task<IActionResult> GetReport([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();
            var Message = "";
            // Set Take all
            Scroll.Take = -1;
            var MapDatas = await this.GetData(Scroll);
            try
            {
                if (MapDatas.Any())
                {
                    var table = new DataTable();
                    //Adding the Columns
                    table.Columns.AddRange(new DataColumn[]
                    {
                        new DataColumn("Item Code", typeof(string)),
                        new DataColumn("Item Desc.", typeof(string)),
                        new DataColumn("Category Code",typeof(string)),
                        new DataColumn("Category",typeof(string)),
                        new DataColumn("Location",typeof(string)),
                        new DataColumn("LocationStock",typeof(string)),
                        new DataColumn("Uom",typeof(string)),
                        new DataColumn("Project",typeof(string)),
                        new DataColumn("LotNo",typeof(string)),
                        new DataColumn("HeatNo",typeof(string)),
                        new DataColumn("Origin",typeof(string)),
                        new DataColumn("ExpDate",typeof(string)),
                    });

                    //Adding the Rows
                    foreach (var item in MapDatas)
                    {
                        item.ItemDesc = this.helperService.ConvertHtmlToText(item.ItemDescFull);
                        item.ItemDesc = item.ItemDesc.Replace("\r\n", "");
                        item.ItemDesc = item.ItemDesc.Replace("\n", "");

                        if (item.StockLocations != null && item.StockLocations.Any())
                        {
                            foreach (var subitem in item.StockLocations)
                            {
                                table.Rows.Add(
                                    item.ItemCode,
                                    item.ItemDesc,
                                    item.Category,
                                    item.CategoryDesc,
                                    subitem.LocationCode,
                                    subitem.QuantityString,
                                    subitem.Uom,
                                    subitem.Project,
                                    subitem.LotNo,
                                    subitem.HeatNo,
                                    subitem.Origin,
                                    subitem.ExpDateString
                                );
                            }
                        }
                        else
                        {
                            table.Rows.Add(
                                    item.ItemCode,
                                    item.ItemDesc,
                                    item.Category,
                                    item.CategoryDesc,
                                    "-",
                                    "0",
                                    item.Uom
                                );
                        }
                    }

                    return File(this.helperService.CreateExcelFile(table, "StockOnHand")
                        , "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export.xlsx");
                }
            }
            catch (Exception ex)
            {
                Message = $"{ex.ToString()}";
            }

            return BadRequest();
        }

        [HttpPost("OnHandMk2GetScroll")]
        public async Task<IActionResult> OnHandMk2GetScroll([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            var Message = "";
            try
            {
                var MapDatas = await this.GetDataOnHandMk2V2(Scroll);
                return new JsonResult(new ScrollDataViewModel<OnHandMk2ViewMode>(Scroll, MapDatas), this.DefaultJsonSettings);
            }
            catch (Exception ex)
            {
                Message = $"{ex.ToString()}";
            }
            return BadRequest(new { Message });
        }

        [HttpPost("OnHandMk2GetReport")]
        public async Task<IActionResult> OnHandMk2GetReport([FromBody] ScrollViewModel Scroll)
        {
            var Message = "Data not been found.";
            try
            {
                if (Scroll != null)
                {
                    var MapDatas = await this.GetDataOnHandMk2V2(Scroll);

                    if (MapDatas.Any())
                    {
                        var table = new DataTable();
                        //Adding the Columns
                        foreach (var field in MapDatas[0].GetType().GetProperties()) // Loop through fields
                        {
                            string name = field.Name; // Get string name
                            var value = field.GetValue(MapDatas[0], null);

                            if (value is DateTime || value is double || value is int || value is null)
                                continue;

                            if (name == "ItemName")
                                continue;

                            table.Columns.Add(field.Name.Replace("String", ""), typeof(string));
                        }

                        //Adding the Rows
                        // Table1
                        foreach (var item in MapDatas)
                        {
                            item.TextName = this.helperService.ConvertHtmlToText(item.TextName);
                            item.TextName = item.TextName.Replace("\r\n", "");
                            item.TextName = item.TextName.Replace("\n", "");

                            table.Rows.Add(
                                item.LocationCode,
                                item.ItemNo,
                                (string.IsNullOrEmpty(item.TextName) ? item.ItemName : item.TextName),
                                item.ItemCate,
                                item.LotCode,
                                item.QuantityString,
                                item.Uom,
                                item.UnitPriceString,
                                item.AmountString,
                                item.OrderDateString);
                        }

                        var file = this.helperService.CreateExcelFilePivotTables(table, "StockOnHand", "StockOnHandPivot");
                        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StockOnHand.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                Message = $"Has error{ex.ToString()}";
            }
            return BadRequest(new { Error = Message });
        }

        [HttpPost("BalanceGetScroll")]
        public async Task<IActionResult> BalanceGetScroll([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            var Message = "";
            try
            {
                var MapDatas = await this.GetDataBalance(Scroll);
                return new JsonResult(new ScrollDataViewModel<StockBalanceViewModel>(Scroll, MapDatas), this.DefaultJsonSettings);
            }
            catch (Exception ex)
            {
                Message = $"{ex.ToString()}";
            }
            return BadRequest(new { Message });
        }

        [HttpPost("BalanceGetReport")]
        public async Task<IActionResult> BalanceGetReport([FromBody] ScrollViewModel Scroll)
        {
            var Message = "Data not been found.";
            try
            {
                if (Scroll != null)
                {
                    var MapDatas = await this.GetDataBalance(Scroll);

                    if (MapDatas.Any())
                    {
                        var table = new DataTable();
                        //Adding the Columns
                        foreach (var field in MapDatas[0].GetType().GetProperties()) // Loop through fields
                        {
                            string name = field.Name; // Get string name
                            var value = field.GetValue(MapDatas[0], null);

                            if (value is DateTime || value is double || value is int || value is null)
                                continue;

                            if (name == "ItemName")
                                continue;

                            table.Columns.Add(field.Name.Replace("String", ""), typeof(string));
                        }

                        //Adding the Rows
                        // Table1
                        foreach (var item in MapDatas)
                        {
                            item.TextName = this.helperService.ConvertHtmlToText(item.TextName);
                            item.TextName = item.TextName.Replace("\r\n", "");
                            item.TextName = item.TextName.Replace("\n", "");

                            table.Rows.Add(
                                item.LocationCode,
                                item.ItemNo,
                                item.ItemCate,
                                (string.IsNullOrEmpty(item.TextName) ? item.ItemName : item.TextName),
                                item.QuantityString,
                                item.Uom,
                                item.Available,
                                item.ReOrderString);
                        }

                        var file = this.helperService.CreateExcelFile(table, "StockBalance");
                        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StockOnHand.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                Message = $"Has error{ex.ToString()}";
            }
            return BadRequest(new { Error = Message });
        }

        [HttpPost("IssusWorkGroupGetScroll")]
        public async Task<IActionResult> IssusWorkGroupGetScroll([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            var Message = "";
            try
            {
                var MapDatas = await this.GetDataIssusWorkGroup(Scroll);
                return new JsonResult(new ScrollDataViewModel<IssusWorkGroupViewModel>(Scroll, MapDatas), this.DefaultJsonSettings);
            }
            catch (Exception ex)
            {
                Message = $"{ex.ToString()}";
            }
            return BadRequest(new { Message });
        }

        [HttpPost("IssusWorkGroupGetReport")]
        public async Task<IActionResult> IssusWorkGroupGetReport([FromBody] ScrollViewModel Scroll)
        {
            var Message = "Data not been found.";
            try
            {
                if (Scroll != null)
                {
                    var MapDatas = await this.GetDataIssusWorkGroup(Scroll);

                    if (MapDatas.Any())
                    {
                        var memory = new MemoryStream();
                        using (var wb = this.excelWbService.Create())
                        {
                            var ws = wb.Worksheets.Add("IssueByWorkGroup");
                  
                            ws.Cell(1, 1).Value = "รายงานการเบิกใช้";
                            ws.Cell(1, 1).DataType = XLDataType.Text;
                            ws.Range(1, 1, 1, 5).Merge().AddToNamed("Titles");

                            var StartDate = Scroll.SDate != null ? Scroll.SDate.Value.ToString("dd/MM/yy") : "-";
                            var EndDate = Scroll.EDate != null ? Scroll.EDate.Value.ToString("dd/MM/yy") : DateTime.Today.ToString("dd/MM/yy");
                            ws.Cell(2, 1).Value = $"วันที่ { StartDate } - { EndDate } (วันที่เริ่มงาน-วันที่จบงาน)";
                            ws.Cell(2, 1).DataType = XLDataType.Text;
                            ws.Range(2, 1, 2, 5).Merge().AddToNamed("Titles");

                            var Project = Scroll.WhereProjects.Any() ? string.Join(",", Scroll.WhereProjects) : "All";
                            ws.Cell(3, 1).Value = $"Project Number : {Project}";
                            ws.Cell(3, 1).DataType = XLDataType.Text;
                            ws.Range(3, 1, 3, 5).Merge().AddToNamed("Titles");

                            var Branch = Scroll.WhereBranchs.Any() ? string.Join(",", Scroll.WhereBranchs) : "All";
                            ws.Cell(4, 1).Value = $"Branch : { Branch }";
                            ws.Cell(4, 1).DataType = XLDataType.Text;
                            ws.Range(4, 1, 4, 5).Merge().AddToNamed("Titles");

                            // Move to the next row (it now has the titles)
                            var hasData = MapDatas.GroupBy(z => new { z.WorkGroup, z.WorkGroupName }).ToList();

                            var startRow = 6;
                            foreach (var wg in hasData.OrderBy(x => x.Key.WorkGroup))
                            {
                                ws.Cell(startRow, 1).Value = $"{wg.Key.WorkGroup} | {wg.Key.WorkGroupName}";
                                ws.Range(startRow, 1, startRow, 5).Merge().AddToNamed("Titles2");
                                var rowData = wg.GroupBy(z => new
                                {
                                    z.ItemNo,
                                    z.TextName,
                                    z.Uom
                                }).Select(
                                    x => new
                                    {
                                        x.Key.ItemNo,
                                        x.Key.TextName,
                                        x.Key.Uom,
                                        Quantity = x.Sum(z => z.Quantity >= 0 ? z.Quantity : z.Quantity * -1),
                                        Cost = x.Sum(z => z.TotalCost >= 0 ? z.TotalCost : z.TotalCost * -1)
                                    }).ToList();

                                var tableData = ws.Cell(startRow + 1, 1).InsertTable(rowData);
                                tableData.ShowTotalsRow = true;
                                tableData.Field("Cost").TotalsRowFunction = XLTotalsRowFunction.Sum;
                                // Just for fun let's add the text "Sum Of Income" to the totals row
                                tableData.Field(0).TotalsRowLabel = "Sum Of Cost";

                                startRow = startRow + 2 + tableData.RowCount();
                            }

                            // Prepare the style for the titles
                            var titlesStyle = wb.Style;
                            titlesStyle.Font.Bold = true;
                            titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            titlesStyle.Fill.BackgroundColor = XLColor.LightSkyBlue;
                            // Format all titles in one shot
                            wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;

                            var titlesStyle2 = titlesStyle;
                            titlesStyle2.Fill.BackgroundColor = XLColor.LightSteelBlue;
                            wb.NamedRanges.NamedRange("Titles2").Ranges.Style = titlesStyle2;


                            ws.Columns().AdjustToContents();

                            wb.SaveAs(memory);
                        }

                        memory.Position = 0;
                        return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StockOnHand.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                Message = $"Has error{ex.ToString()}";
            }
            return BadRequest(new { Error = Message });
        }

        private void ExtractCategoriesCompanies(string northwinddataXlsx,
            out List<string> categories,
            out List<string> companies)
        {
            categories = new List<string>();
            const int coCategoryId = 1;
            const int coCategoryName = 2;

            var wb = new XLWorkbook(northwinddataXlsx);
            var ws = wb.Worksheet("Data");

            // Look for the first row used
            var firstRowUsed = ws.FirstRowUsed();

            // Narrow down the row so that it only includes the used part
            var categoryRow = firstRowUsed.RowUsed();

            // Move to the next row (it now has the titles)
            categoryRow = categoryRow.RowBelow();

            // Get all categories
            while (!categoryRow.Cell(coCategoryId).IsEmpty())
            {
                String categoryName = categoryRow.Cell(coCategoryName).GetString();
                categories.Add(categoryName);

                categoryRow = categoryRow.RowBelow();
            }

            // There are many ways to get the company table.
            // Here we're using a straightforward method.
            // Another way would be to find the first row in the company table
            // by looping while row.IsEmpty()

            // First possible address of the company table:
            var firstPossibleAddress = ws.Row(categoryRow.RowNumber()).FirstCell().Address;
            // Last possible address of the company table:
            var lastPossibleAddress = ws.LastCellUsed().Address;

            // Get a range with the remainder of the worksheet data (the range used)
            var companyRange = ws.Range(firstPossibleAddress, lastPossibleAddress).RangeUsed();

            // Treat the range as a table (to be able to use the column names)
            var companyTable = companyRange.AsTable();

            // Get the list of company names
            companies = companyTable.DataRange.Rows()
              .Select(companyRow => companyRow.Field("Company Name").GetString())
              .ToList();
        }
    }
}