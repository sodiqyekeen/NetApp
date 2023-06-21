using System.ComponentModel.DataAnnotations;

namespace NetApp.Dtos;

public class UpdateUserRoleRequest
{
    [Required]
    public string? UserId { get; set; }

    [Required]
    public List<string> Roles { get; set; } = new();
    // public UserRoleModel SelectedRole { get; set; } = null!;
}
