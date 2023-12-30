using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using NetApp.Domain.Exceptions;
using NetApp.Domain.Models;
using RazorLight;

namespace NetApp.Infrastructure.Services;

internal class EmailService(IOptions<MailSettings> mailSettings, IRepositoryProvider repositoryProvider, ILogger<EmailService> logger) : IEmailService
{
    private readonly MailSettings _mailSettings = mailSettings.Value;
    public async Task SendAsync(EmailRequest request)
    {
        try
        {
            var template = (await repositoryProvider.EmailTemplateRepository.GetTemplateByNameAsync(request.TemplateName))?? throw new ApiException("Invalid template name.");

            var engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(typeof(EmailService))
                .SetOperatingAssembly(typeof(EmailService).Assembly)
                .UseMemoryCachingProvider()
                .Build();

            string mailBody = await engine.CompileRenderStringAsync(template.Name, template.Content, request.ReplacementValues);

            var email = new MimeMessage
            {
                Subject = template.Subject,
                Body = new BodyBuilder { HtmlBody=mailBody }.ToMessageBody()
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
            logger.LogError(ex.Message, ex);
            if (ex.Message == "No Such User Here")
                throw new InvalidEmailAddressException(request.To!);
            throw;
        }

    }

}