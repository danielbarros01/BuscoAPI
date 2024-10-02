using BuscoAPI.DTOS;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using MailKit.Security;

namespace BuscoAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;


        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        public void SendEmail(MailRequest request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(configuration.GetSection("MailSettings:Mail").Value));
            email.To.Add(MailboxAddress.Parse(request.ToEmail));
            email.Subject = request.Subject;
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = request.Body
            };

            using var smtp = new SmtpClient();
      
            smtp.Connect(
                configuration.GetSection("MailSettings:Host").Value,
                Convert.ToInt32(configuration.GetSection("MailSettings:Port").Value),
                SecureSocketOptions.StartTls
            );


            smtp.Authenticate(
                configuration.GetSection("MailSettings:Mail").Value,
                configuration.GetSection("MailSettings:Password").Value
            );


            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
