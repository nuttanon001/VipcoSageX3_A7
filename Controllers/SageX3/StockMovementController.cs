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
        private readonly IHostingEnvironment hosting;

        public StockMovementController(IRepositorySageX3<Stojou> repo,
            IRepositorySageX3<Preceiptd> repoReceipt,
            IHostingEnvironment hosting,
            IMapper mapper, SageX3Context x3Context) : base(repo, mapper)
        {
            // Repoistory
            this.repositoryReceipt = repoReceipt;
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

        #region BackUp
        //private async Task<List<StockMovementViewModel>> GetData2(ScrollViewModel Scroll)
        //{
        //    #region Query

        //    var QueryData = (from StockJou in this.sageContext.Stojou
        //                     join ProductMaster in this.sageContext.Itmmaster on StockJou.Itmref0 equals ProductMaster.Itmref0 into ProductMaster2
        //                     from nProductMaster in ProductMaster2.DefaultIfEmpty()
        //                     join ProductCate in this.sageContext.Itmcateg on nProductMaster.Tclcod0 equals ProductCate.Tclcod0 into ProductCate2
        //                     from nProductCate in ProductCate2.DefaultIfEmpty()
        //                     join aText in this.sageContext.Atextra on new { code1 = nProductCate.Tclcod0, code2 = "TCLAXX" } equals new { code1 = aText.Ident10, code2 = aText.Zone0 } into AText
        //                     from nAText in AText.DefaultIfEmpty()
        //                     join bText in this.sageContext.Texclob on new { Code0 = nProductMaster.Purtex0 } equals new { bText.Code0 } into bText2
        //                     from fullText in bText2.DefaultIfEmpty()
        //                     select new
        //                     {
        //                         StockJou,
        //                         nProductMaster,
        //                         nProductCate,
        //                         nAText,
        //                         fullText,
        //                     }).AsQueryable();

        //    #endregion Query

        //    #region Filter

        //    // Filter
        //    var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
        //                        : Scroll.Filter.Split(null);

        //    foreach (string temp in filters)
        //    {
        //        string keyword = temp.ToLower();
        //        QueryData = QueryData.Where(x => x.nAText.Texte0.ToLower().Contains(keyword) ||
        //                                         x.nProductMaster.Itmdes10.ToLower().Contains(keyword) ||
        //                                         x.nProductMaster.Itmdes20.ToLower().Contains(keyword) ||
        //                                         x.nProductMaster.Itmdes30.ToLower().Contains(keyword) ||
        //                                         x.StockJou.Itmref0.ToLower().Contains(keyword));
        //    }

        //    // Product Category
        //    if (Scroll.WhereBanks.Any())
        //        QueryData = QueryData.Where(x => Scroll.WhereBanks.Contains(x.nProductMaster.Tclcod0));

        //    #endregion Filter

        //    #region Scroll

        //    switch (Scroll.SortField)
        //    {
        //        case "ItemCode":
        //            if (Scroll.SortOrder == -1)
        //                QueryData = QueryData.OrderByDescending(x => x.nProductMaster.Itmref0);
        //            else
        //                QueryData = QueryData.OrderBy(x => x.nProductMaster.Itmref0);
        //            break;

        //        case "ItemDescFull":
        //            if (Scroll.SortOrder == -1)
        //                QueryData = QueryData.OrderByDescending(x => x.nProductMaster.Itmdes10);
        //            else
        //                QueryData = QueryData.OrderBy(x => x.nProductMaster.Itmdes10);
        //            break;

        //        case "Uom":
        //            if (Scroll.SortOrder == -1)
        //                QueryData = QueryData.OrderByDescending(x => x.nProductMaster.Stu0);
        //            else
        //                QueryData = QueryData.OrderBy(x => x.nProductMaster.Stu0);
        //            break;

        //        case "Category":
        //            if (Scroll.SortOrder == -1)
        //                QueryData = QueryData.OrderByDescending(x => x.nProductCate.Tclcod0);
        //            else
        //                QueryData = QueryData.OrderBy(x => x.nProductCate.Tclcod0);
        //            break;

        //        case "CategoryDesc":
        //            if (Scroll.SortOrder == -1)
        //                QueryData = QueryData.OrderByDescending(x => x.nAText.Texte0);
        //            else
        //                QueryData = QueryData.OrderBy(x => x.nAText.Texte0);
        //            break;

        //        case "DocNo":
        //            if (Scroll.SortOrder == -1)
        //                QueryData = QueryData.OrderByDescending(x => x.StockJou.Vcrnum0);
        //            else
        //                QueryData = QueryData.OrderBy(x => x.StockJou.Vcrnum0);
        //            break;

        //        case "Location":
        //            if (Scroll.SortOrder == -1)
        //                QueryData = QueryData.OrderByDescending(x => x.StockJou.Loc0);
        //            else
        //                QueryData = QueryData.OrderBy(x => x.StockJou.Loc0);
        //            break;

        //        case "Bom":
        //            if (Scroll.SortOrder == -1)
        //                QueryData = QueryData.OrderByDescending(x => x.StockJou.Cce1);
        //            else
        //                QueryData = QueryData.OrderBy(x => x.StockJou.Cce1);
        //            break;

        //        case "Project":
        //            if (Scroll.SortOrder == -1)
        //                QueryData = QueryData.OrderByDescending(x => x.StockJou.Cce2);
        //            else
        //                QueryData = QueryData.OrderBy(x => x.StockJou.Cce2);
        //            break;

        //        case "WorkGroup":
        //            if (Scroll.SortOrder == -1)
        //                QueryData = QueryData.OrderByDescending(x => x.StockJou.Cce3);
        //            else
        //                QueryData = QueryData.OrderBy(x => x.StockJou.Cce3);
        //            break;

        //        case "MovementType":
        //            if (Scroll.SortOrder == -1)
        //                QueryData = QueryData.OrderByDescending(x => x.StockJou.Vcrtyp0);
        //            else
        //                QueryData = QueryData.OrderBy(x => x.StockJou.Vcrtyp0);
        //            break;

        //        case "MovementDateString":
        //            if (Scroll.SortOrder == -1)
        //                QueryData = QueryData.OrderByDescending(x => x.StockJou.Iptdat0);
        //            else
        //                QueryData = QueryData.OrderBy(x => x.StockJou.Iptdat0);
        //            break;

        //        case "QuantityString":
        //            if (Scroll.SortOrder == -1)
        //                QueryData = QueryData.OrderByDescending(x => x.StockJou.Qtypcu0);
        //            else
        //                QueryData = QueryData.OrderBy(x => x.StockJou.Qtypcu0);
        //            break;

        //        default:
        //            QueryData = QueryData.OrderByDescending(x => x.nProductMaster.Itmref0).ThenBy(x => x.StockJou.Iptdat0);
        //            break;
        //    }

        //    #endregion Scroll

        //    Scroll.TotalRow = await QueryData.CountAsync();
        //    var Message = "";
        //    try
        //    {
        //        var Datasource = await QueryData.GroupBy(x => new
        //        {
        //            x.StockJou.Vcrnum0,
        //            x.StockJou.Vcrtyp0,
        //            x.StockJou.Itmref0,
        //            x.StockJou.Iptdat0,
        //        }).OrderBy(x => x.Key.Itmref0).ThenBy(x => x.Key.Iptdat0)
        //        .Skip(Scroll.Skip ?? 0).Take(Scroll.Take ?? 15).AsNoTracking().ToListAsync();

        //        var MapDatas = new List<StockMovementViewModel>();

        //        var Purchase = new List<int>() { 6, 8 };
        //        var Stock = new List<int>() { 19, 20, 31, 32 };
        //        var Sale = new List<int>() { 5, 13 };

        //        foreach (var item in Datasource)
        //        {
        //            var MapData = new StockMovementViewModel()
        //            {
        //                Category = item?.FirstOrDefault()?.nProductCate?.Tclcod0 ?? "",
        //                CategoryDesc = item?.FirstOrDefault()?.nAText?.Texte0 ?? "",
        //                Bom = item?.FirstOrDefault()?.StockJou?.Cce1 ?? "",
        //                DocNo = item?.Key?.Vcrnum0 ?? "",
        //                Location = item?.FirstOrDefault()?.StockJou?.Loc0 ?? "",
        //                MovementDate = item?.Key?.Iptdat0,
        //                Project = item?.FirstOrDefault()?.StockJou?.Cce2 ?? "",
        //                QuantityIn = (double)item?.Where(x => x.StockJou.Qtypcu0 > 0)?.Sum(x => x?.StockJou?.Qtypcu0 ?? (decimal)0),
        //                QuantityOut = (double)item?.Where(x => x.StockJou.Qtypcu0 <= 0)?.Sum(x => x?.StockJou?.Qtypcu0 ?? (decimal)0),
        //                WorkGroup = item?.FirstOrDefault()?.StockJou?.Cce3 ?? "",
        //                ItemCode = item?.FirstOrDefault()?.nProductMaster?.Itmref0,
        //                ItemDesc = item?.FirstOrDefault()?.nProductMaster?.Itmdes10,
        //                Uom = item?.FirstOrDefault()?.nProductMaster?.Stu0,
        //                MovementType = Purchase.Contains(item.Key.Vcrtyp0) ? "Purchase" :
        //                               (Stock.Contains(item.Key.Vcrtyp0) ? "Stock" :
        //                               (Sale.Contains(item.Key.Vcrtyp0) ? "Sale" : "Stock"))
        //            };

        //            //ItemName
        //            if (item?.FirstOrDefault()?.fullText?.Texte0 != null)
        //            {
        //                if (item.FirstOrDefault().fullText.Texte0.StartsWith("{\\rtf1"))
        //                    MapData.ItemDescFull = Rtf.ToHtml(item?.FirstOrDefault()?.fullText?.Texte0);
        //                else
        //                    MapData.ItemDescFull = item?.FirstOrDefault()?.fullText?.Texte0;
        //            }
        //            else
        //                MapData.ItemDescFull = item?.FirstOrDefault()?.fullText?.Texte0 ?? "-";

        //            MapDatas.Add(MapData);
        //        }

        //        return MapDatas;
        //    }
        //    catch (Exception ex)
        //    {
        //        Message = $"{ex.ToString()}";
        //    }

        //    return null;
        //}

        // POST: api/StockMovement/GetScroll
        #endregion

        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            var Message = "";
            try
            {
                var HasData = await this.GetData(Scroll);
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
                        new DataColumn("Uom",typeof(string)),
                        new DataColumn("Category",typeof(string)),
                        new DataColumn("Uom",typeof(string)),
                    });

                    //Adding the Rows
                    foreach (var item in MapDatas)
                    {
                        table.Rows.Add(
                            item.ItemCode,
                            item.ItemDesc,
                            item.Category,
                            item.CategoryDesc,
                            item.Uom
                        );
                    }

                    var templateFolder = this.hosting.WebRootPath + "/reports/";
                    var fileExcel = templateFolder + $"StockMove_Report.xlsx";

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var wsFreeze = wb.Worksheets.Add(table, "StockMovementReport");
                        wsFreeze.Columns().AdjustToContents();
                        wsFreeze.SheetView.FreezeRows(1);
                        wb.SaveAs(fileExcel);
                    }

                    var memory = new MemoryStream();
                    using (var stream = new FileStream(fileExcel, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;

                    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Payment_Report.xlsx");
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
