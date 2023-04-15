using System.ComponentModel.DataAnnotations;

namespace NetApp.Application.Dtos.Identity;

public class UpdateUserRoleRequest
{
    [Required]
    public string? UserId { get; set; }

    [Required]
    public UserRoleModel SelectedRole { get; set; } = null!;
}
