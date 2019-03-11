using System;
using System.Linq;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using VipcoSageX3.ViewModels;
using VipcoSageX3.Models.SageX3Extends;
using VipcoSageX3.Services.EmailServices;
using System.Data;
using RtfPipe;
using VipcoSageX3.Services.ExcelExportServices;
using System.Net.Mail;

namespace VipcoSageX3.Services.TaskServices
{
    public class TaskService: IHostedService
    {
        private Timer _timer;
        private readonly ILogger _logger;
        public TaskService(ILogger<TaskService> logger, IServiceProvider services) {
            _logger = logger;
            Services = services;
        }
        public IServiceProvider Services { get; }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            var dateNow = DateTime.Now;
            var sendTime = new List<int>() { 7, 12, 18 };
            if (sendTime.Contains(dateNow.Hour)){
                try
                {
                    using (var scope = this.Services.CreateScope())
                    {
                        #region Get Data
                        var repo3 = scope.ServiceProvider.GetRequiredService<IRepositorySageX3Extend<ReceiptExtend>>();
                        var repo2 = scope.ServiceProvider.GetRequiredService<IRepositorySageX3Extend<TaskStatusMaster>>();
                        var workGroups = (await repo2.GetToListAsync(x => new { x.WorkGroupCode, x.WorkGroupName }, x => x.TaskStatusDetails.Any())).ToList();

                        var lastGet = await repo3.GetFirstOrDefaultAsync(x => x, null, x => x.OrderByDescending(z => z.EndRange));

                        var newLast = new ReceiptExtend()
                        {
                            CreateDate = dateNow,
                            Creator = "System",
                            GetDate = dateNow,
                            GetTime = dateNow.ToString("HH:mm"),
                            StartRange = lastGet?.EndRange ?? 0,
                            EndRange = 0
                        };

                        if (lastGet != null && workGroups.Any())
                        {
                            var help = scope.ServiceProvider.GetRequiredService<IHelperService>();
                            var repo = scope.ServiceProvider.GetRequiredService<IRepositorySageX3Extend<TaskStatusDetail>>();
                            var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                            #region Create DataTable

                            var table = new DataTable();
                            //Adding the Columns
                            table.Columns.AddRange(new DataColumn[]
                            {
                                    new DataColumn("PrNo", typeof(string)),
                                    new DataColumn("JobNo", typeof(string)),
                                    new DataColumn("Item-Name",typeof(string)),
                                    new DataColumn("Uom",typeof(string)),
                                    new DataColumn("BomLv",typeof(string)),
                                    new DataColumn("WorkGroup",typeof(string)),
                                    new DataColumn("Qty",typeof(int)),

                                    new DataColumn("PoNo",typeof(string)),
                                    new DataColumn("QtyPo",typeof(int)),
                                    new DataColumn("TypePo",typeof(string)),

                                    new DataColumn("QtyRc",typeof(int)),
                            });
                            #endregion

                            foreach (var workgroup in workGroups)
                            {
                                #region GetData
                                var sqlCommand = new SqlCommandViewModel()
                                {
                                    SelectCommand = $@" --RECIPT
                                            PRD.ROWID AS [RowId],
                                            PRD.POHNUM_0 AS [PoNumber],
                                            PRD.POPLIN_0 AS [PoLine],
                                            PRD.ITMREF_0 AS [ItemCode],
                                            PRD.QTYPUU_0 AS [RCQuantityPur],
                                            DIM.CCE_0 AS [Branch],
                                            (SELECT CAC.DES_0 FROM VIPCO.CACCE CAC WHERE CAC.CCE_0 = DIM.CCE_0) AS [BranchName],
                                            DIM.CCE_1 AS [WorkItem],
                                            (SELECT CAC.DES_0 FROM VIPCO.CACCE CAC WHERE CAC.CCE_0 = DIM.CCE_1) AS [WorkItemName],
                                            DIM.CCE_2 AS [Project],
                                            (SELECT CAC.DES_0 FROM VIPCO.CACCE CAC WHERE CAC.CCE_0 = DIM.CCE_2) AS [ProjectName],
                                            DIM.CCE_3 AS [WorkGroup],
                                            (SELECT CAC.DES_0 FROM VIPCO.CACCE CAC WHERE CAC.CCE_0 = DIM.CCE_3) AS [WorkGroupName],
                                            --PURCHASE ORDER
                                            POH.ZPO21_0 AS [PoStatusInt],
                                            POD.QTYPUU_0 AS [PoQuantityPur],
                                            --PURCHASE REQUEST
                                            PRO.PSHNUM_0 AS [PrNumber],
                                            PRO.PSDLIN_0 AS [PrLine],
                                            PRO.QTYPUU_0 AS [QuantityPur],
                                            PRO.PUU_0 AS [PurUom],
                                            --ITEMMASTER
                                            TXT.TEXTE_0 AS [ItemName]",
                                    FromCommand = $@" [VIPCO].[PRECEIPTD] PRD
                                            LEFT OUTER JOIN VIPCO.CPTANALIN DIM
                                                ON DIM.ABRFIC_0 = 'PTD'
                                                AND DIM.VCRTYP_0 = 0
                                                AND DIM.VCRSEQ_0 = 0
                                                AND	DIM.CPLCLE_0 = ''
                                                AND	DIM.ANALIG_0 = 1
                                                AND PRD.PTHNUM_0 = DIM.VCRNUM_0
                                                AND PRD.PTDLIN_0 = DIM.VCRLIN_0
                                            LEFT OUTER JOIN VIPCO.PORDER POH
                                                ON PRD.POHNUM_0 = POH.POHNUM_0
                                            LEFT OUTER JOIN VIPCO.PORDERQ POD
                                                ON PRD.POHNUM_0 = POD.POHNUM_0
                                                AND PRD.POPLIN_0 = POD.POPLIN_0 
                                            LEFT OUTER JOIN VIPCO.PREQUISO PRO
                                                ON PRO.POHNUM_0 = PRD.POHNUM_0
                                                AND PRO.POPLIN_0 = PRD.POPLIN_0
                                            LEFT OUTER JOIN VIPCO.ITMMASTER ITM
                                                ON PRD.ITMREF_0 = ITM.ITMREF_0
                                            LEFT OUTER JOIN VIPCO.TEXCLOB TXT
                                                ON TXT.CODE_0 = ITM.PURTEX_0",
                                    WhereCommand = $@" PRD.ROWID > {lastGet.EndRange}
                                                   AND DIM.CCE_3 = '{workgroup.WorkGroupCode.Trim()}'",
                                    OrderCommand = $@" PRD.POHNUM_0"

                                };

                                var dapper = scope.ServiceProvider.GetRequiredService<IRepositoryDapperSageX3<ScheduleReceiptViewModel>>();
                                var hasData = await dapper.GetEntities(sqlCommand);
                                var allJob = hasData.Select(x => x.Project).OrderBy(x => x).Distinct().ToList();

                                #endregion

                                #region Create Message

                                var message = $@"<body style='margin:0; padding:0;background-color:#EFF9FB;'>
                                    <table align='center' border='0' cellpadding='0' cellspacing='0' width='650' style='border-collapse: collapse;'>
                                        <tr>
                                            <td align='center' bgcolor='#70bbd9' style='padding: 40px 0 30px 0;font-family: Avenir, sans-serif; font-size: 18px;'>
                                                <h1 style='color:whitesmoke;'>&nbsp;<strong>Vipco SageX3 extend system</strong></h1>
                                                <hr style='width:80%;'/>
                                                <h4 style='font-family: Avenir, sans-serif; font-size: 15px;'> &nbsp;&nbsp;ระบบแจ้งเตือนการลงรับ วัตถุดิบ,วัสดุ หรือ เครื่องมือ จากระบบ SageX3</h4>
                                                <!--<img src='images/h1.gif' alt='Creating Email Magic' width='300' height='230' style='display: block;' />-->
                                            </td>
                                        </tr>
        
                                        <tr>
                                            <td bgcolor='#ffffff' style='padding: 30px 20px 30px 20px;'>
                                                <table border='0' cellpadding='0' cellspacing='0' width='100%'>
                                                    <tr>
                                                        <td style='padding: 5px 0 5px 0;font-family: Avenir, sans-serif; font-size: 18px;'>
                                                            <b>เรียน</b><i> ผู้เกี่ยวข้องทุกท่าน</i>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style='padding: 20px 0 5px 0;font-family: Avenir, sans-serif; font-size: 16px;'>
                                                            &nbsp;&nbsp;&nbsp;&nbsp;เนื่องด้วยระบบ Vipco SageX3 extend system ได้ตรวจพบการรับเข้า วัตถุดิบ,วัสดุ หรือ เครื่องมือ ของกลุ่มงานที่ท่านมีความเกี่ยวข้อง ผ่านทางระบบ SageX3 
                                                            <br/><br/>&nbsp;&nbsp;&nbsp;&nbsp;ระบบจึงทำการแจ้งเตือนผู้เกี่ยวข้องทุกๆท่าน ให้เข้าตรวจสอบข้อมูลดังกล่าว โดยพร้อมทั้งนี้ระบบได้ทำการแนบ ข้อมูลคำขอสั่งซื้อและ ข้อมูลสั่งซื้อมาพร้อมกับอีเมล์ฉบับนี้
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style='padding: 5px 0 30px 0;font-family: Avenir, sans-serif; font-size: 16px;'>
                                                            &nbsp;&nbsp;&nbsp;&nbsp;เลขที่งานที่ตรวจพบดังนี้ : {string.Join(",", allJob)}
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td bgcolor='#ee4c50' style='padding: 15px 15px 15px 15px;'>
                                                <table border='0' cellpadding='0' cellspacing='0' width='100%'>
                                                    <tr>
                                                        <td width='80%'  style='color:whitesmoke;padding: 5px 0px 5px 0px;font-family: Avenir, sans-serif; font-size: 12px;'>
                                                            &nbsp;This mail auto generated by VIPCO SageX3 extend system.<br/> &nbsp;Do not reply this email. 
                                                        </td>
                                                        <td style='font-family: Avenir, sans-serif; font-size: 12px;'>
                                                             <a href='http://192.168.2.31/extends-sagex3/purchase-request' >more information.</a>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </body>";

                                #endregion

                                #region Excel

                                if (hasData.Any())
                                {
                                    #region Create Table
                                    // Update row index
                                    var last = hasData.Max(x => x.RowId);
                                    newLast.EndRange = last > newLast.EndRange ? last : newLast.EndRange;

                                    table.Rows.Clear();
                                    //Adding the Rows
                                    foreach (var item in hasData)
                                    {
                                        if (item == null)
                                            continue;

                                        if (!string.IsNullOrEmpty(item.ItemName))
                                        {
                                            if (item.ItemName.StartsWith("{\\rtf1"))
                                                item.ItemName = Rtf.ToHtml(item.ItemName);

                                            item.ItemName = help.ConvertHtmlToText(item.ItemName);
                                            item.ItemName = item.ItemName.Replace("\r\n", "");
                                            item.ItemName = item.ItemName.Replace("\n", "");
                                        }
                                        
                                        //var Receipt = "";

                                        table.Rows.Add(
                                                item.PrNumber,
                                                item.Project,
                                                item.ItemName,
                                                item.PurUom,
                                                item.WorkItemName,
                                                item.WorkGroupName,
                                                item.QuantityPur,
                                                item.PoNumber,
                                                item.PoQuantityPur,
                                                item.PoStatus,
                                                item.RCQuantityPur);
                                    }

                                    var excel = help.CreateExcelFilePivotTables(table, "PurchaseStatus", "PurchaseStatusPivot");
                                    #endregion

                                    #region GetMail Address

                                    var MailTos = await repo.GetToListAsync(x => x.Email, x => x.TaskStatusMaster.WorkGroupCode == workgroup.WorkGroupCode);

                                    #endregion

                                    #region Send Mail

                                    await scopedProcessingService.SendMail(new ViewModels.EmailViewModel()
                                    {
                                        MailFrom = MailTos.FirstOrDefault(),
                                        MailTos = MailTos.ToList(),//new List<string>() { "to.nuttanon@vipco-thai.com" },
                                        Message = message,
                                        NameFrom = "No-Reply",
                                        Subject = $"{workgroup.WorkGroupName} Notification mail from Vipco SageX3 extend system.",
                                        HasAttach = true,
                                        Attachment = new Attachment(excel, "Export.xlsx")
                                    });

                                    #endregion

                                    excel.Dispose();
                                }

                                #endregion
                            }

                            await repo3.AddAsync(newLast);
                        }
                        #endregion
                    }
                }
                catch(Exception ex)
                {
                    using (var scope = this.Services.CreateScope())
                    {
                        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                        await scopedProcessingService.SendMail(new ViewModels.EmailViewModel()
                        {
                            MailFrom = "to.nuttanon@vipco-thai.com",
                            MailTos = new List<string>() { "to.nuttanon@vipco-thai.com" },
                            Message = $"Has error {ex.ToString()}",
                            NameFrom = "Notification",
                            Subject = $"Notification error.{dateNow}",
                        });
                    }
                }
            }

            _logger.LogInformation($"Timed Background Service is working. at {dateNow}.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // throw new System.NotImplementedException();
            _logger.LogInformation($"Timed Background Service is stopping. at {DateTime.Now}.");
            _timer?.Change(Timeout.Infinite, 0);
            // Dispose
            this.Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

    }
}
