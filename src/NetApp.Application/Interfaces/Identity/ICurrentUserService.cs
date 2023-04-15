namespace NetApp.Application.Interfaces.Identity;

public interface ICurrentUserService
{
    string Username { get; }
    string Email { get; }
    bool IsSuperAdmin { get; }
    string UserId { get; }
}
