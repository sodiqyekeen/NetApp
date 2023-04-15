using NetApp.Domain.Exceptions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NetApp.Application.Common;
using NetApp.Domain.Models;
using NetApp.Application.Dtos.Common;

namespace NetApp.Infrastructure.Common;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly MailSettings _mailSettings;

    public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger)
    {
        _logger = logger;
        _mailSettings = mailSettings.Value;
    }

    public async Task SendAsync(EmailRequest request)
    {
        try
        {
            var email = new MimeMessage
            {
                Subject = request.Subject,
                Body = new BodyBuilder { HtmlBody = request.Body }.ToMessageBody()
            };
            email.From.Add(MailboxAddress.Parse(request.From ?? $"{_mailSettings.DisplayName} <{_mailSettings.EmailFrom}>"));
            email.To.Add(MailboxAddress.Parse(request.To));

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.SmtpHost, _mailSettings.SmtpPort, SecureSocketOptions.SslOnConnect);
            smtp.Authenticate(_mailSettings.SmtpUser, _mailSettings.SmtpPass);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            if (ex.Message == "No Such User Here")
                throw new InvalidEmailAddressException(request.To!);
            throw;
        }

    }

}