﻿using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.Services.ExcelExportServices
{
    public class ExcelWorkBookService
    {
        public XLWorkbook Create()
        {
            return new XLWorkbook();
        }

        public MemoryStream CreateMemory()
        {
            return new MemoryStream();
        }
    }
}
