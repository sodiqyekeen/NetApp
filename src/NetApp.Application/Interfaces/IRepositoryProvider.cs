using NetApp.Domain.Repositories;

namespace NetApp.Application.Services;

public interface IRepositoryProvider
{
    public IAuthenticationTokenRepository AuthenticationTokenRepository { get; }
    public IEmailTemplateRepository EmailTemplateRepository { get; }
    public ISessionRepository SessionRepository { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
}
