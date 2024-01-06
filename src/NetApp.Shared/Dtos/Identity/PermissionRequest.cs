using System.ComponentModel.DataAnnotations;

namespace NetApp.Dtos;

public class PermissionRequest
{
    [Required]
    public string RoleId { get; set; } = null!;
    public List<PermissionBase> Permissions { get; set; } = [];
}
