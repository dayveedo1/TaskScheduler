namespace HangfireTest.Data
{
    public interface IEmailSender
    {
        Task SendEmailAsync(Message message);
    }
}
