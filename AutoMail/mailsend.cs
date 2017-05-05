using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace AutoMail
{

    public class mailsend
    {
        public static void sendmail(string mailconfig, string subject, string tomail, string body, string attachmentCol, string CC, string BCC)
        {
            try
            {

                MailServer objmailserver = new MailServer(mailconfig);

                SmtpClient client = new SmtpClient(objmailserver.SMTP);
                client.UseDefaultCredentials = false;
                client.EnableSsl = Convert.ToBoolean(objmailserver.Enablessl);
                if (objmailserver.Port != -1)
                {
                    client.Port = objmailserver.Port;
                }
                client.Credentials = new NetworkCredential(objmailserver.Username, objmailserver.Password);
                MailAddress from = new MailAddress(objmailserver.FromMail);
                MailMessage mail = new MailMessage();
                mail.From = from;
                if (tomail != "")
                {
                    foreach (var item in tomail.Split(','))
                    {
                        MailAddress to = new MailAddress(item);
                        mail.To.Add(to);
                    }
                }
                if (CC != "")
                {
                    foreach (var item in CC.Split(','))
                    {
                        MailAddress cc = new MailAddress(item);
                        mail.CC.Add(cc);
                    }

                }
                if (BCC != "")
                {
                    foreach (var item in BCC.Split(','))
                    {
                        MailAddress bcc = new MailAddress(item);
                        mail.Bcc.Add(bcc);
                    }
                }
                if (attachmentCol != "")
                {
                    foreach (var item in attachmentCol.Split(','))
                    {
                        mail.Attachments.Add(new Attachment(item));
                    }
                }
                mail.Subject = subject;
                mail.IsBodyHtml = true;
                mail.Body = body;
                client.Send(mail);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private class MailServer
        {
            public string FromMail { get; set; }
            public string ToMail { get; set; }
            public string SMTP { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public int Port { get; set; }
            public bool IsHtmlBody { get; set; }
            public bool Enablessl { get; set; }
            public MailServer(string type)
            {
                this.FromMail = ConfigurationManager.AppSettings[type + "FromMail"].ToString();
                this.ToMail = ConfigurationManager.AppSettings[type + "ToMail"].ToString();
                this.SMTP = ConfigurationManager.AppSettings[type + "SMTP"].ToString();
                this.Username = ConfigurationManager.AppSettings[type + "Username"].ToString();
                this.Password = ConfigurationManager.AppSettings[type + "Password"].ToString();
                if (ConfigurationManager.AppSettings[type + "Port"] != null)
                {
                    this.Port = Convert.ToInt32(ConfigurationManager.AppSettings[type + "Port"].ToString());
                }
                else {
                    this.Port = -1;
                }
                this.Enablessl = Convert.ToBoolean(ConfigurationManager.AppSettings[type + "Enablessl"].ToString());
                this.IsHtmlBody = true;
            }

        }

    }
}
