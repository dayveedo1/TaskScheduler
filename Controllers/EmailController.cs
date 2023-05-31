using Hangfire;
using HangfireTest.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace HangfireTest.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [EnableCors("AllowAll")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSender _emailSender;

        public EmailController(IEmailSender _emailSender)
        {
            this._emailSender = _emailSender;
        }

        [HttpGet("send")]
        public async Task<IActionResult> SendEmail()
        {

            var message = new Message()
            {
                To = "dayveedo47@gmail.com",
                Subject = "HangFire Test Update",
                Content = "HangFire Job Executed",
                //Attachments = 
            };

            try
            {
                await _emailSender.SendEmailAsync(message);
                return Ok();

            } catch (Exception ex)
            {
                throw;
            }
        }

       [HttpGet("job")]
       public IActionResult Job()
        {
            RecurringJob.AddOrUpdate(() => SendEmail(), Cron.Minutely, TimeZoneInfo.Local);
            return Ok();
        }
    }
}
