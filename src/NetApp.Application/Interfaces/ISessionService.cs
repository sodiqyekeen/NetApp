using NetApp.Domain.Entities;

namespace NetApp.Application.Services;

public interface ISessionService
{
    Session? CurrentSession { get; }
    string? Username { get; }
    string? Email { get; }
    string? UserId { get; }
    List<string> Roles { get; }
}
