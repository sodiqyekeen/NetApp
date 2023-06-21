using System.ComponentModel.DataAnnotations;

namespace NetApp.Dtos;

public class RoleRequest
{
    public string? Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Description { get; set; }
}