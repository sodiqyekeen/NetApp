using NetApp.Domain.Repositories;
using NetApp.Infrastructure.Contexts;
using NetApp.Infrastructure.Repositories;

namespace NetApp.Infrastructure;

public class RepositoryProvider(NetAppDbContext dbContext) : IRepositoryProvider
{
    private EmailTemplateRepository? _emailTemplateRepository;
    private AuthenticationTokenRepository? _authenticationTokenRepository;
    private SessionRepository? _sessionRepository;
    public IAuthenticationTokenRepository AuthenticationTokenRepository => _authenticationTokenRepository ??= new AuthenticationTokenRepository(dbContext);
    public IEmailTemplateRepository EmailTemplateRepository => _emailTemplateRepository ??= new EmailTemplateRepository(dbContext);
    public ISessionRepository SessionRepository => _sessionRepository ??= new SessionRepository(dbContext);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken()) => await dbContext.SaveChangesAsync(cancellationToken);
}
