using System.ComponentModel.DataAnnotations;

namespace NetApp.Application.Dtos.Identity;

public class RoleRequest
{
    public string? Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Description { get; set; }
}