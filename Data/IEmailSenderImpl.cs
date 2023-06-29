using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace HangfireTest.Data
{
    public class IEmailSenderImpl : IEmailSender
    {
        private readonly EmailConfig _emailConfig;
        private IConfiguration configuration;

        public IEmailSenderImpl(IOptions<EmailConfig> config, IConfiguration configuration)
        {
            this._emailConfig = config.Value;
            this.configuration = configuration;
        }

        public async Task SendEmailAsync(Message message)
        {
            //if (_emailConfig == null || string.IsNullOrEmpty(_emailConfig.Name) || string.IsNullOrEmpty(_emailConfig.From))
            //    throw new InvalidOperationException("Email configuration is not set up correctly.");
            try
            {
                var email = new MimeMessage();
                //email.From.Add(new MailboxAddress(_emailConfig.Username, _emailConfig.From));
                email.From.Add(new MailboxAddress("Your account name", "your email address"));
                email.To.Add(MailboxAddress.Parse(message.To));
                email.Subject = message.Subject;

                BodyBuilder builder = new();
                if (message.Attachments != null)
                {
                    byte[] filebytes;

                    foreach (var file in message.Attachments)
                    {
                        if (file.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                file.CopyTo(ms);
                                filebytes = ms.ToArray();
                            }

                            builder.Attachments.Add(file.FileName, filebytes, ContentType.Parse(file.ContentType));
                        }
                    }
                }

                builder.HtmlBody = message.Content;    //"HangFire Job Executed";
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtp.AuthenticationMechanisms.Remove("XOAUTH2");
                //smtp.Authenticate(_emailConfig.Username, _emailConfig.Password);
                smtp.Authenticate("your email account", "secure password generated from google app for your email account"); //had to create a google app password for this

                await smtp.SendAsync(email);

                smtp.Disconnect(true);
                smtp.Dispose();
            } catch (Exception ex)
            {
                throw;
            }

          

        }
      
    }
}
