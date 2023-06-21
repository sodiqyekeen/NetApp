namespace NetApp.Application.Services;
public interface IEmailService
{
    Task SendAsync(EmailRequest request);
}
