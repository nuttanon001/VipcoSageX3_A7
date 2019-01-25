using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoSageX3.Services.TaskServices
{
    public static class TaskServiceCollectionExtensions
    {
        public static IServiceCollection AddTaskSchedule(this IServiceCollection services)
        {
            // Host service for task schedule
            services.AddHostedService<TaskService>();

            return services;
        }
    }
}
