using System.ComponentModel.DataAnnotations;

namespace NetApp.Application.Dtos.Identity;

public class RegisterRequest
{
    [Required]
    public string? Email { get; set; }

    [Required]
    public string? Username { get; set; }
}
