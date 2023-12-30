using NetApp.Domain.Entities;

namespace NetApp.Application.Services;

public interface ISessionService
{
    Session CurrentSession { get; }
    string Username { get; }
    string Email { get; }
    string UserId { get; }
    string Role { get; }
}
