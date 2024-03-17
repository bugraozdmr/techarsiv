namespace GargamelinBurnu.Infrastructure.Helpers.Contracts;

public interface IEmailSender
{
    Task<int> SendEmailAsync(string email, string subject, string message);
    Task SendMultipleEmailsAsync(IEnumerable<string> emails, string subject, string message);
}