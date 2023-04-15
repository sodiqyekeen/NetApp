namespace NetApp.Application.Dtos.Identity;

public record AuthenticationResponse(string JWToken, string RefreshToken);

public record UserRolesResponse(List<UserRoleModel> UserRoles);

public record UserRoleModel(string RoleName, string RoleDescription, bool Selected);

public record User(
     string Id,
     string UserName,
     string Email,
     IList<string> Roles,
     bool Active
);