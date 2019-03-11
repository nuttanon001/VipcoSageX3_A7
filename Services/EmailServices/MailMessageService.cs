﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using VipcoSageX3.ViewModels;

namespace VipcoSageX3.Services.EmailServices
{
    public class MailMessageService
    {
        public MailMessage Create(EmailViewModel email)
        {
            var mailMessage = new MailMessage()
            {
                From = new MailAddress(email.MailFrom, email.NameFrom),
                IsBodyHtml = true,
                Body = email.Message,
                Subject = email.Subject,
                // Add MailAddress To
            };

            email.MailTos.ForEach(item => mailMessage.To.Add(item));

            if (email.HasAttach.HasValue && email.HasAttach.Value)
                mailMessage.Attachments.Add(email.Attachment);

            return mailMessage;
        }
    }
}
