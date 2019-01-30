using AutoMapper;
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

namespace VipcoSageX3.Controllers.SageX3
{
    // : api/PurchaseRequest
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseRequestController : GenericSageX3Controller<Prequisd>
    {
        private readonly IRepositorySageX3<Cacce> repositoryDim;
        private readonly IRepositorySageX3<Prequiso> repositoryPRLink;
        private readonly IRepositorySageX3<Porderq> repositoryPOLine;
        private readonly IRepositorySageX3<Porder> repositoryPoHeader;
        private readonly IRepositorySageX3<Preceiptd> repositoryRCLine;
        private readonly IRepositorySageX3<Cptanalin> repositoryDimLink;
        private readonly IRepositorySageX3<Itmmaster> repositoryItem;
        private readonly IRepositoryDapperSageX3<PurchaseRequestAndOrderViewModel> repositoryPrAndPo;
        private readonly IRepositoryDapperSageX3<PurchaseReceiptViewModel> repositoryPrq;
        private readonly IHelperService helperService;
        private readonly ExcelWorkBookService excelWorkBookService;
        private readonly IHostingEnvironment hosting;

        //Context
        private readonly SageX3Context sageContext;

        // GET: api/PurchaseRequest
        public PurchaseRequestController(
            IRepositorySageX3<Prequisd> repo,
            IRepositorySageX3<Prequiso> repoPrLink,
            IRepositorySageX3<Porderq> repoPoLine,
            IRepositorySageX3<Preceiptd> repoRcLine,
            IRepositorySageX3<Cptanalin> repoDimLink,
            IRepositorySageX3<Porder> repoPoHeader,
            IRepositorySageX3<Cacce> repoDim,
            IRepositorySageX3<Itmmaster> repoItem,
            IRepositoryDapperSageX3<PurchaseRequestAndOrderViewModel> repoPrAndPo,
            IRepositoryDapperSageX3<PurchaseReceiptViewModel> repoPrq,
            IHelperService helper,
            SageX3Context x3Context,
            IHostingEnvironment hosting,
            ExcelWorkBookService  workBookService,
            IMapper mapper) : base(repo, mapper)
        {
            // Repository SageX3
            this.repositoryDim = repoDim;
            this.repositoryDimLink = repoDimLink;
            this.repositoryPoHeader = repoPoHeader;
            this.repositoryPOLine = repoPoLine;
            this.repositoryPRLink = repoPrLink;
            this.repositoryRCLine = repoRcLine;
            this.repositoryPrAndPo = repoPrAndPo;
            this.repositoryPrq = repoPrq;
            this.repositoryItem = repoItem;
            //DI
            this.helperService = helper;
            // Context
            this.sageContext = x3Context;
            // Hosting
            this.hosting = hosting;
            this.excelWorkBookService = workBookService;
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

        private async Task<List<PurchaseRequestAndOrderViewModel>> GetData(ScrollViewModel Scroll, bool option = false)
        {
            #region Query

            var QueryData = (from prh in this.sageContext.Prequis
                             join prd in this.sageContext.Prequisd on prh.Pshnum0 equals prd.Pshnum0 into new_pr
                             from all1 in new_pr.DefaultIfEmpty()
                             join itm in this.sageContext.Itmmaster on all1.Itmref0 equals itm.Itmref0 into new_pr_itm
                             from all2 in new_pr_itm.DefaultIfEmpty()
                             join dim in this.sageContext.Cptanalin on new { key1 = all1.Pshnum0, key2 = all1.Psdlin0 } equals new { key1 = dim.Vcrnum0, key2 = dim.Vcrlin0 } into new_pr_dim
                             from all3 in new_pr_dim.DefaultIfEmpty()
                             join pro in this.sageContext.Prequiso on new { all1.Pshnum0, all1.Psdlin0 } equals new { pro.Pshnum0, pro.Psdlin0 } into new_pr_lik
                             from all4 in new_pr_lik.DefaultIfEmpty()
                             join pod in this.sageContext.Porderq on new { all4.Pohnum0, all4.Poplin0 } equals new { pod.Pohnum0, pod.Poplin0 } into new_pr_po
                             from all5 in new_pr_po.DefaultIfEmpty()
                             join dim in this.sageContext.Cptanalin on
                                new { key1 = all5.Pohnum0, key2 = all5.Poplin0 } equals
                                new { key1 = dim.Vcrnum0, key2 = dim.Vcrlin0 } into new_po_dim
                             from all6 in new_po_dim.DefaultIfEmpty()
                             where all2.Tclcod0.StartsWith('1') //&& (all4.Pohnum0 == "PO1808-0006" || all4.Pohnum0 == "PO1808-0008" || all4.Pohnum0 == "PO1808-0072")
                             select new
                             {
                                 item = all2,
                                 pod = all5,
                                 prd = all1,
                                 prh,
                                 pro = all4,
                                 dim = all3,
                                 dimPo = all6
                             }).AsQueryable();

            #endregion Query

            #region Filter&Scroll

            // Filter
            var filters = string.IsNullOrEmpty(Scroll.Filter) ? new string[] { "" }
                                : Scroll.Filter.Split(null);
            foreach (string temp in filters)
            {
                string keyword = temp.ToLower();
                QueryData = QueryData.Where(x => x.prd.Pjt0.ToLower().Contains(keyword) ||
                                                 x.prd.Itmref0.ToLower().Contains(keyword) ||
                                                 x.prd.Itmdes0.ToLower().Contains(keyword) ||
                                                 x.prh.Creusr0.ToLower().Contains(keyword) ||
                                                 x.prd.Pshnum0.ToLower().Contains(keyword) ||
                                                 x.pod.Pohnum0.ToLower().Contains(keyword) ||
                                                 x.pod.Itmref0.ToLower().Contains(keyword));
            }

            if (!string.IsNullOrEmpty(Scroll.WhereBranch))
                QueryData = QueryData.Where(x => x.dim.Cce0 == Scroll.WhereBranch);

            if (!string.IsNullOrEmpty(Scroll.WhereWorkGroup))
                QueryData = QueryData.Where(x => x.dim.Cce3 == Scroll.WhereWorkGroup);

            if (!string.IsNullOrEmpty(Scroll.WhereWorkItem))
                QueryData = QueryData.Where(x => x.dim.Cce1 == Scroll.WhereWorkItem);

            if (!string.IsNullOrEmpty(Scroll.WhereProject))
                QueryData = QueryData.Where(x => x.dim.Cce2 == Scroll.WhereProject);

            if (Scroll.SDate.HasValue && Scroll.EDate.HasValue)
            {
                QueryData = QueryData.Where(x =>
                    x.prh.Prqdat0.Date >= Scroll.SDate.Value.Date &&
                    x.prh.Prqdat0.Date <= Scroll.EDate.Value.Date);
            }

            //MapData.Branch = PrDim.Cce0;
            //MapData.WorkItem = PrDim.Cce1;
            //MapData.Project = PrDim.Cce2;
            //MapData.WorkGroup = PrDim.Cce3;
            switch (Scroll.SortField)
            {
                case "PrNumber":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.prh.Pshnum0);
                    else
                        QueryData = QueryData.OrderBy(x => x.prh.Pshnum0);
                    break;

                case "Project":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.prh.Pjth0);
                    else
                        QueryData = QueryData.OrderBy(x => x.prh.Pjth0);
                    break;

                case "PRDateString":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.prh.Prqdat0);
                    else
                        QueryData = QueryData.OrderBy(x => x.prh.Prqdat0);
                    break;

                case "ItemName":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.prd.Itmdes0);
                    else
                        QueryData = QueryData.OrderBy(x => x.prd.Itmdes0);
                    break;

