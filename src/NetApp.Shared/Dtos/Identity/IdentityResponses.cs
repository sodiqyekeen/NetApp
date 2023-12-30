namespace NetApp.Dtos;

public record AuthenticationResponse(string JWToken, string RefreshToken);

public record UserRolesResponse(string UserId, List<UserRoleModel> Roles);

public record UserRoleModel(string RoleId, string RoleName, string RoleDescription);

// public record User(
//      string Id,
//      string UserName,
//      string Email,
//      //List<string> Roles,
//      bool Active
// );