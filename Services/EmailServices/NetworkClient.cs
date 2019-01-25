using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VipcoSageX3.ViewModels;

namespace VipcoSageX3.Services.EmailServices
{
    public class NetworkClient
    {
        private readonly SmtpClientService smtpClientService;
        private readonly ILogger _logger;

        public NetworkClient(SmtpClientService smtpClient,ILogger<NetworkClient> logger)
        {
            this.smtpClientService = smtpClient;
            this._logger = logger;
        }

        public async Task<bool> SendEmail(MailMessage mailMessage)
        {
            try
            {
                using (SmtpClient client = this.smtpClientService.Create())
                {
                    await client.SendMailAsync(mailMessage);
                    return true;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError($"Has error {ex.ToString()}");
            }
            return false;
        }

        public  bool SendEmailNoneAsync(MailMessage mailMessage)
        {
            try
            {
                using (SmtpClient client = this.smtpClientService.Create())
                {
                    client.Send(mailMessage);
                    return true;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError($"Has error {ex.ToString()}");
            }
            return false;
        }
    }
}
