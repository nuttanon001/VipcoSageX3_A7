﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VipcoSageX3.ViewModels;

namespace VipcoSageX3.Services.EmailServices
{
    public class EmailSender:IEmailSender
    {
        private readonly MailMessageService mailMessageService;
        private readonly NetworkClient networkClient;
        private bool invalid;

        public EmailSender(MailMessageService mailMessage,NetworkClient client)
        {
            this.mailMessageService = mailMessage;
            this.networkClient = client;
            this.invalid = false;
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
        /// <summary>
        /// Valid email address.
        /// </summary>
        /// <param name="strIn">Email address to check</param>
        /// <returns></returns>
        public bool IsValidEmail(string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format.
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public async Task<bool> SendMail(EmailViewModel email)
        {
            var message = this.mailMessageService.Create(email);
            var result = await this.networkClient.SendEmail(message);
            // Dispose attachment
            if (email.HasAttach.HasValue && email.HasAttach.Value)
                email.Attachment.Dispose();

            return result;
        }

        public bool SendMailNoneAsync(EmailViewModel email)
        {
            var message = this.mailMessageService.Create(email);
            return this.networkClient.SendEmailNoneAsync(message);
        }
    }
}
