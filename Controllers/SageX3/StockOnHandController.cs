using AutoMapper;
using ClosedXML.Excel;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//3rd Party
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
        private readonly IHostingEnvironment hosting;
        private readonly IHelperService helperService;

        public StockOnHandController(IRepositorySageX3<Itmmvt> repo,
            IRepositorySageX3<Stock> repoStock,
            IRepositorySageX3<Stolot> repoStockLot,
            IHostingEnvironment hosting,
            IHelperService helperService,
            IMapper mapper, SageX3Context x3Context) : base(repo, mapper)
        {
            // Repoistory
            this.repositoryStock = repoStock;
            this.repositoryStockLot = repoStockLot;
            // Host
            this.hosting = hosting;
            // Helper
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
                        Uom =  string.IsNullOrEmpty(item?.nProductMaster?.Pcu0.Trim()) ? item?.nProductMaster?.Stu0 : item?.nProductMaster?.Pcu0,
                    };

                    // Set Stock
                    var ListStock = await this.repositoryStock.GetToListAsync(x => x, x => x.Itmref0 == MapData.ItemCode);
                    if (ListStock != null && ListStock.Any())
                    {
                        foreach (var stock in ListStock.GroupBy(x => new { x.Loc0,x.Pcu0,x.Pjt0,x.Lot0,x.Bpslot0,x.Palnum0 }))
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

        #endregion

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

                    return File(this.helperService.CreateExcelFile(table,"StockOnHand")
                        , "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export.xlsx");
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