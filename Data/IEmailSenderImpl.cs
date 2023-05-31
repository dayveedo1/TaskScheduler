using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace HangfireTest.Data
{
    public class IEmailSenderImpl : IEmailSender
    {
        private readonly EmailConfig _emailConfig;

        public IEmailSenderImpl(IOptions<EmailConfig> config)
        {
            this._emailConfig = config.Value;
        }

        public async Task SendEmailAsync(Message message)
        {
            if (_emailConfig == null || string.IsNullOrEmpty(_emailConfig.Name) || string.IsNullOrEmpty(_emailConfig.From))
                throw new InvalidOperationException("Email configuration is not set up correctly.");

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailConfig.Name, _emailConfig.From));
            email.To.Add(MailboxAddress.Parse(message.To));
            //email.To.Add(MailboxAddress.Parse("dayveedo47@gmail.com"));
            email.Subject = message.Subject;
            //email.Subject = "HangFire Test Update";


            BodyBuilder builder = new();
            if (message.Attachments != null)
            {
                byte[] filebytes;

                foreach(var file in message.Attachments)
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
            smtp.Connect(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.StartTls);
            smtp.AuthenticationMechanisms.Remove("XOAUTH2");
            smtp.Authenticate(_emailConfig.Username, _emailConfig.Password);

            await smtp.SendAsync(email);

            smtp.Disconnect(true);
            smtp.Dispose();

        }
      
    }
}
