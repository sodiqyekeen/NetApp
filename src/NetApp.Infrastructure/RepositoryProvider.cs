using NetApp.Domain.Repositories;

namespace NetApp.Infrastructure;

public class RepositoryProvider : IRepositoryProvider
{
    public IAuthenticationTokenRepository AuthenticationTokenRepository => throw new NotImplementedException();

    public Task SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();
}
