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
using Microsoft.AspNetCore.Authorization;

namespace VipcoSageX3.Controllers.SageX3
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : GenericSageX3Controller<Paymenth>
    {
        private readonly IRepositorySageX3<Bank> repositoryBank;
        private readonly IHostingEnvironment hosting;
        public PaymentController(IRepositorySageX3<Paymenth> repo,
            IRepositorySageX3<Bank> repoBank,
            IHostingEnvironment hosting,
            IMapper mapper) : base(repo, mapper)
        {
            this.repositoryBank = repoBank;
            this.hosting = hosting;
        }

        // POST: api/Payment/GetScroll
        //[Authorize(Policy = "SuperUser")]
        //[Authorize(Policy = "Admin")]
        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            // Filter
            var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                : Scroll.Filter.Split(null);

            var predicate = PredicateBuilder.False<Paymenth>();

            foreach (string temp in filters)
            {
                string keyword = temp.ToLower();
                predicate = predicate.Or(x => x.Chqnum0.ToLower().Contains(keyword) ||
                                              x.Cur0.ToLower().Contains(keyword) ||
                                              x.Des0.ToLower().Contains(keyword) ||
                                              x.Bpr0.ToLower().Contains(keyword) ||
                                              x.Num0.ToLower().Contains(keyword) ||
                                              x.Ref0.ToLower().Contains(keyword) ||
                                              x.Bpainv0.ToLower().Contains(keyword) ||
                                              x.Bpanam0.ToLower().Contains(keyword));
            }
            // Bank
            if (!string.IsNullOrEmpty(Scroll.WhereBank))
                predicate = predicate.And(x => x.Ban0 == Scroll.WhereBank);
            // Supplier
            if (!string.IsNullOrEmpty(Scroll.WhereSupplier))
                predicate = predicate.And(x => x.Bpr0 == Scroll.WhereSupplier);

            if (Scroll.SDate.HasValue)
            {
                Scroll.SDate = Scroll.SDate.Value.AddHours(7);
                predicate = predicate.And(x => x.Accdat0.Date >= Scroll.SDate.Value.Date);
            }

            if (Scroll.EDate.HasValue)
            {
                Scroll.EDate = Scroll.EDate.Value.AddHours(7);
                predicate = predicate.And(x => x.Accdat0.Date <= Scroll.EDate.Value.Date);
            }

            if (Scroll.WhereBanks.Any())
                predicate = predicate.And(x => Scroll.WhereBanks.Contains(x.Ban0));

            //Order by
            Func<IQueryable<Paymenth>, IOrderedQueryable<Paymenth>> order;
            // Order
            switch (Scroll.SortField)
            {
                case "AmountString":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Amtcur0);
                    else
                        order = o => o.OrderBy(x => x.Amtcur0);
                    break;
                case "BankNo":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Ban0);
                    else
                        order = o => o.OrderBy(x => x.Ban0);
                    break;
                case "CheckNo":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Chqnum0);
                    else
                        order = o => o.OrderBy(x => x.Chqnum0);
                    break;
                case "Currency":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Cur0);
                    else
                        order = o => o.OrderBy(x => x.Cur0);
                    break;
                case "Description":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Des0);
                    else
                        order = o => o.OrderBy(x => x.Des0);
                    break;
                case "PayBy":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Bpr0);
                    else
                        order = o => o.OrderBy(x => x.Bpr0);
                    break;
                case "PaymentDateString":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Accdat0);
                    else
                        order = o => o.OrderBy(x => x.Accdat0);
                    break;
                case "PaymentNo":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Num0);
                    else
                        order = o => o.OrderBy(x => x.Num0);
                    break;
                case "SupplierNo":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Bpainv0);
                    else
                        order = o => o.OrderBy(x => x.Bpainv0);
                    break;
                case "SupplierName":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Bpanam0);
                    else
                        order = o => o.OrderBy(x => x.Bpanam0);
                    break;
                default:
                    order = o => o.OrderByDescending(x => x.Accdat0);
                    break;
            }

            var QueryData = await this.repository.GetToListAsync(
                                    selector: selected => selected,  // Selected
                                    predicate: predicate, // Where
                                    orderBy: order, // Order
                                    include: null, // Include
                                    skip: Scroll.Skip ?? 0, // Skip
                                    take: Scroll.Take ?? 15); // Take

            // Get TotalRow
            Scroll.TotalRow = await this.repository.GetLengthWithAsync(predicate: predicate);

            var mapDatas = new List<PaymentViewModel>();
            foreach (var item in QueryData)
            {
                var MapItem = this.mapper.Map<Paymenth, PaymentViewModel>(item);
                //Project Name
                if (!string.IsNullOrEmpty(item.Ban0))
                {
                    MapItem.BankName += await this.repositoryBank.GetFirstOrDefaultAsync(z => z.Des0, z => z.Ban0 == item.Ban0) ?? "-";
                }
                mapDatas.Add(MapItem);
            }

            return new JsonResult(new ScrollDataViewModel<PaymentViewModel>(Scroll, mapDatas), this.DefaultJsonSettings);
        }

        //[Authorize(Policy = "SuperUser")]
        //[Authorize(Policy = "Admin")]
        [HttpPost("GetReport")]
        public async Task<IActionResult> GetReport([FromBody] ScrollViewModel Scroll)
        {
            var Message = "";
            try
            {
                if (Scroll == null)
                    return BadRequest();

                // Filter
                var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                    : Scroll.Filter.Split(null);

                var predicate = PredicateBuilder.False<Paymenth>();

                foreach (string temp in filters)
                {
                    string keyword = temp.ToLower();
                    predicate = predicate.Or(x => x.Chqnum0.ToLower().Contains(keyword) ||
                                                  x.Cur0.ToLower().Contains(keyword) ||
                                                  x.Des0.ToLower().Contains(keyword) ||
                                                  x.Bpr0.ToLower().Contains(keyword) ||
                                                  x.Num0.ToLower().Contains(keyword) ||
                                                  x.Ref0.ToLower().Contains(keyword) ||
                                                  x.Bpainv0.ToLower().Contains(keyword) ||
                                                  x.Bpanam0.ToLower().Contains(keyword));
                }
                // Bank
                if (!string.IsNullOrEmpty(Scroll.WhereBank))
                    predicate = predicate.And(x => x.Ban0 == Scroll.WhereBank);
                // Supplier
                if (!string.IsNullOrEmpty(Scroll.WhereSupplier))
                    predicate = predicate.And(x => x.Bpr0 == Scroll.WhereSupplier);

                if (Scroll.SDate.HasValue)
                {
                    Scroll.SDate = Scroll.SDate.Value.AddHours(7);
                    predicate = predicate.And(x => x.Accdat0.Date >= Scroll.SDate.Value.Date);
                }

                if (Scroll.EDate.HasValue)
                {
                    Scroll.EDate = Scroll.EDate.Value.AddHours(7);
                    predicate = predicate.And(x => x.Accdat0.Date <= Scroll.EDate.Value.Date);
                }

                if (Scroll.WhereBanks.Any())
                {
                    predicate = predicate.And(x => Scroll.WhereBanks.Contains(x.Ban0));
                }
                //Order by
                Func<IQueryable<Paymenth>, IOrderedQueryable<Paymenth>> order;
                // Order
                switch (Scroll.SortField)
                {
                    case "AmountString":
                        if (Scroll.SortOrder == -1)
                            order = o => o.OrderByDescending(x => x.Amtcur0);
                        else
                            order = o => o.OrderBy(x => x.Amtcur0);
                        break;
                    case "BankNo":
                        if (Scroll.SortOrder == -1)
                            order = o => o.OrderByDescending(x => x.Ban0);
                        else
                            order = o => o.OrderBy(x => x.Ban0);
                        break;
                    case "CheckNo":
                        if (Scroll.SortOrder == -1)
                            order = o => o.OrderByDescending(x => x.Chqnum0);
                        else
                            order = o => o.OrderBy(x => x.Chqnum0);
                        break;
                    case "Currency":
                        if (Scroll.SortOrder == -1)
                            order = o => o.OrderByDescending(x => x.Cur0);
                        else
                            order = o => o.OrderBy(x => x.Cur0);
                        break;
                    case "Description":
                        if (Scroll.SortOrder == -1)
                            order = o => o.OrderByDescending(x => x.Des0);
                        else
                            order = o => o.OrderBy(x => x.Des0);
                        break;
                    case "PayBy":
                        if (Scroll.SortOrder == -1)
                            order = o => o.OrderByDescending(x => x.Bpr0);
                        else
                            order = o => o.OrderBy(x => x.Bpr0);
                        break;
                    case "PaymentDateString":
                        if (Scroll.SortOrder == -1)
                            order = o => o.OrderByDescending(x => x.Accdat0);
                        else
                            order = o => o.OrderBy(x => x.Accdat0);
                        break;
                    case "PaymentNo":
                        if (Scroll.SortOrder == -1)
                            order = o => o.OrderByDescending(x => x.Num0);
                        else
                            order = o => o.OrderBy(x => x.Num0);
                        break;
                    case "SupplierNo":
                        if (Scroll.SortOrder == -1)
                            order = o => o.OrderByDescending(x => x.Bpainv0);
                        else
                            order = o => o.OrderBy(x => x.Bpainv0);
                        break;
                    case "SupplierName":
                        if (Scroll.SortOrder == -1)
                            order = o => o.OrderByDescending(x => x.Bpanam0);
                        else
                            order = o => o.OrderBy(x => x.Bpanam0);
                        break;
                    default:
                        order = o => o.OrderByDescending(x => x.Accdat0);
                        break;
                }

                var QueryData = await this.repository.GetToListAsync(
                                        selector: selected => selected,  // Selected
                                        predicate: predicate, // Where
                                        orderBy: order, // Order
                                        include: null // Include
                                        );

                var mapDatas = new List<PaymentViewModel>();
                foreach (var item in QueryData)
                {
                    var MapItem = this.mapper.Map<Paymenth, PaymentViewModel>(item);
                    //Project Name
                    if (!string.IsNullOrEmpty(item.Ban0))
                    {
                        MapItem.BankName += await this.repositoryBank.GetFirstOrDefaultAsync(z => z.Des0, z => z.Ban0 == item.Ban0) ?? "-";
                    }
                    mapDatas.Add(MapItem);
                }

                if (mapDatas.Any())
                {
                    var table = new DataTable();
                    //Adding the Columns
                    table.Columns.AddRange(new DataColumn[]
                    {
                        new DataColumn("PaymentDate", typeof(string)),
                        new DataColumn("PaymentNo.", typeof(string)),
                        new DataColumn("SupplierName",typeof(string)),
                        new DataColumn("BankAmount",typeof(string)),
                        new DataColumn("BankAmount2",typeof(string)),
                        new DataColumn("CheckNo",typeof(string)),
                        new DataColumn("Description",typeof(string)),
                        new DataColumn("BankName",typeof(string)),
                        new DataColumn("BankNo",typeof(string)),
                        new DataColumn("RefNo",typeof(string)),
                        new DataColumn("PayBy",typeof(string)),
                        new DataColumn("SupplierNo",typeof(string)),
                    });

                    //Adding the Rows
                    foreach (var item in mapDatas)
                    {
                        table.Rows.Add(
                            item.PaymentDateString,
                            item.PaymentNo,
                            item.SupplierName,
                            item.AmountString,
                            item.Amount2String,
                            item.CheckNo,
                            item.Description,
                            item.BankName,
                            item.BankNo,
                            item.RefNo,
                            item.PayBy,
                            item.SupplierNo
                        );
                    }

                    var templateFolder = this.hosting.WebRootPath + "/reports/";
                    var fileExcel = templateFolder + $"Payment_Report.xlsx";

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var wsFreeze = wb.Worksheets.Add(table, "PaymentReport");
                        wsFreeze.SheetView.FreezeRows(1);
                        wb.SaveAs(fileExcel);
                    }

                    var memory = new MemoryStream();
                    using (var stream = new FileStream(fileExcel, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;

                    // "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reports.xlsx"
                    // "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Reports.docx"
                    // stream.Position = 0;
                    // return File(byteArray, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reports.xlsx");

                    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Payment_Report.xlsx");
                }

                return NoContent();
            }
            catch(Exception ex)
            {
                Message = $"Has error {ex.ToString()}";
            }

            return BadRequest(new { Error = Message });
        }

        public async Task<IActionResult> GetReport2([FromBody] ScrollViewModel Scroll)
        {
            //if (Scroll == null)
            //    return BadRequest();


            // Filter
            var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                : Scroll.Filter.Split(null);

            var predicate = PredicateBuilder.False<Paymenth>();

            foreach (string temp in filters)
            {
                string keyword = temp.ToLower();
                predicate = predicate.Or(x => x.Chqnum0.ToLower().Contains(keyword) ||
                                              x.Cur0.ToLower().Contains(keyword) ||
                                              x.Des0.ToLower().Contains(keyword) ||
                                              x.Bpr0.ToLower().Contains(keyword) ||
                                              x.Num0.ToLower().Contains(keyword) ||
                                              x.Ref0.ToLower().Contains(keyword) ||
                                              x.Bpainv0.ToLower().Contains(keyword) ||
                                              x.Bpanam0.ToLower().Contains(keyword));
            }
            // Bank
            if (!string.IsNullOrEmpty(Scroll.WhereBank))
                predicate = predicate.And(x => x.Ban0 == Scroll.WhereBank);
            // Supplier
            if (!string.IsNullOrEmpty(Scroll.WhereSupplier))
                predicate = predicate.And(x => x.Bpr0 == Scroll.WhereSupplier);

            if (Scroll.SDate.HasValue)
            {
                Scroll.SDate = Scroll.SDate.Value.AddHours(7);
                predicate = predicate.And(x => x.Accdat0.Date >= Scroll.SDate.Value.Date);
            }

            if (Scroll.EDate.HasValue)
            {
                Scroll.EDate = Scroll.EDate.Value.AddHours(7);
                predicate = predicate.And(x => x.Accdat0.Date <= Scroll.EDate.Value.Date);
            }

            if (Scroll.WhereBanks.Any())
            {
                predicate = predicate.And(x => Scroll.WhereBanks.Contains(x.Ban0));
            }
            //Order by
            Func<IQueryable<Paymenth>, IOrderedQueryable<Paymenth>> order;
            // Order
            switch (Scroll.SortField)
            {
                case "AmountString":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Amtcur0);
                    else
                        order = o => o.OrderBy(x => x.Amtcur0);
                    break;
                case "BankNo":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Ban0);
                    else
                        order = o => o.OrderBy(x => x.Ban0);
                    break;
                case "CheckNo":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Chqnum0);
                    else
                        order = o => o.OrderBy(x => x.Chqnum0);
                    break;
                case "Currency":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Cur0);
                    else
                        order = o => o.OrderBy(x => x.Cur0);
                    break;
                case "Description":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Des0);
                    else
                        order = o => o.OrderBy(x => x.Des0);
                    break;
                case "PayBy":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Bpr0);
                    else
                        order = o => o.OrderBy(x => x.Bpr0);
                    break;
                case "PaymentDateString":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Accdat0);
                    else
                        order = o => o.OrderBy(x => x.Accdat0);
                    break;
                case "PaymentNo":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Num0);
                    else
                        order = o => o.OrderBy(x => x.Num0);
                    break;
                case "SupplierNo":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Bpainv0);
                    else
                        order = o => o.OrderBy(x => x.Bpainv0);
                    break;
                case "SupplierName":
                    if (Scroll.SortOrder == -1)
                        order = o => o.OrderByDescending(x => x.Bpanam0);
                    else
                        order = o => o.OrderBy(x => x.Bpanam0);
                    break;
                default:
                    order = o => o.OrderByDescending(x => x.Accdat0);
                    break;
            }

            var QueryData = await this.repository.GetToListAsync(
                                    selector: selected => selected,  // Selected
                                    predicate: predicate, // Where
                                    orderBy: order, // Order
                                    include: null // Include
                                    );

            var mapDatas = new List<PaymentViewModel>();
            foreach (var item in QueryData)
            {
                var MapItem = this.mapper.Map<Paymenth, PaymentViewModel>(item);
                //Project Name
                if (!string.IsNullOrEmpty(item.Ban0))
                {
                    MapItem.BankName += await this.repositoryBank.GetFirstOrDefaultAsync(z => z.Des0, z => z.Ban0 == item.Ban0) ?? "-";
                }
                mapDatas.Add(MapItem);
            }

            if (mapDatas.Any())
            {
                var table = new DataTable();
                //Adding the Columns
                table.Columns.AddRange(new DataColumn[]
                {
                        new DataColumn("PaymentNo.", typeof(string)),
                        new DataColumn("PaymentDateString", typeof(string)),
                        new DataColumn("BankNo",typeof(string)),
                        new DataColumn("BankName",typeof(string)),
                        new DataColumn("PayBy",typeof(string)),
                        new DataColumn("SupplierNo",typeof(string)),
                        new DataColumn("SupplierName",typeof(string)),
                        new DataColumn("AmountString",typeof(string)),
                        new DataColumn("Description",typeof(string)),
                        new DataColumn("CheckNo",typeof(string)),
                        new DataColumn("RefNo",typeof(string)),
                });

                //Adding the Rows
                foreach (var item in mapDatas)
                {
                    table.Rows.Add(
                        item.PaymentNo,
                        item.PaymentDateString,
                        item.BankNo,
                        item.BankName,
                        item.PayBy,
                        item.SupplierNo,
                        item.SupplierName,
                        item.AmountString,
                        item.Description,
                        item.CheckNo,
                        item.RefNo
                    );
                }

                var stream = new MemoryStream();
                using (XLWorkbook wb = new XLWorkbook())
                {
                    var wsFreeze = wb.Worksheets.Add(table, "PaymentReport");
                    wsFreeze.SheetView.FreezeRows(1);
                    wb.SaveAs(stream);
                }


                stream.Seek(0, SeekOrigin.Begin);
                var byteArray = stream.ToArray();

                // "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reports.xlsx"
                // "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Reports.docx"
                // stream.Position = 0;
                // return File(byteArray, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reports.xlsx");

                const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Response.ContentType = contentType;
                HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");

                var fileContentResult = new FileContentResult(byteArray, contentType)
                {
                    FileDownloadName = "Reports.xlsx"
                };

                return fileContentResult;
            }

            return NoContent();
        }
    }
}
