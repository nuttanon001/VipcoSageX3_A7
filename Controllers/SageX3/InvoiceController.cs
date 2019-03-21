using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using VipcoSageX3.Services;
using VipcoSageX3.Services.ExcelExportServices;
using VipcoSageX3.ViewModels;

namespace VipcoSageX3.Controllers.SageX3
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly IRepositoryDapperSageX3<InvoiceSupBPViewModel> repositoryInvoice;
        private readonly IRepositoryDapperSageX3<Journal2ViewModel> repositoryJournal2;
        private readonly IRepositoryDapperSageX3<InvoiceOutStandingViewModel> repositoryInvOutStand;
        private readonly IHelperService helperService;

        public JsonSerializerSettings DefaultJsonSettings =>
          new JsonSerializerSettings()
          {
              Formatting = Formatting.Indented,
              PreserveReferencesHandling = PreserveReferencesHandling.Objects,
              ReferenceLoopHandling = ReferenceLoopHandling.Ignore
          };

        public InvoiceController(
            IRepositoryDapperSageX3<InvoiceSupBPViewModel> repoInvoice,
            IRepositoryDapperSageX3<Journal2ViewModel> repoJournal2,
            IRepositoryDapperSageX3<InvoiceOutStandingViewModel> repoInvOutStand,
            IHelperService helperService)
        {
            // Repository
            this.repositoryInvoice = repoInvoice;
            this.repositoryJournal2 = repoJournal2;
            this.repositoryInvOutStand = repoInvOutStand;
            // Helper
            this.helperService = helperService;
        }

        private async Task<List<InvoiceSupBPViewModel>> GetData(ScrollViewModel scroll, bool option = false)
        {
            if (scroll != null)
            {
                string sWhere = "WHERE PIN.PIVTYP_0 IN ('ADC','ADV','DOWN','PREG','DRM','CRM')";
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
                                                    $@"(LOWER(PIN.NUM_0) LIKE '%{keyword}%'
                                                        OR LOWER(PIN.PIVTYP_0) LIKE '%{keyword}%'
                                                        OR LOWER(PIN.FCY_0 ) LIKE '%{keyword}%'
                                                        OR LOWER(PIN.BPR_0 ) LIKE '%{keyword}%'
                                                        OR LOWER(PIN.ACCNUM_0 ) LIKE '%{keyword}%')";
                }

                if (!string.IsNullOrEmpty(scroll.WhereProject))
                    sWhere += (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"LOWER(BPSL.PJTLIN_0) LIKE '%{scroll.WhereProject.ToLower()}%'";

                if (!string.IsNullOrEmpty(scroll.WhereSupplier))
                    sWhere += (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"LOWER(PIN.BPR_0) = '{scroll.WhereSupplier.ToLower()}'";

                if (scroll.SDate.HasValue)
                    sWhere +=
                        (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"PIN.ACCDAT_0 >= '{scroll.SDate.Value.ToString("yyyy-MM-dd")}'";

                if (scroll.EDate.HasValue)
                    sWhere +=
                        (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"PIN.ACCDAT_0 <= '{scroll.EDate.Value.ToString("yyyy-MM-dd")}'";

                #endregion Where

                #region Sort

                switch (scroll.SortField)
                {
                    case "DocumentNo":
                        if (scroll.SortOrder == -1)
                            sSort = $"PIN.NUM_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PIN.Pshnum0);
                        else
                            sSort = $"PIN.NUM_0 ASC";//QueryData = QueryData.OrderBy(x => x.PIN.Pshnum0);
                        break;

                    case "AccountDateString":
                        if (scroll.SortOrder == -1)
                            sSort = $"PIN.ACCDAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PIN.Pjth0);
                        else
                            sSort = $"PIN.ACCDAT_0 ASC";//QueryData = QueryData.OrderBy(x => x.PIN.Pjth0);
                        break;

                    case "InvType":
                        if (scroll.SortOrder == -1)
                            sSort = $"PIN.PIVTYP_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PIN.Prqdat0);
                        else
                            sSort = $"PIN.PIVTYP_0 ASC";//QueryData = QueryData.OrderBy(x => x.PIN.Prqdat0);
                        break;

                    case "Site":
                        if (scroll.SortOrder == -1)
                            sSort = $"PIN.FCY_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.prd.Itmdes0);
                        else
                            sSort = $"PIN.FCY_0 ASC";//QueryData = QueryData.OrderBy(x => x.prd.Itmdes0);
                        break;

                    case "SupplierName":
                        if (scroll.SortOrder == -1)
                            sSort = $"PIN.BPR_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PIN.Cce0);
                        else
                            sSort = $"PIN.BPR_0 ASC";//QueryData = QueryData.OrderBy(x => x.PIN.Cce0);
                        break;

                    case "HeadAccountCode":
                        if (scroll.SortOrder == -1)
                            sSort = $"PIN.ACCNUM_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PIN.Cce1);
                        else
                            sSort = $"PIN.ACCNUM_0 ASC";//QueryData = QueryData.OrderBy(x => x.PIN.Cce1);
                        break;

                    case "LineAccountCode":
                        if (scroll.SortOrder == -1)
                            sSort = $"BPSL.ACC_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PIN.Cce1);
                        else
                            sSort = $"BPSL.ACC_0 ASC";//QueryData = QueryData.OrderBy(x => x.PIN.Cce1);
                        break;

                    case "AmountTaxString":
                        if (scroll.SortOrder == -1)
                            sSort = $"BPSL.AMTNOTLIN_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PIN.Cce1);
                        else
                            sSort = $"BPSL.AMTNOTLIN_0 ASC";//QueryData = QueryData.OrderBy(x => x.PIN.Cce1);
                        break;

                    case "Tax":
                        if (scroll.SortOrder == -1)
                            sSort = $"BPSL.VAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PIN.Cce1);
                        else
                            sSort = $"BPSL.VAT_0 ASC";//QueryData = QueryData.OrderBy(x => x.PIN.Cce1);
                        break;

                    case "Project1":
                        if (scroll.SortOrder == -1)
                            sSort = $"BPSL.PJTLIN_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PIN.Cce3);
                        else
                            sSort = $"BPSL.PJTLIN_0 ASC";//QueryData = QueryData.OrderBy(x => x.PIN.Cce3);
                        break;

                    default:
                        sSort = $"PIN.ACCDAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PIN.Prqdat0);
                        break;
                }

                #endregion Sort

                #region Query

                // Query mulitple command
                sQuery = $@"SELECT	PIN.NUM_0 AS [DocumentNo],
                                    PIN.ACCDAT_0 AS [AccountDate],
                                    PIN.PIVTYP_0 AS [InvType],
                                    PIN.FCY_0 AS [Site],
                                    PIN.BPR_0 AS [Supplier],
                                    (SELECT T.BPSNAM_0 FROM VIPCO.BPSUPPLIER T WHERE T.BPSNUM_0 = PIN.BPR_0) AS [SupplierName],
                                    PIN.ACCNUM_0 AS [HeadAccountCode],
                                    BPSL.ACC_0 AS [LineAccountCode],
                                    BPSL.AMTNOTLIN_0 AS [AmountTax],
                                    BPSL.VAT_0 AS [Tax],
                                    BPSL.AMTVAT_0 AS [TaxAmount],
                                    BPSL.DES_0 AS [Comment],
                                    BPSL.PJTLIN_0  AS [Project1],
                                    BPSA.CCE_2 AS [Project2],
                                    BPSA.CCE_0 AS [Branch],
                                    BPSA.CCE_1 AS [Bom],
                                    BPSA.CCE_3 AS [WorkGroup],
                                    '' AS [CostCenter],
                                    BPSL.ZISSUEDBY_0 AS [Issued],
                                    (SELECT T.BPSNAM_0 FROM VIPCO.BPSUPPLIER T WHERE T.BPSNUM_0 = BPSL.ZISSUEDBY_0) AS [Title],
                                    BPSL.ZTAXINVNO_0 AS [TaxinvNo]
                            FROM	VIPCO.PINVOICE AS PIN
                                    LEFT OUTER JOIN VIPCO.BPSINVLIG AS BPSL
                                        ON PIN.NUM_0 = BPSL.NUM_0
                                    LEFT OUTER JOIN VIPCO.BPSINVLIGA AS BPSA
                                        ON BPSL.NUM_0 = BPSA.NUM_0
                                        AND BPSL.LIG_0 = BPSA.LIG_0
                            {sWhere}
                            ORDER BY    {sSort}
                            OFFSET      @Skip ROWS       -- skip 10 rows
                            FETCH NEXT  @Take ROWS ONLY; -- take 10 rows;
                            SELECT	    COUNT(*)
                            FROM	    VIPCO.PINVOICE AS PIN
                                        LEFT OUTER JOIN VIPCO.BPSINVLIG AS BPSL
                                            ON PIN.NUM_0 = BPSL.NUM_0
                                        LEFT OUTER JOIN VIPCO.BPSINVLIGA AS BPSA
                                            ON BPSL.NUM_0 = BPSA.NUM_0
                                            AND BPSL.LIG_0 = BPSA.LIG_0
                            {sWhere};";

                #endregion Query

                var result = await this.repositoryInvoice.GetListEntitesAndTotalRow(sQuery, new { Skip = scroll.Skip ?? 0, Take = scroll.Take ?? 15 });
                var dbData = result.Entities;
                scroll.TotalRow = result.TotalRow;

                return dbData;
            }
            return null;
        }

        private async Task<List<Journal2ViewModel>> GetDataJournal2(ScrollViewModel scroll, bool option = false)
        {
            if (scroll != null)
            {
                string sWhere = "WHERE GAC.TYP_0 IN ('BFEE','GLA','GLG','RVAO','TRN')";
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
                                                    $@"(LOWER(GAC.NUM_0) LIKE '%{keyword}%'
                                                        OR LOWER(GAC.TYP_0) LIKE '%{keyword}%'
                                                        OR LOWER(GAC.JOU_0 ) LIKE '%{keyword}%')";
                }

                if (!string.IsNullOrEmpty(scroll.WhereProject))
                    sWhere += (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"LOWER(GAA.CCE_2) LIKE '%{scroll.WhereProject.ToLower()}%'";
                
                if (scroll.SDate.HasValue)
                    sWhere += (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"GAC.ACCDAT_0 >= '{scroll.SDate.Value.ToString("yyyy-MM-dd")}'";

                if (scroll.EDate.HasValue)
                    sWhere += (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"GAC.ACCDAT_0 <= '{scroll.EDate.Value.ToString("yyyy-MM-dd")}'";

                #endregion Where

                #region Sort

                switch (scroll.SortField)
                {
                    case "DocumentNo":
                        if (scroll.SortOrder == -1)
                            sSort = $"GAC.NUM_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PIN.Pshnum0);
                        else
                            sSort = $"GAC.NUM_0 ASC";//QueryData = QueryData.OrderBy(x => x.PIN.Pshnum0);
                        break;

                    case "DateString":
                        if (scroll.SortOrder == -1)
                            sSort = $"GAC.ACCDAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PIN.Pjth0);
                        else
                            sSort = $"GAC.ACCDAT_0 ASC";//QueryData = QueryData.OrderBy(x => x.PIN.Pjth0);
                        break;

                    case "EntryType":
                        if (scroll.SortOrder == -1)
                            sSort = $"GAC.TYP_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PIN.Prqdat0);
                        else
                            sSort = $"GAC.TYP_0 ASC";//QueryData = QueryData.OrderBy(x => x.PIN.Prqdat0);
                        break;

                    case "Journal":
                        if (scroll.SortOrder == -1)
                            sSort = $"GAC.JOU_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.prd.Itmdes0);
                        else
                            sSort = $"GAC.JOU_0 ASC";//QueryData = QueryData.OrderBy(x => x.prd.Itmdes0);
                        break;

                    case "Site":
                        if (scroll.SortOrder == -1)
                            sSort = $"GAD.FCYLIN_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PIN.Cce0);
                        else
                            sSort = $"GAD.FCYLIN_0 ASC";//QueryData = QueryData.OrderBy(x => x.PIN.Cce0);
                        break;

                    case "Project":
                        if (scroll.SortOrder == -1)
                            sSort = $"GAA.CCE_2 DESC";//QueryData = QueryData.OrderByDescending(x => x.PIN.Cce1);
                        else
                            sSort = $"GAA.CCE_2 ASC";//QueryData = QueryData.OrderBy(x => x.PIN.Cce1);
                        break;

                    case "Branch":
                        if (scroll.SortOrder == -1)
                            sSort = $"GAA.CCE_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PIN.Cce1);
                        else
                            sSort = $"GAA.CCE_0 ASC";//QueryData = QueryData.OrderBy(x => x.PIN.Cce1);
                        break;

                    case "Bom":
                        if (scroll.SortOrder == -1)
                            sSort = $"GAA.CCE_1 DESC";//QueryData = QueryData.OrderByDescending(x => x.PIN.Cce1);
                        else
                            sSort = $"GAA.CCE_1 ASC";//QueryData = QueryData.OrderBy(x => x.PIN.Cce1);
                        break;

                    case "WorkGroup":
                        if (scroll.SortOrder == -1)
                            sSort = $"GAA.CCE_3 DESC";//QueryData = QueryData.OrderByDescending(x => x.PIN.Cce3);
                        else
                            sSort = $"GAA.CCE_3 ASC";//QueryData = QueryData.OrderBy(x => x.PIN.Cce3);
                        break;

                    default:
                        sSort = $"GAC.ACCDAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PIN.Prqdat0);
                        break;
                }

                #endregion Sort

                #region Query

                // Query mulitple command
                sQuery = $@"SELECT	GAC.NUM_0 AS [DocumentNo],
                                    GAC.ACCDAT_0 AS [Date],
                                    GAC.TYP_0 AS [EntryType],
                                    GAC.JOU_0 AS [Journal],
                                    GAD.FCYLIN_0 AS [Site],
                                    GAD.ACC_0 AS [Account],
                                    (CASE
                                        WHEN GAD.SNS_0 > 0
                                            THEN GAD.AMTCUR_0
                                    END) AS [Debit],
                                    (CASE
                                        WHEN GAD.SNS_0 < 0
                                            THEN GAD.AMTCUR_0
                                    END) AS [Credit],
                                    GAD.DES_0 AS [Description],
                                    GAA.CCE_2 AS [Project],
                                    GAA.CCE_0 AS [Branch],
                                    GAA.CCE_1 AS [Bom],
                                    GAA.CCE_3 AS [WorkGroup],
                                    '' AS [CostCenter],
                                    GAD.FREREF_0 AS [FreeReference],
                                    GAD.TAX_0 AS [Tax]
                            FROM	VIPCO.GACCENTRY AS GAC
                                    LEFT OUTER JOIN VIPCO.GACCENTRYD AS GAD
                                        ON GAC.NUM_0 = GAD.NUM_0
                                    LEFT OUTER JOIN VIPCO.GACCENTRYA AS GAA
                                        ON GAC.NUM_0 = GAA.NUM_0
                                        AND GAD.TYP_0 = GAA.TYP_0
                                        AND GAD.LIN_0 = GAA.LIN_0
                                        AND GAD.LEDTYP_0 = GAA.LEDTYP_0
                            {sWhere}
                            ORDER BY    {sSort}
                            OFFSET      @Skip ROWS       -- skip 10 rows
                            FETCH NEXT  @Take ROWS ONLY; -- take 10 rows;
                            SELECT	    COUNT(*)
                            FROM	    VIPCO.GACCENTRY AS GAC
                                        LEFT OUTER JOIN VIPCO.GACCENTRYD AS GAD
                                            ON GAC.NUM_0 = GAD.NUM_0
                                        LEFT OUTER JOIN VIPCO.GACCENTRYA AS GAA
                                            ON GAC.NUM_0 = GAA.NUM_0
                                            AND GAD.TYP_0 = GAA.TYP_0
                                            AND GAD.LIN_0 = GAA.LIN_0
                                            AND GAD.LEDTYP_0 = GAA.LEDTYP_0
                            {sWhere};";

                #endregion Query

                var result = await this.repositoryJournal2.GetListEntitesAndTotalRow(sQuery, new { Skip = scroll.Skip ?? 0, Take = scroll.Take ?? 15 });
                var dbData = result.Entities;
                scroll.TotalRow = result.TotalRow;

                return dbData;
            }
            return null;
        }

        private async Task<List<InvoiceOutStandingViewModel>> GetDataInvoiceOutStanding(ScrollViewModel Scroll, bool option = false)
        {
            if (Scroll != null)
            {
                // ACC_0 ลูกหนี้ในประเทศ 113101 และ ลูกหนี้ต่างประเทศ 113201
                string sWhere = "GAC.FLGCLE_0 = 1 AND GAD.ACC_0 IN ('113101','113201')";
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
                                                    $@"(LOWER(GAD.NUM_0) LIKE '%{keyword}%'
                                                        OR LOWER(BPC.BPCNAM_0) LIKE '%{keyword}%')";
                }

                // Where Customer
                if (Scroll.WhereBanks.Any())
                {
                    var list = new List<string>();

                    foreach (var item in Scroll.WhereBanks)
                        list.Add($"'{item}'");

                    var customers = string.Join(',', list);
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"BPC.BPCNUM_0 IN ({customers})";
                }

                // Where Project
                if (!string.IsNullOrEmpty(Scroll.WhereProject))
                {
                    sWhere += (string.IsNullOrEmpty(sWhere) ? " " : " AND ") + $"SINV.CCE_2 = '{Scroll.WhereProject}'";
                }

                #endregion

                #region Sort

                switch (Scroll.SortField)
                {
                    case "InvoiceNo":
                        if (Scroll.SortOrder == -1)
                            sSort = $"GAD.NUM_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pshnum0);
                        else
                            sSort = $"GAD.NUM_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pshnum0);
                        break;

                    case "CustomerNo":
                        if (Scroll.SortOrder == -1)
                            sSort = $"BPC.BPCNUM_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Pjth0);
                        else
                            sSort = $"BPC.BPCNUM_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Pjth0);
                        break;

                    case "CustomerName":
                        if (Scroll.SortOrder == -1)
                            sSort = $"BPC.BPCNAM_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"BPC.BPCNAM_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "Project":
                        if (Scroll.SortOrder == -1)
                            sSort = $"SINV.CCE_2 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"SINV.CCE_2 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "DocDateString":
                        if (Scroll.SortOrder == -1)
                            sSort = $"GAC.DUDDAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"GAC.DUDDAT_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    case "DueDateString":
                        if (Scroll.SortOrder == -1)
                            sSort = $"GAH.BPRDATVCR_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        else
                            sSort = $"GAH.BPRDATVCR_0 ASC";//QueryData = QueryData.OrderBy(x => x.PAYM.Prqdat0);
                        break;

                    default:
                        sSort = $"GAD.NUM_0 ASC";//QueryData = QueryData.OrderByDescending(x => x.PAYM.Prqdat0);
                        break;
                }

                #endregion

                var sqlCommnad = new SqlCommandViewModel()
                {
                    SelectCommand = $@"	GAD.NUM_0 AS InvoiceNo,
                                        SINV.AMTATI_0 AS InvPriceInTax,
                                        SINV.AMTNOT_0 AS InvPriceExTax,
                                        GAH.CUR_0 AS Currency,
                                        BPC.BPCNUM_0 AS CustomerNo,
                                        BPC.ZCOMPNAME_0 AS CustomerName,
                                        SINV.CCE_2 AS Project,
                                        GAC.FLGCLE_0 AS StatusClose,
                                        (CASE 
                                            WHEN GAH.CUR_0 = 'THB' 
                                                    THEN SINV.AMTATI_0--SIND.NETPRIATI_0 * SIND.QTY_0
                                            END) AS [THB_TAX],
                                            (CASE 
                                            WHEN GAH.CUR_0 = 'USD' 
                                                    THEN SINV.AMTATI_0--SIND.NETPRIATI_0 * SIND.QTY_0
                                            END) AS [USD_TAX],
                                            (CASE 
                                            WHEN GAH.CUR_0 = 'EUR' OR GAH.CUR_0 = 'GBP' 
                                                    THEN SINV.AMTATI_0--SIND.NETPRIATI_0 * SIND.QTY_0
                                            END) AS [EUR_TAX],

                                            (CASE 
                                            WHEN GAH.CUR_0 = 'THB' 
                                                    THEN SINV.AMTNOT_0--SIND.NETPRINOT_0 * SIND.QTY_0
                                            END) AS [THB],
                                            (CASE 
                                            WHEN GAH.CUR_0 = 'USD' 
                                                    THEN SINV.AMTNOT_0--SIND.NETPRINOT_0 * SIND.QTY_0
                                            END) AS [USD],
                                            (CASE 
                                            WHEN GAH.CUR_0 = 'EUR' OR GAH.CUR_0 = 'GBP' 
                                                    THEN SINV.AMTNOT_0--SIND.NETPRINOT_0 * SIND.QTY_0
                                            END) AS [EUR],
                                        GAC.DUDDAT_0 AS DueDate,
                                        GAH.BPRDATVCR_0 AS DocDate,
                                        SYSDATETIME() AS NowDate,
                                        DATEDIFF(DAY,SYSDATETIME(),GAC.DUDDAT_0 ) AS DIFF",
                    FromCommand = $@" VIPCO.GACCENTRYD GAD
                                    LEFT OUTER JOIN VIPCO.GACCENTRY GAH
                                            ON GAD.NUM_0 = GAH.NUM_0
                                            AND GAD.TYP_0 = GAH.TYP_0
                                    LEFT OUTER JOIN VIPCO.GACCDUDATE GAC
                                            ON GAD.ACCNUM_0 = GAC.ACCNUM_0
                                            AND GAD.TYP_0 = GAC.TYP_0 
                                            AND GAD.LIN_0 = GAC.LIG_0
                                    LEFT OUTER JOIN VIPCO.BPCUSTOMER BPC
                                            ON GAD.BPR_0 = BPC.BPCNUM_0
                                    LEFT OUTER JOIN VIPCO.SINVOICE SINV
                                            ON  GAC.NUM_0 = SINV.NUM_0",
                    WhereCommand = sWhere,
                    OrderCommand = sSort
                };

                var result = await this.repositoryInvOutStand.GetEntitiesAndTotal(sqlCommnad, new { Skip = Scroll.Skip ?? 0, Take = Scroll.Take ?? 50 });
                var dbData = result.Entities;
                Scroll.TotalRow = result.TotalRow;

                return dbData;
            }
            return null;
        }
        // POST: api/Invoice/GetScroll
        [HttpPost("GetScroll")]
        public async Task<IActionResult> GetScroll([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            var Message = "";
            try
            {
                var MapDatas = await this.GetData(Scroll);
                return new JsonResult(new ScrollDataViewModel<InvoiceSupBPViewModel>(Scroll, MapDatas), this.DefaultJsonSettings);
            }
            catch (Exception ex)
            {
                Message = $"{ex.ToString()}";
            }
            return BadRequest();
        }

        // POST: api/Invoice/GetReport
        [HttpPost("GetReport")]
        public async Task<IActionResult> GetReport([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            var Message = "Data not been found.";
            try
            {
                var MapDatas = await this.GetData(Scroll);

                if (MapDatas.Any())
                {
                    var table = new DataTable();
                    //Adding the Columns
                    foreach (var field in MapDatas[0].GetType().GetProperties()) // Loop through fields
                    {
                        string name = field.Name; // Get string name
                        var value = field.GetValue(MapDatas[0], null);

                        if (value is DateTime || value is double || value is int)
                            continue;

                        table.Columns.Add(field.Name.Replace("String", ""), typeof(string));
                    }

                    //Adding the Rows
                    // Table1
                    foreach (var item in MapDatas)
                    {
                        table.Rows.Add(
                            item.DocumentNo,
                            item.AccountDateString,
                            item.InvType,
                            item.Site,
                            item.Supplier,
                            item.SupplierName,
                            item.LineAccountCode,
                            item.AmountTaxString,
                            item.Tax,
                            item.TaxAmountString,
                            item.Comment,
                            item.Project1,
                            item.Project2,
                            item.Branch,
                            item.Bom,
                            item.WorkGroup,
                            item.CostCenter,
                            item.Issued,
                            item.Title,
                            item.TaxinvNo);
                    }

                    var file = this.helperService.CreateExcelFile(table, "SupplierBPInvoice");
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Journal.xlsx");
                }
            }
            catch (Exception ex)
            {
                Message = $"Has error{ex.ToString()}";
            }
            return BadRequest(new { Error = Message });
        }

        // POST: api/Invoice/GetScroll
        [HttpPost("JournalGetScroll")]
        public async Task<IActionResult> JournalGetScroll([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            var Message = "";
            try
            {
                var MapDatas = await this.GetDataJournal2(Scroll);
                return new JsonResult(new ScrollDataViewModel<Journal2ViewModel>(Scroll, MapDatas), this.DefaultJsonSettings);
            }
            catch (Exception ex)
            {
                Message = $"{ex.ToString()}";
            }
            return BadRequest( new { Message });
        }

        // POST: api/Invoice/GetReport
        [HttpPost("JournalGetReport")]
        public async Task<IActionResult> JournalGetReport([FromBody] ScrollViewModel Scroll)
        {
            if (Scroll == null)
                return BadRequest();

            var Message = "Data not been found.";
            try
            {
                var MapDatas = await this.GetDataJournal2(Scroll);

                if (MapDatas.Any())
                {
                    var table = new DataTable();
                    //Adding the Columns
                    foreach (var field in MapDatas[0].GetType().GetProperties()) // Loop through fields
                    {
                        string name = field.Name; // Get string name
                        var value = field.GetValue(MapDatas[0], null);

                        if (value is DateTime || value is double || value is int || name == "Credit" || name == "Debit")
                            continue;

                        table.Columns.Add(field.Name.Replace("String", ""), typeof(string));
                    }

                    //Adding the Rows
                    // Table1
                    foreach (var item in MapDatas)
                    {
                        table.Rows.Add(
                            item.DocumentNo,
                            item.DateString,
                            item.EntryType,
                            item.Journal,
                            item.Site,
                            item.Account,
                            item.DebitString,
                            item.CreditString,
                            item.Description,
                            item.Project,
                            item.Branch,
                            item.Bom,
                            item.WorkGroup,
                            item.CostCenter,
                            item.FreeReference,
                            item.Tax);
                    }

                    var file = this.helperService.CreateExcelFile(table, "SupplierBPInvoice");
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Journal.xlsx");
                }
            }
            catch (Exception ex)
            {
                Message = $"Has error{ex.ToString()}";
            }
            return BadRequest(new { Error = Message });
        }

        [HttpPost("InvoiceOutStandingGetScroll")]
        public async Task<IActionResult> InvoiceOutStandingGetScroll([FromBody] ScrollViewModel Scroll)
        {
            var message = "Data not been found.";
            try
            {
                var MapDatas = await this.GetDataInvoiceOutStanding(Scroll);

                foreach (var item in MapDatas)
                    item.InvoiceStatus = item.DIFF > 0 ? InvoiceStatus.OutStanding : InvoiceStatus.OverDue;

                return new JsonResult(new ScrollDataViewModel<InvoiceOutStandingViewModel>(Scroll, MapDatas), this.DefaultJsonSettings);
            }
            catch(Exception ex)
            {
                message = $"Has error {ex.ToString()}";
            }
            return BadRequest(new { message });
        }

        // POST: api/Invoice/GetReport
        [HttpPost("InvoiceOutStandingGetReport")]
        public async Task<IActionResult> InvoiceOutStandingGetReport([FromBody] ScrollViewModel Scroll)
        {
            var Message = "Data not been found.";
            try
            {
                if (Scroll != null)
                {
                    var MapDatas = await this.GetDataInvoiceOutStanding(Scroll);

                    foreach (var item in MapDatas)
                        item.InvoiceStatus = item.DIFF > 0 ? InvoiceStatus.OutStanding : InvoiceStatus.OverDue;

                    if (MapDatas.Any())
                    {
                        var table = new DataTable();
                        //Adding the Columns
                        foreach (var field in MapDatas[0].GetType().GetProperties()) // Loop through fields
                        {
                            string name = field.Name; // Get string name
                            var value = field.GetValue(MapDatas[0], null);

                            if (value is DateTime || value is double || 
                                value is int || value is InvoiceStatus || 
                                name == "InvoiceStatus")
                                continue;

                            if (name == "InvoiceStatusString")
                            {
                                var Col = table.Columns.Add(field.Name.Replace("String", ""), typeof(string));
                                Col.SetOrdinal(0);
                            }
                            else
                            {
                                var newName = field.Name.Replace("String", "");
                                var columns = table.Columns;
                                if (!columns.Contains(newName))
                                    table.Columns.Add(newName, typeof(string));
                            }
                        }

                        //Adding the Rows
                        // Table1
                        foreach (var item in MapDatas)
                        {
                            table.Rows.Add(
                                item.InvoiceStatusString,
                                item.InvoiceNo,
                                item.InvPriceInTaxString,
                                item.InvPriceExTaxString,
                                item.Currency,
                                item.CustomerNo,
                                item.CustomerName,
                                item.Project,
                                item.THB_TAXString,
                                item.USD_TAXString,
                                item.EUR_TAXString,
                                item.THBString,
                                item.USDString,
                                item.EURString,
                                item.DueDateString,
                                item.DocDateString,
                                item.NowDateString,
                                item.DIFFString);
                        }

                        var file = this.helperService.CreateExcelFilePivotTables(table, "InvoiceOutStandingOverDue", "InvoiceOutStandingOverDuePivot",false);
                        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OutStandingOverDue.xlsx");
                    }

                }
                   
            }
            catch (Exception ex)
            {
                Message = $"Has error{ex.ToString()}";
            }
            return BadRequest(new { Error = Message });
        }
    }
}