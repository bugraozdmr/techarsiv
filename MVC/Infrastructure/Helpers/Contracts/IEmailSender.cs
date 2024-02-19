namespace GargamelinBurnu.Infrastructure.Helpers.Contracts;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
}