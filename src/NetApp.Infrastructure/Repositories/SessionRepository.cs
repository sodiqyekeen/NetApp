using Microsoft.EntityFrameworkCore;
using NetApp.Domain.Entities;
using NetApp.Domain.Repositories;
using NetApp.Infrastructure.Contexts;

namespace NetApp.Infrastructure.Repositories;

internal class SessionRepository(NetAppDbContext dbContext) : RepositoryBase<Session>(dbContext), ISessionRepository
{
    public async Task AddSessionAsync(Session session) => await AddAsync(session);

    public void DeleteSession(Session session) => Delete(session);

    public async Task<Session?> GetSessionByIdAsync(Guid id) => await GetByIdAsync(id);

    public async Task<Session?> GetSessionByRefreshTokenAsync(string refreshToken) =>
        await _dbContext.Sessions.FirstOrDefaultAsync(s => s.RefreshToken == refreshToken);

    //public async Task<IEnumerable<Session>> GetActiveSessions

}
