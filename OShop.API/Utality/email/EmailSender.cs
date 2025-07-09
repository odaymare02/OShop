using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace OShop.API.Utality.email
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)//name of provider who support me send emails,and number of port
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("mareoday123@gmail.com", "mail pqxx ydax eudh\r\n")
            };

            return client.SendMailAsync(
                new MailMessage(from: "OShope@shope.com",
                                to: email,
                                subject,
                                message
                                )
                {
                    IsBodyHtml=true// to allow the body of message to be as html 
                }
                );
        }
    
    }
}

