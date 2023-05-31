using MimeKit;

namespace HangfireTest.Data
{
    public class Message
    {
        //public List<MailboxAddress> To { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public List<IFormFile> Attachments { get; set; }

    }
}