                case "Branch":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.dim.Cce0);
                    else
                        QueryData = QueryData.OrderBy(x => x.dim.Cce0);
                    break;

                case "WorkItemName":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.dim.Cce1);
                    else
                        QueryData = QueryData.OrderBy(x => x.dim.Cce1);
                    break;

                case "WorkGroupName":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.dim.Cce3);
                    else
                        QueryData = QueryData.OrderBy(x => x.dim.Cce3);
                    break;

                case "PoNumber":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.pod.Pohnum0);
                    else
                        QueryData = QueryData.OrderBy(x => x.pod.Pohnum0);
                    break;

                case "PoDateString":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.pod.Orddat0);
                    else
                        QueryData = QueryData.OrderBy(x => x.pod.Orddat0);
                    break;

                case "DueDateString":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.pod.Extrcpdat0);
                    else
                        QueryData = QueryData.OrderBy(x => x.pod.Extrcpdat0);
                    break;

                case "CreateBy":
                    if (Scroll.SortOrder == -1)
                        QueryData = QueryData.OrderByDescending(x => x.prh.Creusr0);
                    else
                        QueryData = QueryData.OrderBy(x => x.prh.Creusr0);
                    break;

                default:
                    QueryData = QueryData.OrderByDescending(x => x.prh.Prqdat0);
                    break;
            }

            Scroll.TotalRow = await QueryData.CountAsync();

            #endregion Filter&Scroll

            #region MyRegion

            var DataSource = await QueryData.Skip(Scroll.Skip ?? 0).Take(Scroll.Take ?? 15).AsNoTracking().ToListAsync();
            var MapDatas = new List<PurchaseRequestAndOrderViewModel>();
            var ListCCE = new List<string>();

            foreach (var item in DataSource)
            {
                var MapData = this.mapper.Map<Prequis, PurchaseRequestAndOrderViewModel>(item.prh);
                MapData.ToDate = DateTime.Today.AddDays(-2);

                this.mapper.Map<Prequisd, PurchaseRequestAndOrderViewModel>(item.prd, MapData);
                // PrWeight
                if (item?.item?.Itmwei0 > 0)
                    MapData.PrWeight = (double)item.item.Itmwei0 * MapData.QuantityPur;

                //ItemName
                if (!string.IsNullOrEmpty(item?.item?.Purtex0))
                {
                    var fulltext = await this.sageContext.Texclob.Where(x => x.Code0 == item.item.Purtex0)
                                  .Select(x => x.Texte0)
                                  .FirstOrDefaultAsync();

                    if (!string.IsNullOrEmpty(fulltext))
                    {
                        if (fulltext.StartsWith("{\\rtf1") && !option)
                            MapData.ItemName = Rtf.ToHtml(fulltext);
                        else
                            MapData.ItemName = fulltext;
                    }
                    else
                        MapData.ItemName = MapData.ItemName;
                }
                else
                    MapData.ItemName = MapData.ItemName;

                // Get Dimension for PurchaseRequest1
                if (item.dim != null)
                {
                    MapData.Branch = item.dim.Cce0;
                    MapData.WorkItem = item.dim.Cce1;
                    MapData.Project = item.dim.Cce2;
                    MapData.WorkGroup = item.dim.Cce3;
                    // Add CCE
                    ListCCE.Add(item.dim.Cce0);
                    ListCCE.Add(item.dim.Cce1);
                    ListCCE.Add(item.dim.Cce2);
                    ListCCE.Add(item.dim.Cce3);
                }

                // Get Purchase Request Link
                if (item.pro != null)
                {
                    this.mapper.Map<Prequiso, PurchaseRequestAndOrderViewModel>(item.pro, MapData);
                    // Get Purchase Order
                    if (item.pod != null)
                    {
                        // Get Status PurchaseOrder
                        var PoHeader = await this.repositoryPoHeader.GetFirstOrDefaultAsync(x => new Porder()
                        {
                            Pohnum0 = x.Pohnum0,
                            Zpo210 = x.Zpo210,
                            Cleflg0 = x.Cleflg0,
                            Rowid = x.Rowid
                        }, x => x.Pohnum0 == item.pod.Pohnum0);

                        if (PoHeader != null)
                            this.mapper.Map<Porder, PurchaseRequestAndOrderViewModel>(PoHeader, MapData);

                        this.mapper.Map<Porderq, PurchaseRequestAndOrderViewModel>(item.pod, MapData);
                        if (item.dimPo != null)
                        {
                            MapData.PoBranch = item.dimPo.Cce0;
                            MapData.PoWorkItem = item.dimPo.Cce1;
                            MapData.PoProject = item.dimPo.Cce2;
                            MapData.PoWorkGroup = item.dimPo.Cce3;
                            // Add CCE
                            ListCCE.Add(item.dimPo.Cce0);
                            ListCCE.Add(item.dimPo.Cce1);
                            ListCCE.Add(item.dimPo.Cce2);
                            ListCCE.Add(item.dimPo.Cce3);
                        }

                        //var ListRcs = await this.repositoryRCLine.GetToListAsync(
                        //    x => x,
                        //    x => x.Pohnum0 == item.pod.Pohnum0 && x.Poplin0 == item.pod.Poplin0 && x.Poqseq0 == item.pod.Poqseq0);

                        var ReciptionLine = await (from prc in this.sageContext.Preceiptd
                                                   join sto in this.sageContext.Stojou on
                                                       new { key1 = prc.Pthnum0, key2 = prc.Ptdlin0 } equals
                                                       new { key1 = sto.Vcrnum0, key2 = sto.Vcrlin0 } into new_sto
                                                   from all1 in new_sto.DefaultIfEmpty()
                                                   where prc.Pohnum0 == item.pod.Pohnum0 &&
                                                         prc.Poplin0 == item.pod.Poplin0 &&
                                                         prc.Poqseq0 == item.pod.Poqseq0 &&
                                                         ((all1.Vcrtypreg0 == 0 && all1.Regflg0 == 1) ||
                                                         (all1.Vcrtypreg0 == 17 && all1.Regflg0 == 1))
                                                   select new
                                                   {
                                                       preciptD = prc,
                                                       stock = all1,
                                                   }).ToListAsync();

                        if (ReciptionLine.Any())
                        {
                            foreach (var itemRc in ReciptionLine)
                            {
                                var RcMapData = this.mapper.Map<Preceiptd, PurchaseReceiptViewModel>(itemRc.preciptD);
                                RcMapData.HeatNumber = itemRc?.stock?.Lot0 ?? "";
                                RcMapData.HeatNumber += itemRc?.stock?.Slo0 ?? "";

                                var RcDim = await this.repositoryDimLink.GetFirstOrDefaultAsync(x => x, x => x.Vcrnum0 == itemRc.preciptD.Pthnum0 && x.Vcrlin0 == itemRc.preciptD.Ptdlin0);
                                if (RcDim != null)
                                {
                                    RcMapData.RcBranch = RcDim.Cce0;
                                    RcMapData.RcWorkItem = RcDim.Cce1;
                                    RcMapData.RcProject = RcDim.Cce2;
                                    RcMapData.RcWorkGroup = RcDim.Cce3;
                                    // Add CCE
                                    ListCCE.Add(RcDim.Cce0);
                                    ListCCE.Add(RcDim.Cce1);
                                    ListCCE.Add(RcDim.Cce2);
                                    ListCCE.Add(RcDim.Cce3);
                                }
                                // Add Rc to mapData
                                MapData.PurchaseReceipts.Add(RcMapData);
                            }
                        }
                        else
                            MapData.DeadLine = MapData.DueDate != null ? MapData.ToDate.Date > MapData.DueDate.Value.Date : false;
                    }
                }

                MapDatas.Add(MapData);
            }

            var allDim = await this.repositoryDim.GetToListAsync(x => new { Code = x.Cce0, Desc = x.Des0 }, x => ListCCE.Contains(x.Cce0));
            if (allDim.Any())
            {
                foreach (var item in MapDatas)
                {
                    //CCE name purchase request
                    item.BranchName = allDim.FirstOrDefault(x => x.Code == item.Branch)?.Desc ?? "-";
                    item.WorkGroupName = allDim.FirstOrDefault(x => x.Code == item.WorkGroup)?.Desc ?? "-";
                    item.WorkItemName = allDim.FirstOrDefault(x => x.Code == item.WorkItem)?.Desc ?? "-";
                    item.ProjectName = allDim.FirstOrDefault(x => x.Code == item.Project)?.Desc ?? "-";
                    //CCE name purchase order
                    item.PoBranchName = allDim.FirstOrDefault(x => x.Code == item.PoBranch)?.Desc ?? "-";
                    item.PoWorkGroupName = allDim.FirstOrDefault(x => x.Code == item.PoWorkGroup)?.Desc ?? "-";
                    item.PoWorkItemName = allDim.FirstOrDefault(x => x.Code == item.PoWorkItem)?.Desc ?? "-";
                    item.PoProjectName = allDim.FirstOrDefault(x => x.Code == item.PoProject)?.Desc ?? "-";
                    foreach (var item2 in item.PurchaseReceipts)
                    {
                        //CCE name purchase order
                        item2.RcBranchName = allDim.FirstOrDefault(x => x.Code == item2.RcBranch)?.Desc ?? "-";
                        item2.RcWorkGroupName = allDim.FirstOrDefault(x => x.Code == item2.RcWorkGroup)?.Desc ?? "-";
                        item2.RcWorkItemName = allDim.FirstOrDefault(x => x.Code == item2.RcWorkItem)?.Desc ?? "-";
                        item2.RcProjectName = allDim.FirstOrDefault(x => x.Code == item2.RcProject)?.Desc ?? "-";
                    }
                }
            }

            #endregion MyRegion

            return MapDatas;
        }


        private async Task<List<PurchaseRequestAndOrderViewModel>> GetData3(ScrollViewModel scroll)
        {
            if (scroll != null)
            {
                string sWhere = "WHERE [ITM].[TCLCOD_0] LIKE '1%'";
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
                                                    $@"(LOWER(PRD.PJT_0) LIKE '%{keyword}%'
                                                        OR LOWER([PRD].[ITMREF_0]) LIKE '%{keyword}%'
                                                        OR LOWER([PRD].[ITMDES_0]) LIKE '%{keyword}%'
                                                        OR LOWER([PRH].[CREUSR_0]) LIKE '%{keyword}%'
                                                        OR LOWER([PRD].[PSHNUM_0]) LIKE '%{keyword}%'
                                                        OR LOWER([POD].[POHNUM_0]) LIKE '%{keyword}%'
                                                        OR LOWER([POD].[ITMREF_0]) LIKE '%{keyword}%')";
                }

                if (!string.IsNullOrEmpty(scroll.WhereBranch))
                    sWhere += (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"DIM.CCE_0 = '{scroll.WhereBranch}'";

                if (!string.IsNullOrEmpty(scroll.WhereWorkGroup))
                    sWhere += (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"DIM.CCE_3 = '{scroll.WhereWorkGroup}'";

                if (!string.IsNullOrEmpty(scroll.WhereWorkItem))
                    sWhere += (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"DIM.CCE_1 = '{scroll.WhereWorkItem}'";

                if (!string.IsNullOrEmpty(scroll.WhereProject))
                    sWhere += (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"DIM.CCE_2 = '{scroll.WhereProject}'";

                if (scroll.SDate.HasValue)
                    sWhere +=
                        (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"PRH.PRQDAT_0 >= '{scroll.SDate.Value.AddHours(7).ToString("yyyy-MM-dd")}'";

                if (scroll.EDate.HasValue)
                    sWhere +=
                        (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"PRH.PRQDAT_0 <= '{scroll.EDate.Value.AddHours(7).ToString("yyyy-MM-dd")}'";

                #endregion Where

                #region Sort

                switch (scroll.SortField)
                {
                    case "PrNumber":
                        if (scroll.SortOrder == -1)
                            sSort = $"PRH.PSHNUM_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.prh.Pshnum0);
                        else
                            sSort = $"PRH.PSHNUM_0 ASC";//QueryData = QueryData.OrderBy(x => x.prh.Pshnum0);
                        break;

                    case "Project":
                        if (scroll.SortOrder == -1)
                            sSort = $"DIM.CCE_2 DESC";//QueryData = QueryData.OrderByDescending(x => x.prh.Pjth0);
                        else
                            sSort = $"DIM.CCE_2 ASC";//QueryData = QueryData.OrderBy(x => x.prh.Pjth0);
                        break;

                    case "PRDateString":
                        if (scroll.SortOrder == -1)
                            sSort = $"PRH.PRQDAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.prh.Prqdat0);
                        else
                            sSort = $"PRH.PRQDAT_0 ASC";//QueryData = QueryData.OrderBy(x => x.prh.Prqdat0);
                        break;

                    case "ItemName":
                        if (scroll.SortOrder == -1)
                            sSort = $"PRD.ITMDES_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.prd.Itmdes0);
                        else
                            sSort = $"PRD.ITMDES_0 ASC";//QueryData = QueryData.OrderBy(x => x.prd.Itmdes0);
                        break;

                    case "Branch":
                        if (scroll.SortOrder == -1)
                            sSort = $"DIM.CCE_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.dim.Cce0);
                        else
                            sSort = $"DIM.CCE_0 ASC";//QueryData = QueryData.OrderBy(x => x.dim.Cce0);
                        break;

                    case "WorkItemName":
                        if (scroll.SortOrder == -1)
                            sSort = $"DIM.CCE_1 DESC";//QueryData = QueryData.OrderByDescending(x => x.dim.Cce1);
                        else
                            sSort = $"DIM.CCE_1 ASC";//QueryData = QueryData.OrderBy(x => x.dim.Cce1);
                        break;

                    case "WorkGroupName":
                        if (scroll.SortOrder == -1)
                            sSort = $"DIM.CCE_3 DESC";//QueryData = QueryData.OrderByDescending(x => x.dim.Cce3);
                        else
                            sSort = $"DIM.CCE_3 ASC";//QueryData = QueryData.OrderBy(x => x.dim.Cce3);
                        break;

                    case "PoNumber":
                        if (scroll.SortOrder == -1)
                            sSort = $"POD.POHNUM_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.pod.Pohnum0);
                        else
                            sSort = $"POD.POHNUM_0 ASC";//QueryData = QueryData.OrderBy(x => x.pod.Pohnum0);
                        break;

                    case "PoDateString":
                        if (scroll.SortOrder == -1)
                            sSort = $"POD.ORDDAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.pod.Orddat0);
                        else
                            sSort = $"POD.ORDDAT_0 ASC";//QueryData = QueryData.OrderBy(x => x.pod.Orddat0);
                        break;

                    case "DueDateString":
                        if (scroll.SortOrder == -1)
                            sSort = $"POD.EXTRCPDAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.pod.Extrcpdat0);
                        else
                            sSort = $"POD.EXTRCPDAT_0 ASC";//QueryData = QueryData.OrderBy(x => x.pod.Extrcpdat0);
                        break;

                    case "CreateBy":
                        if (scroll.SortOrder == -1)
                            sSort = $"PRH.CREUSR_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.prh.Creusr0);
                        else
                            sSort = $"PRH.CREUSR_0 ASC";//QueryData = QueryData.OrderBy(x => x.prh.Creusr0);
                        break;

                    case "RequestDateString":
                        if (scroll.SortOrder == -1)
                            sSort = $"PRD.EXTRCPDAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.prh.Creusr0);
                        else
                            sSort = $"PRD.EXTRCPDAT_0 ASC";//QueryData = QueryData.OrderBy(x => x.prh.Creusr0);
                        break;
                      
                    default:
                        sSort = $"PRH.PRQDAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.prh.Prqdat0);
                        break;
                }

                #endregion Sort

                #region Query

                // Query mulitple command
                sQuery = $@"SELECT		--PRH
                                        PRH.CLEFLG_0 AS [PrCloseStatusInt],
                                        PRH.CREUSR_0 AS [CreateBy],
                                        PRH.PRQDAT_0 AS [PRDate],
                                        --PRD
                                        PRD.EXTRCPDAT_0 AS [RequestDate],
                                        PRD.PSHNUM_0 AS [PrNumber],
                                        PRD.PSDLIN_0 AS [PrLine],
                                        PRD.ITMREF_0 AS [ItemCode],
                                        PRD.PUU_0 AS [PurUom],
                                        PRD.STU_0 AS [StkUom],
                                        PRD.QTYPUU_0 AS [QuantityPur],
                                        PRD.QTYSTU_0 AS [QuantityStk],
                                        --ITM
                                        ITM.ITMWEI_0 AS [ItemWeight],
                                        TXT.TEXTE_0 AS [ItemName],
                                        --PRO
                                        PRO.POHNUM_0 AS [LinkPoNumber],
                                        PRO.POPLIN_0 AS [LinkPoLine],
                                        PRO.POQSEQ_0 AS [LinkPoSEQ],
                                        --POH
                                        POH.CLEFLG_0 AS [CloseStatusInt],
                                        POH.ZPO21_0 AS [PoStatusInt],
                                        --POD
                                        POD.POHNUM_0 AS [PoNumber],
                                        POD.POPLIN_0 AS [PoLine],
                                        POD.POQSEQ_0 AS [PoSequence],
                                        POD.ORDDAT_0 AS [PoDate],
                                        POD.EXTRCPDAT_0 AS [DueDate],
                                        POD.PUU_0 AS [PoPurUom],
                                        POD.STU_0 AS [PoStkUom],
                                        POD.QTYPUU_0 AS [PoQuantityPur],
                                        POD.QTYSTU_0 AS [PoQuantityStk],
                                        POD.QTYWEU_0 AS [PoQuantityWeight],
                                        --DIM
                                        DIM.CCE_0 AS [Branch],
                                        DIM.CCE_1 AS [WorkItem],
                                        DIM.CCE_2 AS [Project],
                                        DIM.CCE_3 AS [WorkGroup],
                                        (SELECT CAC.DES_0 FROM VIPCO.CACCE CAC WHERE CAC.CCE_0 = DIM.CCE_0) AS [BranchName],
                                        (SELECT CAC.DES_0 FROM VIPCO.CACCE CAC WHERE CAC.CCE_0 = DIM.CCE_1) AS [WorkItemName],
                                        (SELECT CAC.DES_0 FROM VIPCO.CACCE CAC WHERE CAC.CCE_0 = DIM.CCE_2) AS [ProjectName],
                                        (SELECT CAC.DES_0 FROM VIPCO.CACCE CAC WHERE CAC.CCE_0 = DIM.CCE_3) AS [WorkGroupName],
                                        --DIMPO
                                        DIMPO.CCE_0 AS [PoBranch],
                                        DIMPO.CCE_1 AS [PoWorkItem],
                                        DIMPO.CCE_2 AS [PoProject],
                                        DIMPO.CCE_3 AS [PoWorkGroup],
                                        (SELECT CAC.DES_0 FROM VIPCO.CACCE CAC WHERE CAC.CCE_0 = DIMPO.CCE_0) AS [PoBranchName],
                                        (SELECT CAC.DES_0 FROM VIPCO.CACCE CAC WHERE CAC.CCE_0 = DIMPO.CCE_1) AS [PoWorkItemName],
                                        (SELECT CAC.DES_0 FROM VIPCO.CACCE CAC WHERE CAC.CCE_0 = DIMPO.CCE_2) AS [PoProjectName],
                                        (SELECT CAC.DES_0 FROM VIPCO.CACCE CAC WHERE CAC.CCE_0 = DIMPO.CCE_3) AS [PoWorkGroupName]
                            FROM		VIPCO.PREQUIS PRH
                                        INNER JOIN VIPCO.PREQUISD PRD
                                            ON PRH.PSHNUM_0 = PRD.PSHNUM_0
                                        LEFT OUTER JOIN VIPCO.PREQUISO PRO
                                            ON PRD.PSHNUM_0 = PRO.PSHNUM_0
                                            AND PRD.PSDLIN_0 = PRO.PSDLIN_0
                                        LEFT OUTER JOIN VIPCO.PORDER POH
                                            ON PRO.POHNUM_0 = POH.POHNUM_0
                                        LEFT OUTER JOIN VIPCO.PORDERQ POD
                                            ON PRO.POHNUM_0 = POD.POHNUM_0
                                            AND PRO.POPLIN_0 = POD.POPLIN_0
                                        LEFT OUTER JOIN VIPCO.CPTANALIN DIM
                                            ON DIM.ABRFIC_0 = 'PSD'
                                            AND DIM.VCRTYP_0 = 0
                                            AND DIM.VCRSEQ_0 = 0
                                            AND	DIM.CPLCLE_0 = ''
                                            AND	DIM.ANALIG_0 = 1
                                            AND PRD.PSHNUM_0 = DIM.VCRNUM_0
                                            AND PRD.PSDLIN_0 = DIM.VCRLIN_0
                                        LEFT OUTER JOIN VIPCO.CPTANALIN DIMPO
                                            ON DIMPO.ABRFIC_0 = 'POP'
                                            AND DIMPO.VCRTYP_0 = 0
                                            AND	POD.POQSEQ_0 = DIMPO.VCRSEQ_0
                                            AND POD.POHNUM_0 = DIMPO.VCRNUM_0
                                            AND POD.POPLIN_0 = DIMPO.VCRLIN_0
                                            AND	DIMPO.CPLCLE_0 = ''
                                            AND	DIMPO.ANALIG_0 = 1
                                        LEFT OUTER JOIN VIPCO.ITMMASTER ITM
                                            ON PRD.ITMREF_0 = ITM.ITMREF_0
                                        LEFT OUTER JOIN VIPCO.TEXCLOB TXT
                                            ON TXT.CODE_0 = ITM.PURTEX_0
                            {sWhere}
                            ORDER BY    {sSort}
                            OFFSET      @Skip ROWS       -- skip 10 rows
                            FETCH NEXT  @Take ROWS ONLY; -- take 10 rows;
                            SELECT	    COUNT(*)
                            FROM	    VIPCO.PREQUIS PRH
                                        INNER JOIN VIPCO.PREQUISD PRD
                                            ON PRH.PSHNUM_0 = PRD.PSHNUM_0
                                        LEFT OUTER JOIN VIPCO.PREQUISO PRO
                                            ON PRD.PSHNUM_0 = PRO.PSHNUM_0
                                            AND PRD.PSDLIN_0 = PRO.PSDLIN_0
                                        LEFT OUTER JOIN VIPCO.PORDER POH
                                            ON PRO.POHNUM_0 = POH.POHNUM_0
                                        LEFT OUTER JOIN VIPCO.PORDERQ POD
                                            ON PRO.POHNUM_0 = POD.POHNUM_0
                                            AND PRO.POPLIN_0 = POD.POPLIN_0
                                        LEFT OUTER JOIN VIPCO.CPTANALIN DIM
                                            ON DIM.ABRFIC_0 = 'PSD'
                                            AND DIM.VCRTYP_0 = 0
                                            AND DIM.VCRSEQ_0 = 0
                                            AND	DIM.CPLCLE_0 = ''
                                            AND	DIM.ANALIG_0 = 1
                                            AND PRD.PSHNUM_0 = DIM.VCRNUM_0
                                            AND PRD.PSDLIN_0 = DIM.VCRLIN_0
                                        LEFT OUTER JOIN VIPCO.CPTANALIN DIMPO
                                            ON DIMPO.ABRFIC_0 = 'POP'
                                            AND DIMPO.VCRTYP_0 = 0
                                            AND	POD.POQSEQ_0 = DIMPO.VCRSEQ_0
                                            AND POD.POHNUM_0 = DIMPO.VCRNUM_0
                                            AND POD.POPLIN_0 = DIMPO.VCRLIN_0
                                            AND	DIMPO.CPLCLE_0 = ''
                                            AND	DIMPO.ANALIG_0 = 1
                                        LEFT OUTER JOIN VIPCO.ITMMASTER ITM
                                            ON PRD.ITMREF_0 = ITM.ITMREF_0
                                        LEFT OUTER JOIN VIPCO.TEXCLOB TXT
                                            ON TXT.CODE_0 = ITM.PURTEX_0
                            {sWhere};";

                #endregion Query

                var result = await this.repositoryPrAndPo.GetListEntitesAndTotalRow(sQuery, new { Skip = scroll.Skip ?? 0, Take = scroll.Take ?? 15 });
                var dbData = result.Entities;
                scroll.TotalRow = result.TotalRow;

                string sReceipt = "";
                foreach (var item in dbData)
                {
                    item.ToDate = DateTime.Today.AddDays(-2);
                    if (!string.IsNullOrEmpty(item.ItemName))
                    {
                        if (item.ItemName.StartsWith("{\\rtf1"))
                            item.ItemName = Rtf.ToHtml(item.ItemName);
                    }
                    else
                        item.ItemName = await this.repositoryItem.GetFirstOrDefaultAsync(x => x.Itmdes10, x => x.Itmref0 == item.ItemCode);

                    if (item?.ItemWeight > 0)
                        item.PrWeight = (double)item.ItemWeight * item.QuantityPur;

                    #region Receipt

                    sReceipt = $@"SELECT	--STOJOU
                                            STO.LOT_0 AS [HeatNumber],
                                            STO.SLO_0 AS [HeatNumber2],
                                            STO.STA_0 AS [RcStatus],
                                            --PRECEIPTD
                                            PRC.PTHNUM_0 AS [RcNumber],
                                            PRC.PTDLIN_0 AS [RcLine],
                                            PRC.RCPDAT_0 AS [RcDate],
                                            PRC.PUU_0 AS [RcPurUom],
                                            PRC.STU_0 AS [RcStkUom],
                                            PRC.UOM_0 AS [RcUom],
                                            PRC.QTYPUU_0 AS [RcQuantityPur],
                                            PRC.QTYSTU_0 AS [RcQuantityStk],
                                            PRC.QTYUOM_0 AS [RcQuantityUom],
                                            PRC.QTYWEU_0 AS [RcQuantityWeight],
                                            PRC.INVQTYPUU_0 AS [RcQuantityInvPur],
                                            PRC.INVQTYSTU_0 AS [RcQuantityInvStk],
                                            --DIM
                                            DIM.CCE_0 AS [RcBranch],
                                            DIM.CCE_1 AS [RcWorkItem],
                                            DIM.CCE_2 AS [RcProject],
                                            DIM.CCE_3 AS [RcWorkGroup],
                                            (SELECT CAC.DES_0 FROM VIPCO.CACCE CAC WHERE CAC.CCE_0 = DIM.CCE_0) AS [RcBranchName],
                                            (SELECT CAC.DES_0 FROM VIPCO.CACCE CAC WHERE CAC.CCE_0 = DIM.CCE_1) AS [RcWorkItemName],
                                            (SELECT CAC.DES_0 FROM VIPCO.CACCE CAC WHERE CAC.CCE_0 = DIM.CCE_2) AS [RcProjectName],
                                            (SELECT CAC.DES_0 FROM VIPCO.CACCE CAC WHERE CAC.CCE_0 = DIM.CCE_3) AS [RcWorkGroupName]

                                FROM		[VIPCO].[STOJOU] STO WITH(INDEX(STOJOU_STJ0))
                                            LEFT OUTER JOIN [VIPCO].[PRECEIPTD] PRC
                                            ON STO.VCRNUM_0 = PRC.PTHNUM_0
                                            AND	STO.VCRLIN_0 = PRC.PTDLIN_0
                                            LEFT OUTER JOIN VIPCO.CPTANALIN DIM
                                                ON DIM.ABRFIC_0 = 'PTD'
                                                AND DIM.VCRTYP_0 = 0
                                                AND DIM.VCRSEQ_0 = 0
                                                AND	DIM.CPLCLE_0 = ''
                                                AND	DIM.ANALIG_0 = 1
                                                AND PRC.PTHNUM_0 = DIM.VCRNUM_0
                                                AND PRC.PTDLIN_0 = DIM.VCRLIN_0
                                WHERE		PRC.POHNUM_0 = @PoNumber
                                            AND PRC.POPLIN_0 = @PoLine
                                            AND PRC.POQSEQ_0 = @PoSequence
                                            AND ((STO.VCRTYPREG_0 = 0 AND STO.REGFLG_0 = 1) OR (STO.VCRTYPREG_0 = 17 AND STO.REGFLG_0 = 1))
                                ORDER BY	PRC.POPLIN_0 ASC";

                    var receipts = await this.repositoryPrq.GetListEntites(sReceipt, new { item.PoNumber, item.PoLine, item.PoSequence });
                    if (receipts.Any())
                    {
                        receipts.ForEach(receipt =>
                        {
                            if (string.IsNullOrEmpty(receipt.HeatNumber2))
                                receipt.HeatNumber += receipt.HeatNumber2;

                            item.PurchaseReceipts.Add(receipt);
                        });
                    }
                    else
                        item.DeadLine = item.DueDate != null ? item.ToDate.Date > item.DueDate.Value.Date : false;

                    #endregion Receipt
                }

                return dbData;
            }
            return null;
        }

        // POST: api/PurchaseRequest/GetScroll
        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            var Message = "";
            try
            {
                var MapDatas = await this.GetData3(Scroll);
                foreach (var item in MapDatas)
                {
                    item.ItemName = this.helperService.ConvertHtmlToText(item.ItemName);
                }
                return new JsonResult(new ScrollDataViewModel<PurchaseRequestAndOrderViewModel>(Scroll, MapDatas), this.DefaultJsonSettings);
            }
            catch (Exception ex)
            {
                Message = $"{ex.ToString()}";
            }
            return BadRequest(new { Message });
        }

        [HttpPost("GetReport")]
        public async Task<IActionResult> GetReport([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            var Message = "Data not been found.";
            try
            {
                // Set skip and take
                // Scroll.Skip = 0;
                // Scroll.Take = 999;

                var MapDatas = await this.GetData3(Scroll);

                if (MapDatas.Any())
                {
                    var table = new DataTable();
                    //Adding the Columns
                    table.Columns.AddRange(new DataColumn[]
                    {
                        new DataColumn("PrNo", typeof(string)),
                        new DataColumn("JobNo", typeof(string)),
                        new DataColumn("PrDate",typeof(string)),
                        new DataColumn("RequestDate",typeof(string)),
                        new DataColumn("Item-Code",typeof(string)),
                        new DataColumn("Item-Name",typeof(string)),
                        new DataColumn("Uom",typeof(string)),
                        new DataColumn("Branch",typeof(string)),
                        new DataColumn("BomLv",typeof(string)),
                        new DataColumn("WorkGroup",typeof(string)),
                        new DataColumn("Qty",typeof(int)),
                        new DataColumn("PrWeight",typeof(string)),
                        new DataColumn("PrClose",typeof(string)),
                        new DataColumn("Create",typeof(string)),

                        new DataColumn("PoNo",typeof(string)),
                        new DataColumn("JobNo-2",typeof(string)),
                        new DataColumn("PoDate",typeof(string)),
                        new DataColumn("DueDate",typeof(string)),
                        new DataColumn("QtyPo",typeof(int)),
                        new DataColumn("Weight",typeof(int)),
                        new DataColumn("Uom-2",typeof(string)),
                        new DataColumn("Branch-2",typeof(string)),
                        new DataColumn("BomLv-2",typeof(string)),
                        new DataColumn("WorkGroup-2",typeof(string)),
                        new DataColumn("TypePo",typeof(string)),
                        new DataColumn("PoStatus",typeof(string)),
                        new DataColumn("RcNumber",typeof(string)),
                        new DataColumn("RcStatus",typeof(string)),
                        new DataColumn("HeatNumber",typeof(string)),
                        new DataColumn("RcProject",typeof(string)),
                        new DataColumn("RcDateString",typeof(string)),
                        new DataColumn("RcQuantityPur",typeof(string)),
                        new DataColumn("RcQuantityWeight",typeof(string)),
                        new DataColumn("RcPurUom",typeof(string)),
                        new DataColumn("RcBranch",typeof(string)),
                        new DataColumn("RcWorkItemName",typeof(string)),
                        new DataColumn("RcWorkGroupName",typeof(string))
                    });

                    //Adding the Rows
                    foreach (var item in MapDatas)
                    {
                        item.ItemName = this.helperService.ConvertHtmlToText(item.ItemName);
                        item.ItemName = item.ItemName.Replace("\r\n", "");
                        item.ItemName = item.ItemName.Replace("\n", "");
                        //var Receipt = "";

                        if (item.PurchaseReceipts.Any())
                        {
                            foreach (var item2 in item.PurchaseReceipts)
                            {
                                table.Rows.Add(
                                    item.PrNumber,
                                    item.Project,
                                    item.PRDateString,
                                    item.RequestDateString,
                                    item.ItemCode,
                                    item.ItemName,
                                    item.PurUom,
                                    item.Branch,
                                    item.WorkItemName,
                                    item.WorkGroupName,
                                    item.QuantityPur,
                                    item.PrWeightString,
                                    item.PrCloseStatus,
                                    item.CreateBy,

                                    item.PoNumber,
                                    item.PoProject,
                                    item.PoDateString,
                                    item.DueDateString,
                                    item.PoQuantityPur,
                                    item.PoQuantityWeight,
                                    item.PoPurUom,
                                    item.PoBranch,
                                    item.PoWorkItemName,
                                    item.PoWorkGroupName,
                                    item.PoStatus,
                                    item.CloseStatus,

                                    item2.RcNumber,
                                    item2.RcStatus,
                                    item2.HeatNumber,
                                    item2.RcProject,
                                    item2.RcDateString,
                                    item2.RcQuantityPur,
                                    item2.RcQuantityWeight,
                                    item2.RcPurUom,
                                    item2.RcBranch,
                                    item2.RcWorkItemName,
                                    item2.RcWorkGroupName
                                );
                            }
                        }
                        else
                        {
                            table.Rows.Add(
                                   item.PrNumber,
                                   item.Project,
                                   item.PRDateString,
                                   item.RequestDateString,
                                   item.ItemCode,
                                   item.ItemName,
                                   item.PurUom,
                                   item.Branch,
                                   item.WorkItemName,
                                   item.WorkGroupName,
                                   item.QuantityPur,
                                   item.PrWeightString,
                                   item.PrCloseStatus,
                                   item.CreateBy,

                                   item.PoNumber,
                                   item.PoProject,
                                   item.PoDateString,
                                   item.DueDateString,
                                   item.PoQuantityPur,
                                   item.PoQuantityWeight,
                                   item.PoPurUom,
                                   item.PoBranch,
                                   item.PoWorkItemName,
                                   item.PoWorkGroupName,
                                   item.PoStatus,
                                   item.CloseStatus,

                                   "",
                                   "",
                                   "",
                                   "",
                                   "",
                                   "",
                                   "",
                                   "",
                                   "",
                                   "",
                                   ""
                               );
                        }
                      
                    }

                    return File(this.helperService.CreateExcelFilePivotTables(table,"PurchaseStatus","PurchaseDataPivot"),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Payment_Report.xlsx");
                }
            }
            catch (Exception ex)
            {
                Message = $"Has error{ex.ToString()}";
            }
            return BadRequest(new { Error = Message });
        }

        /*
        public async Task<IActionResult> GetReport2([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            var Message = "Data not been found.";
            try
            {
                // Set skip and take
                // Scroll.Skip = 0;
                // Scroll.Take = 999;

                var MapDatas = await this.GetData3(Scroll);

                if (MapDatas.Any())
                {
                    var table = new DataTable();
                    //Adding the Columns
                    table.Columns.AddRange(new DataColumn[]
                    {
                        new DataColumn("PrNo", typeof(string)),
                        new DataColumn("JobNo", typeof(string)),
                        new DataColumn("PrDate",typeof(string)),
                        new DataColumn("RequestDate",typeof(string)),
                        new DataColumn("Item-Code",typeof(string)),
                        new DataColumn("Item-Name",typeof(string)),
                        new DataColumn("Uom",typeof(string)),
                        new DataColumn("Branch",typeof(string)),
                        new DataColumn("BomLv",typeof(string)),
                        new DataColumn("WorkGroup",typeof(string)),
                        new DataColumn("Qty",typeof(int)),
                        new DataColumn("PrWeight",typeof(string)),
                        new DataColumn("PrClose",typeof(string)),
                        new DataColumn("Create",typeof(string)),

                        new DataColumn("PoNo",typeof(string)),
                        new DataColumn("JobNo-2",typeof(string)),
                        new DataColumn("PoDate",typeof(string)),
                        new DataColumn("DueDate",typeof(string)),
                        new DataColumn("QtyPo",typeof(int)),
                        new DataColumn("Weight",typeof(int)),
                        new DataColumn("Uom-2",typeof(string)),
                        new DataColumn("Branch-2",typeof(string)),
                        new DataColumn("BomLv-2",typeof(string)),
                        new DataColumn("WorkGroup-2",typeof(string)),
                        new DataColumn("TypePo",typeof(string)),
                        new DataColumn("PoStatus",typeof(string)),
                        new DataColumn("Receipt",typeof(string))
                    });

                    //Adding the Rows
                    foreach (var item in MapDatas)
                    {
                        item.ItemName = this.helperService.ConvertHtmlToText(item.ItemName);
                        item.ItemName = item.ItemName.Replace("\r\n", "");
                        item.ItemName = item.ItemName.Replace("\n", "");
                        var Receipt = "";
                        foreach (var item2 in item.PurchaseReceipts)
                        {
                            if (!string.IsNullOrEmpty(Receipt))
                                Receipt += "\r\n";

                            Receipt += item2.RcNumber + "     ";
                            Receipt += item2.RcStatus + "     ";
                            Receipt += item2.HeatNumber + "     ";
                            Receipt += item2.RcProject + "     ";
                            Receipt += item2.RcDateString + "     ";
                            Receipt += item2.RcQuantityPur + "     ";
                            Receipt += item2.RcQuantityWeight + "     ";
                            Receipt += item2.RcPurUom + "     ";
                            Receipt += item2.RcBranch + "     ";
                            Receipt += item2.RcWorkItemName + "     ";
                            Receipt += item2.RcWorkGroupName + "     ";
                        }

                        table.Rows.Add(
                            item.PrNumber,
                            item.Project,
                            item.PRDateString,
                            item.RequestDateString,
                            item.ItemCode,
                            item.ItemName,
                            item.PurUom,
                            item.Branch,
                            item.WorkItemName,
                            item.WorkGroupName,
                            item.QuantityPur,
                            item.PrWeightString,
                            item.PrCloseStatus,
                            item.CreateBy,

                            item.PoNumber,
                            item.PoProject,
                            item.PoDateString,
                            item.DueDateString,
                            item.PoQuantityPur,
                            item.PoQuantityWeight,
                            item.PoPurUom,
                            item.PoBranch,
                            item.PoWorkItemName,
                            item.PoWorkGroupName,
                            item.PoStatus,
                            item.CloseStatus,
                            Receipt
                        );
                    }

                    return File(this.helperService.CreateExcelFile(table, "PurchaseStatus"),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Payment_Report.xlsx");
                }
            }
            catch (Exception ex)
            {
                Message = $"Has error{ex.ToString()}";
            }
            return BadRequest(new { Error = Message });
        }
        */
    }
}