using NetApp.Domain.Entities;

namespace NetApp.Domain.Repositories;

public interface ISessionRepository
{
    Task AddSessionAsync(Session session);
    Task<Session?> GetSessionByIdAsync(Guid id);
    Task<Session?> GetSessionByRefreshTokenAsync(string refreshToken);
    //Task<bool> IsSessionValidAsync(Guid id);
    void DeleteSession(Session session);
}
