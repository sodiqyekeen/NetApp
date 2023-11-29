namespace NetApp.UI.Infrastructure.Store;

public record CurrentUser(
    string Username,
    string Email,
    string? ImageUrl=null);

