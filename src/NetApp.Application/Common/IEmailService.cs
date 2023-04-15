using NetApp.Application.Dtos.Common;

namespace NetApp.Application.Common;
public interface IEmailService
{
    Task SendAsync(EmailRequest request);
}
