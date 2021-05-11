using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Mail;
using Kharban_WebAPI.Repository;
using Kharban_WebAPI.Helper;

namespace Kharban_WebAPI.Common
{
    /// <summary>
    /// Summary description for MailAlerts
    /// </summary>
    public class MailSend
    {
        private string _smtpserver = string.Empty;
        private string _Port = string.Empty;
        private string _Authenticate = string.Empty;
        private string _UserName = string.Empty;
        private string _ePassword = string.Empty;
        private string _UseSSL = string.Empty;
        private string _EmailFrom = string.Empty;

        public MailSend(string Website)
        {
            try
            {
                string myEmailCredentials = SiteKey.DKR_MAIL;
                string[] settings = myEmailCredentials.Split(';');
                _smtpserver = GetValue(settings, "smtpserver");
                _Port = GetValue(settings, "Port");
                _Authenticate = GetValue(settings, "Authenticate");
                _UserName = GetValue(settings, "UserName");
                _ePassword = GetValue(settings, "ePassword");
                _UseSSL = GetValue(settings, "UseSSL");
                _EmailFrom = GetValue(settings, "EmailFrom");
            }
            catch (Exception ex)
            {
                LoggingRepository.SaveException(ex);
            }
        }

        public bool SendMail(string MailTO, string EmailSubject, string EmailBody, Stream stream = null, string attachmentName = null)
        {
            try
            {
                //MailTO = "hiringshopjitendra@gmail.com";
                string Website = SiteKey.SiteUrl;
                string smtpAddress = _smtpserver;
                int portNumber = int.Parse(_Port);
                bool enableSSL = (_UseSSL.ToLower() == "true" ? true : false);
                string emailFrom = _EmailFrom;
                string password = _ePassword;
                string username = _UserName;
                string emailTo = MailTO;
                string subject = EmailSubject;
                string body = EmailBody;

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    mail.To.Add(emailTo);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    if (stream != null)
                    {
                        mail.Attachments.Add(new Attachment(stream, attachmentName));
                    }

                    // Can set to false, if you are sending pure text.
                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(username, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }
                return true;

            }
            catch (Exception ex)
            {
                LoggingRepository.SaveException(ex);
                return false;
            }
        }

        private string GetValue(string[] settings, string configsettingcontain)
        {
            string value = string.Empty;
            configsettingcontain = settings.Where(C => C.Contains(configsettingcontain)).SingleOrDefault();
            value = configsettingcontain.Substring(configsettingcontain.LastIndexOf("=") + 1);
            return value;
        }


        public bool SendMailAttachments(string MailTO, string EmailSubject, string EmailBody, Attachment atfile)
        {
            try
            {
                string smtpAddress = _smtpserver;
                int portNumber = int.Parse(_Port);
                bool enableSSL = (_UseSSL.ToLower() == "true" ? true : false);
                string emailFrom = _UserName;
                string password = _ePassword;
                string emailTo = MailTO;
                string subject = EmailSubject;
                string body = EmailBody;
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    mail.To.Add(emailTo);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    // Can set to false, if you are sending pure text.
                    if (atfile != null)
                    {
                        mail.Attachments.Add(atfile);
                    }
                    //mail.Attachments.Add(new Attachment("C:\\SomeFile.txt"));
                    //mail.Attachments.Add(new Attachment("C:\\SomeZip.zip"));

                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(emailFrom, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }
                return true;

            }
            catch (Exception ex)
            {
                LoggingRepository.SaveException(ex);
                return false;
            }
        }

    }
}