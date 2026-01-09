using Microsoft.AspNetCore.Identity.UI.Services;

namespace Kara_OK.Web.Services;

public class DevEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
        => Task.CompletedTask;
}