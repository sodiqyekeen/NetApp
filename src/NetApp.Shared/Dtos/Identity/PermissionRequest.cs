using System.ComponentModel.DataAnnotations;

namespace NetApp.Application.Dtos.Identity;

public class PermissionRequest
{
    [Required]
    public string? RoleId { get; set; }
    public List<PermissionBase> Permissions { get; set; } = new();
}
