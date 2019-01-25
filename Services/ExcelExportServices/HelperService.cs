using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using VipcoSageX3.ViewModels;

namespace VipcoSageX3.Services.ExcelExportServices
{
    public class HelperService : IHelperService
    {
        private readonly HtmlDocumentService htmlDocument;
        private readonly ExcelWorkBookService excelService;

        public HelperService(HtmlDocumentService _htmlDocument, ExcelWorkBookService _excelService)
        {
            this.htmlDocument = _htmlDocument;
            this.excelService = _excelService;
        }

        public string ConvertHtmlToText(string HtmlCode)
        {
            var htmlBody = this.htmlDocument.Create(HtmlCode);
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

        public MemoryStream CreateExcelFile(DataTable table, string sheetName)
        {
            var memory = this.excelService.CreateMemory();

            using (var wb = this.excelService.Create())
            {
                var wsFreeze = wb.Worksheets.Add(table,sheetName);
                wsFreeze.Columns().AdjustToContents();
                wsFreeze.SheetView.FreezeRows(1);
                wb.SaveAs(memory);
            }
            memory.Position = 0;
            return memory;
        }

        public MemoryStream CreateExcelFileMuiltSheets(List<MuiltSheetViewModel> muiltSheets)
        {
            var memory = this.excelService.CreateMemory();

            using (var wb = this.excelService.Create())
            {
                foreach (var item in muiltSheets)
                {
                    var wsFreeze = wb.Worksheets.Add(item.Tables, item.SheetName);
                    wsFreeze.Columns().AdjustToContents();
                    wsFreeze.SheetView.FreezeRows(1);
                }

                wb.SaveAs(memory);
            }
            memory.Position = 0;
            return memory;
        }

        public MemoryStream CreateExcelFilePivotTables(DataTable table, string sheetName,string pivotName = "")
        {
            var memory = this.excelService.CreateMemory();

            using (var workbook = this.excelService.Create())
            {
                var wsSource = workbook.Worksheets.Add(sheetName);
                wsSource.Hide();
                var tableSource = wsSource.Cell(1, 1).InsertTable(table, true);
                // wsSource.SheetView.FreezeRows(1);
                // Set data to excel

                // Add a new sheet for our pivot table
                var ptSheet = workbook.Worksheets.Add(pivotName ?? (sheetName + "Pivot"));
                // Create the pivot table, using the data from the "PastrySalesData" table

                var pt = ptSheet.PivotTables.Add(pivotName ?? (sheetName + "Pivot"), ptSheet.Cell(1, 1), tableSource);
                // The rows in our pivot table will be the names of the pastries
                // pt.RowLabels.Add("Name");

                foreach (var col in table.Columns)
                {
                    pt.RowLabels.Add(col.ToString());
                }

                // The columns will be the months
                // pt.ColumnLabels.Add("Purchase Status");

                // The values in our table will come from the "NumberOfOrders" field
                // The default calculation setting is a total of each row/column
                // pt.Values.Add("NumberOfOrders");

                pt.SetLayout(ClosedXML.Excel.XLPivotLayout.Tabular);
                pt.Layout = ClosedXML.Excel.XLPivotLayout.Tabular;
                pt.EmptyCellReplacement = "-";
                pt.SetEmptyCellReplacement("-");
                pt.ClassicPivotTableLayout = true;
                pt.ShowGrandTotalsColumns = false;
                pt.ShowGrandTotalsRows = false;
                // Set worksheet columns wiltd
                ptSheet.Columns().AdjustToContents();
                ptSheet.SheetView.FreezeColumns(1);
                ptSheet.SheetView.FreezeRows(2);

                ptSheet.Range(ptSheet.RangeAddress.FirstAddress,ptSheet.RangeAddress.LastAddress)
                    .AddConditionalFormat()
                    .WhenEquals("\"(blank)\"")
                    .NumberFormat.SetFormat(";;;");

                workbook.SaveAs(memory);
            }
            memory.Position = 0;
            return memory;
        }
    }
}