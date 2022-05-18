using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace TicketDesk.Web.Client.Models
{
    public static class Utility
    {
        public static bool SendEmail(string toEmail, string Body, string Subject)
        {
            try
            {
                string FromMail = System.Configuration.ConfigurationManager.AppSettings["FromMail"];
                string DisplayName = System.Configuration.ConfigurationManager.AppSettings["DisplayName"];
                string FromPassword = System.Configuration.ConfigurationManager.AppSettings["FromPassword"];
                string Host = System.Configuration.ConfigurationManager.AppSettings["Host"];
                int Port = int.Parse( System.Configuration.ConfigurationManager.AppSettings["Port"]);
               
                var fromAddress = new MailAddress(FromMail, DisplayName);
                var toAddress = new MailAddress(toEmail, DisplayName);
               
                string subject = $"{Subject}";
                
                var smtp = new SmtpClient
                {
                    Host = Host,
                    Port = Port,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = true, 
                    Credentials = new NetworkCredential(fromAddress.Address, FromPassword)
                };
                using (var message1 = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = Body
                })
                {
                    smtp.Send(message1);
                }

            }
            catch (Exception ex)
            {

            }
            return true;
        }
    }
}