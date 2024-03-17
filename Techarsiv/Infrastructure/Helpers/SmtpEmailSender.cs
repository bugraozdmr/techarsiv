using System.Net;
using System.Net.Mail;
using GargamelinBurnu.Infrastructure.Helpers.Contracts;
using MailKit.Security;
using MimeKit;

namespace GargamelinBurnu.Infrastructure.Helpers;

public class SmtpEmailSender : IEmailSender
{
    private string? _host;
    private int _port;
    private bool _enableSSL;
    private string? _username;
    private string? _password;

    
    public SmtpEmailSender(string? host, int port, bool enableSsl, string? username, string? password)
    {
        _host = host;
        _port = port;
        _enableSSL = enableSsl;
        _username = username;
        _password = password;
    }

    public async Task<int> SendEmailAsync(string email, string subject, string message)
    {
        try
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(MailboxAddress.Parse(_username ?? ""));
            mimeMessage.To.Add(MailboxAddress.Parse(email));
            mimeMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = message;

            mimeMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync (_host, _port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_username, _password);
                await client.SendAsync(mimeMessage);
                await client.DisconnectAsync(true);
            }

            return 1; // Başarılı oldu, 1 dön
        }
        catch (Exception ex)
        {
            return -1; // Başarısız oldu, -1 dön
        }
    }


    public async Task SendMultipleEmailsAsync(IEnumerable<string> emails, string subject, string message)
    {
        var tasks = emails.Select(email => SendEmailAsync(email, subject, message));
        await Task.WhenAll(tasks);
    }
}