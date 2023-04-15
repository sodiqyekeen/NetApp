using NetApp.Domain.Repositories;

namespace NetApp.Application.Interfaces;

public interface IRepositoryProvider
{
    public IAuthenticationTokenRepository AuthenticationTokenRepository { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
}
