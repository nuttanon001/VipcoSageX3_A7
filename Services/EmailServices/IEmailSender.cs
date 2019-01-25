using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VipcoSageX3.ViewModels;

namespace VipcoSageX3.Services.EmailServices
{
    public interface IEmailSender
    {
        Task<bool> SendMail(EmailViewModel email);
        bool SendMailNoneAsync(EmailViewModel email);

        bool IsValidEmail(string strIn);
    }
}
