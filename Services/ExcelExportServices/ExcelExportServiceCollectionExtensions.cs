using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.Services.ExcelExportServices
{
    public static class ExcelExportServiceCollectionExtensions
    {
        public static IServiceCollection AddExcelExport(this IServiceCollection services)
        {
            services.AddSingleton<HtmlDocumentService>();
            services.AddSingleton<ExcelWorkBookService>();
            services.AddSingleton<JsonSerializerService>();
            services.AddScoped<IHelperService, HelperService>();

            return services;
        }
    }
}
