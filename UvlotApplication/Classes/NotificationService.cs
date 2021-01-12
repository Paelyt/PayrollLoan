﻿using System;
using System.Collections;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using UvlotApplication.Classes;

namespace UvlotApplication.Classes
{
    public partial class NotificationService
    {
        private static readonly SmtpClient SmtpServer = new SmtpClient();

        public static bool SendMail(MailMessage mail)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, certificate, chain, sslPolicyErrors) => true;
                mail.From = new MailAddress("mail@Paytrx.com");
                mail.IsBodyHtml = true;
                SmtpServer.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                return false;
            }
        }
        public static string SendSms(TransactionAlert tAlert)
        {
            string smsStatus;
            tAlert.BaseUrl = ConfigurationManager.AppSettings["SmsBaseUrl"];
            tAlert.Username = ConfigurationManager.AppSettings["SmsUserName"];
            tAlert.Password = ConfigurationManager.AppSettings["SmsPassword"];
            tAlert.Sender= ConfigurationManager.AppSettings["SmsSender"];

            var smsParameters =
                $"username={tAlert.Username}&password={tAlert.Password}&sender={tAlert.Sender}&mobiles={tAlert.PhoneNumber}&recipient={tAlert.PhoneNumber}&message={tAlert.Message}";

            //http://portal.bulksmsnigeria.net/api/?username=godwinawojobi@yahoo.com&password=captain12&message=Sent%20SMS!&sender=Godwin&mobiles=2347057987704

            using (var wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                smsStatus = wc.UploadString(tAlert.BaseUrl, smsParameters);
            }
            return smsStatus;
        }

        public static bool SendMail(string msgSubject, string msgBody, string addressTo, string addressCc, string addressBcc)
        {

            try
            {
                ServicePointManager.ServerCertificateValidationCallback +=
                   (sender, certificate, chain, sslPolicyErrors) => true;

                var smtpServer = ConfigurationManager.AppSettings["MailServerAddress"];
                WebLog.Log("smtpServer:" + smtpServer);
                var smtpServerPort = ConfigurationManager.AppSettings["SMTPServerPort"];
                WebLog.Log("SMTPServerPort:" + smtpServerPort);
                var mailFrom = ConfigurationManager.AppSettings["MailFrom"];
                WebLog.Log("mailFrom:" + mailFrom);
                var mailFromPassword = ConfigurationManager.AppSettings["MailFromPassword"];
                WebLog.Log("mailFromPassword:" + mailFromPassword);

                var email = new MailMessage
                {
                    From = new MailAddress(mailFrom, msgSubject),
                    Subject = msgSubject,
                    IsBodyHtml = true,
                    Body = msgBody,
                };

                
              //  var FIlePath = (@"C:\\Users\\Reliance Limited\\Documents\\Visual Studio 2015\\Projects\\UvlotApplication\\UvlotApplication\\Images\\Receipt.PNG");
              //  email.Attachments.Add(new Attachment(FIlePath));
             
                WebLog.Log("email:" + email.From + email.Subject +email.IsBodyHtml +email.Body);
               // email.Attachments.Add(new Attachment(addressBcc));
                //mm.Attachments.Add(New Attachment(New MemoryStream(bytes), "iTextSharpPDF.pdf"));
               email.To.Add(addressTo);
                WebLog.Log("email:" + email);
                if (addressCc != null) email.CC.Add(addressCc);
                if (addressBcc != null) email.Bcc.Add(addressBcc);

                var mailClient = new SmtpClient();

                var basicAuthenticationInfo = new NetworkCredential(mailFrom, mailFromPassword);
                WebLog.Log("basicAuthenticationInfo:" + basicAuthenticationInfo);
                mailClient.Host = smtpServer;
                WebLog.Log("mailClient.Host:" + mailClient.Host);
                mailClient.Credentials = basicAuthenticationInfo;
                WebLog.Log("mailClient.Credentials:" + mailClient.Credentials);
                mailClient.Port = int.Parse(smtpServerPort);
                WebLog.Log("mailClient.Port:" + mailClient.Port);
                mailClient.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["EnableSsl"]);
                WebLog.Log("mailClient.EnableSsl:" + mailClient.EnableSsl);
                mailClient.Send(email);


                return true;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
            }
            return false;
        }


        public class TransactionAlert
        {
            public virtual string BaseUrl { get; set; }
            public virtual string Username { get; set; }
            public virtual string Password { get; set; }
            public virtual string Sender { get; set; }
            public virtual string PhoneNumber { get; set; }
            public virtual string Message { get; set; }
        }

    }
}
