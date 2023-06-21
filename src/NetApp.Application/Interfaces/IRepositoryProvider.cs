using NetApp.Domain.Repositories;

namespace NetApp.Application.Services;

public interface IRepositoryProvider
{
    public IAuthenticationTokenRepository AuthenticationTokenRepository { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
}
