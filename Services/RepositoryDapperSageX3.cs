using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using VipcoSageX3.ViewModels;

namespace VipcoSageX3.Services
{
    public class RepositoryDapperSageX3<Entity> : IRepositoryDapperSageX3<Entity> where Entity : class
    {
        private readonly IConfiguration config;

        public RepositoryDapperSageX3(IConfiguration config)
        {
            this.config = config;
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(this.config.GetConnectionString("SageX3Connection"));
            }
        }

        public async Task<List<PurchaseRequestAndOrderViewModel>> GetPurchaseRequestAndOrders(ScrollViewModel Scroll)
        {
            using (IDbConnection conn = Connection)
            {
                string sQuery = "";
                string sWhere = "";
                string sSort = "";

                #region Where

                if (!string.IsNullOrEmpty(Scroll.WhereBranch))
                    sWhere += (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"DIM.CCE_0 = {Scroll.WhereBranch}"; //QueryData.Where(x => x.dim.Cce0 == Scroll.WhereBranch);

                if (!string.IsNullOrEmpty(Scroll.WhereWorkGroup))
                    sWhere += (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"DIM.CCE_3 = {Scroll.WhereBranch}";// QueryData = QueryData.Where(x => x.dim.Cce3 == Scroll.WhereWorkGroup);

                if (!string.IsNullOrEmpty(Scroll.WhereWorkItem))
                    sWhere += (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"DIM.CCE_1 = {Scroll.WhereBranch}";//QueryData = QueryData.Where(x => x.dim.Cce1 == Scroll.WhereWorkItem);

                if (!string.IsNullOrEmpty(Scroll.WhereProject))
                    sWhere += (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"DIM.CCE_2 = {Scroll.WhereBranch}";//QueryData = QueryData.Where(x => x.dim.Cce2 == Scroll.WhereProject);

                if (Scroll.SDate.HasValue && Scroll.EDate.HasValue)
                {
                    sWhere += (string.IsNullOrEmpty(sWhere) ? "WHERE " : " AND ") + $"PRH.PRQDAT_0 >= {Scroll.SDate.Value.Date} AND PRH.PRQDAT_0 <= {Scroll.EDate.Value.Date}";
                    //QueryData = QueryData.Where(x =>
                    //    x.prh.Prqdat0.Date >= Scroll.SDate.Value.Date &&
                    //    x.prh.Prqdat0.Date <= Scroll.EDate.Value.Date);
                }

                #endregion Where

                #region Sort

                switch (Scroll.SortField)
                {
                    case "PrNumber":
                        if (Scroll.SortOrder == -1)
                            sSort = $"PRH.PSHNUM_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.prh.Pshnum0);
                        else
                            sSort = $"PRH.PSHNUM_0 ASC";//QueryData = QueryData.OrderBy(x => x.prh.Pshnum0);
                        break;

                    case "Project":
                        if (Scroll.SortOrder == -1)
                            sSort = $"DIM.CCE_2 DESC";//QueryData = QueryData.OrderByDescending(x => x.prh.Pjth0);
                        else
                            sSort = $"DIM.CCE_2 ASC";//QueryData = QueryData.OrderBy(x => x.prh.Pjth0);
                        break;

                    case "PRDateString":
                        if (Scroll.SortOrder == -1)
                            sSort = $"PRH.PRQDAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.prh.Prqdat0);
                        else
                            sSort = $"PRH.PRQDAT_0 ASC";//QueryData = QueryData.OrderBy(x => x.prh.Prqdat0);
                        break;

                    case "ItemName":
                        if (Scroll.SortOrder == -1)
                            sSort = $"PRD.ITMDES_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.prd.Itmdes0);
                        else
                            sSort = $"PRD.ITMDES_0 ASC";//QueryData = QueryData.OrderBy(x => x.prd.Itmdes0);
                        break;

                    case "Branch":
                        if (Scroll.SortOrder == -1)
                            sSort = $"DIM.CCE_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.dim.Cce0);
                        else
                            sSort = $"DIM.CCE_0 ASC";//QueryData = QueryData.OrderBy(x => x.dim.Cce0);
                        break;

                    case "WorkItemName":
                        if (Scroll.SortOrder == -1)
                            sSort = $"DIM.CCE_1 DESC";//QueryData = QueryData.OrderByDescending(x => x.dim.Cce1);
                        else
                            sSort = $"DIM.CCE_1 ASC";//QueryData = QueryData.OrderBy(x => x.dim.Cce1);
                        break;

                    case "WorkGroupName":
                        if (Scroll.SortOrder == -1)
                            sSort = $"DIM.CCE_3 DESC";//QueryData = QueryData.OrderByDescending(x => x.dim.Cce3);
                        else
                            sSort = $"DIM.CCE_3 ASC";//QueryData = QueryData.OrderBy(x => x.dim.Cce3);
                        break;

                    case "PoNumber":
                        if (Scroll.SortOrder == -1)
                            sSort = $"POD.POHNUM_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.pod.Pohnum0);
                        else
                            sSort = $"POD.POHNUM_0 ASC";//QueryData = QueryData.OrderBy(x => x.pod.Pohnum0);
                        break;

                    case "PoDateString":
                        if (Scroll.SortOrder == -1)
                            sSort = $"POD.ORDDAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.pod.Orddat0);
                        else
                            sSort = $"POD.ORDDAT_0 ASC";//QueryData = QueryData.OrderBy(x => x.pod.Orddat0);
                        break;

                    case "DueDateString":
                        if (Scroll.SortOrder == -1)
                            sSort = $"POD.EXTRCPDAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.pod.Extrcpdat0);
                        else
                            sSort = $"POD.EXTRCPDAT_0 ASC";//QueryData = QueryData.OrderBy(x => x.pod.Extrcpdat0);
                        break;

                    case "CreateBy":
                        if (Scroll.SortOrder == -1)
                            sSort = $"PRH.CREUSR_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.prh.Creusr0);
                        else
                            sSort = $"PRH.CREUSR_0 ASC";//QueryData = QueryData.OrderBy(x => x.prh.Creusr0);
                        break;

                    default:
                        sSort = $"PRH.PRQDAT_0 DESC";//QueryData = QueryData.OrderByDescending(x => x.prh.Prqdat0);
                        break;
                }

                #endregion Sort

                #region QueryString

                sQuery = $@"SELECT	--PRH
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

                            FROM	VIPCO.PREQUIS PRH
                                    INNER JOIN VIPCO.PREQUISD PRD ON PRH.PSHNUM_0 = PRD.PSHNUM_0
                                    LEFT OUTER JOIN VIPCO.PREQUISO PRO ON PRD.PSHNUM_0 = PRO.PSHNUM_0 AND PRD.PSDLIN_0 = PRO.PSDLIN_0
                                    LEFT OUTER JOIN VIPCO.PORDER POH ON PRO.POHNUM_0 = POH.POHNUM_0
                                    LEFT OUTER JOIN VIPCO.PORDERQ POD ON PRO.POHNUM_0 = POD.POHNUM_0 AND PRO.POPLIN_0 = POD.POPLIN_0
                                    LEFT OUTER JOIN VIPCO.CPTANALIN DIM ON PRD.PSHNUM_0 = DIM.VCRNUM_0 AND PRD.PSDLIN_0 = DIM.VCRLIN_0
                                    LEFT OUTER JOIN VIPCO.CPTANALIN DIMPO ON POD.POHNUM_0 = DIMPO.VCRNUM_0 AND POD.POPLIN_0 = DIMPO.VCRLIN_0
                                    LEFT OUTER JOIN VIPCO.ITMMASTER ITM ON PRD.ITMREF_0 = ITM.ITMREF_0
                                    LEFT OUTER JOIN VIPCO.TEXCLOB TXT ON TXT.CODE_0 = ITM.PURTEX_0
                            {sWhere}
                            ORDER BY {sSort}
                            OFFSET     @Skip ROWS       -- skip 10 rows
                            FETCH NEXT @Take ROWS ONLY; -- take 10 rows";

                #endregion QueryString

                conn.Open();
                var result = await conn.QueryAsync<PurchaseRequestAndOrderViewModel>(sQuery,
                     new { Skip = (Scroll.Skip ?? 0), Take = (Scroll.Take ?? 15) }, commandTimeout: 120);
                conn.Close();
                return result.ToList();
            }
        }

        public async Task<List<Entity>> GetListEntites<Parameter>(string SqlCommand, Parameter parameter, int timeout = 60)
        {
            using (IDbConnection conn = Connection)
            {
                conn.Open();
                var result = await conn.QueryAsync<Entity>(SqlCommand, parameter, commandTimeout: timeout);
                conn.Close();
                return result.ToList();
            }
        }

        public async Task<List<Entity>> GetListEntites(string SqlCommand, int timeout = 60)
        {
            using (IDbConnection conn = Connection)
            {
                conn.Open();
                var result = await conn.QueryAsync<Entity>(SqlCommand, commandTimeout: timeout);
                conn.Close();
                return result.ToList();
            }
        }

        public async Task<ReturnViewModel<Entity>> GetListEntitesAndTotalRow<Parameter>(string SqlCommand, Parameter parameter, int timeout = 60)
        {
            using (IDbConnection conn = Connection)
            {
                conn.Open();
                var result = await conn.QueryMultipleAsync(SqlCommand, parameter, commandTimeout: timeout);
                var dbData = new ReturnViewModel<Entity>()
                {
                    Entities = result.Read<Entity>().ToList(),
                    TotalRow = result.Read<int>().FirstOrDefault()
                };
                conn.Close();

                return dbData;
            }
        }
    }
}