using Microsoft.AspNetCore.Identity.UI.Services;

namespace StampSystem.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Here you would implement the logic to send an email.
            return Task.CompletedTask;
        }
    }
}
