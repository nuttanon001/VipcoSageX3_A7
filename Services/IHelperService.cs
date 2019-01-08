using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VipcoSageX3.ViewModels;

namespace VipcoSageX3.Services
{
    public interface IHelperService
    {
        string ConvertHtmlToText(string HtmlCode);

        MemoryStream CreateExcelFile(DataTable table, string sheetName);

        MemoryStream CreateExcelFileMuiltSheets(List<MuiltSheetViewModel> muiltSheets);
    }
}
