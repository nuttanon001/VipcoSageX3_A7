using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VipcoSageX3.ViewModels;

namespace VipcoSageX3.Services.ExcelExportServices
{
    public interface IHelperService
    {
        string ConvertHtmlToText(string HtmlCode);
        TEntity AddHourMethod<TEntity>(TEntity entity);
        MemoryStream CreateExcelFile(DataTable table, string sheetName);

        MemoryStream CreateExcelFileMuiltSheets(List<MuiltSheetViewModel> muiltSheets);

        MemoryStream CreateExcelFilePivotTables(DataTable table, string sheetName, string pivotName = "", bool hideData = true);
    }
}
