using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ModelLayer.Model;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RepositoryLayer.EmailService
{
    public class EmailService
    {
        public static bool ForgetpasswordMail(ForgetPasswordModel model,string port, string host, string fromAddress, string fromPassword, string subject, string body, string resturl)
        {                  
            var receiverEmailId = model.Email;

            var fromAdd = new MailAddress(fromAddress, "Shaikh Abid");
            var toAddress = new MailAddress(model.Email, "To Name");                      

            try
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com")
                {
                    Host = host,
                    Port = Convert.ToInt32(port),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAdd.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAdd, toAddress)
                {
                    Subject = subject,
                    Body = resturl + "?token="+body
                })
                    client.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }
    }           
}
