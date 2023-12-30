using Microsoft.EntityFrameworkCore;
using NetApp.Domain.Entities;
using NetApp.Domain.Repositories;
using NetApp.Infrastructure.Contexts;

namespace NetApp.Infrastructure.Repositories;

internal class AuthenticationTokenRepository(NetAppDbContext dbContext) : RepositoryBase<AuthenticationToken>(dbContext), IAuthenticationTokenRepository
{
    public async Task AddTokenAsync(AuthenticationToken authenticationToken) => await AddAsync(authenticationToken);

    public void UpdateToken(AuthenticationToken authenticationToken) => Update(authenticationToken);

    public void DeleteToken(AuthenticationToken authenticationToken) => Delete(authenticationToken);

    public async Task<AuthenticationToken?> GetTokenByKeyAsync(string key) => await GetAll().FirstOrDefaultAsync(x => x.Key == key);

    public async Task<AuthenticationToken?> GetTokenByValueAsync(string value) => await GetAll().FirstOrDefaultAsync(x => x.Value == value);
}
