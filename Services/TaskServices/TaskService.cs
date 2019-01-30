using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VipcoSageX3.Services.EmailServices;
using VipcoSageX3.ViewModels;

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
     
                using (var scope = this.Services.CreateScope())
                {
                    var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                    await scopedProcessingService.SendMail(new ViewModels.EmailViewModel()
                    {
                        MailFrom = "to.nuttanon@vipco-thai.com",
                        MailTos = new List<string>() { "to.nuttanon@vipco-thai.com" },
                        Message = $"Time server is :{dateNow}",
                        NameFrom = "Nuttanon",
                        Subject = "Schedule Task"
                    });
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
