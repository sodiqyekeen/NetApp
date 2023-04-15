namespace NetApp.Application.Dtos.Identity;

public class PermissionResponse
{
    public string RoleId { get; set; } = null!;
    public string RoleName { get; set; } = null!;
    public List<RoleClaimResponse> RoleClaims { get; set; } = new();
}
