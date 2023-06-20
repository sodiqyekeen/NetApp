namespace NetApp.Application.Dtos.Identity;

public class PermissionResponse
{
    public string RoleId { get; set; } = null!;
    public string RoleName { get; set; } = null!;
    public string RoleDescription { get; set; } = null!;
    public List<ModulePermission> RolePermissions { get; set; } = new();
}

public record Permission(string Name, string Value, string Description): PermissionBase(Value);

public record ModulePermission(string Name, string Description, List<Permission> Permissions);

public record PermissionBase(string Value){
     public bool Selected { get; set; }
}