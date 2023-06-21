namespace NetApp.Application.Services;

public interface ISessionService
{
    string Username { get; }
    string Email { get; }
    string UserId { get; }
    string Role { get; }
}
