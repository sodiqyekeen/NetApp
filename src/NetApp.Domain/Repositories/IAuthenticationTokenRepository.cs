using NetApp.Domain.Entities;

namespace NetApp.Domain.Repositories;

public interface IAuthenticationTokenRepository
{
    Task AddTokenAsync(AuthenticationToken authenticationToken);
    Task<AuthenticationToken?> GetTokenByKeyAsync(string key);
    Task<AuthenticationToken?> GetTokenByValueAsync(string value);
    void DeleteToken(AuthenticationToken authenticationToken);
}
