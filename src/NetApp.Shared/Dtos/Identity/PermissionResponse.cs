namespace NetApp.Dtos;

public class PermissionResponse
{
    public string RoleId { get; set; } = null!;
    public string RoleName { get; set; } = null!;
    public string RoleDescription { get; set; } = null!;
    public List<ModulePermission> RolePermissions { get; set; } = [];
}

public record Permission(string Name, string Value, string Description) : PermissionBase(Value);

public record ModulePermission(string Name, string Description, List<Permission> Permissions);

public record PermissionBase(string Value, bool Selected = false)
{
    public bool Selected { get; set; } = Selected;
}

public record RoleWithPermissionsResponse(string Id, string Name, string Description, List<string> Permissions);