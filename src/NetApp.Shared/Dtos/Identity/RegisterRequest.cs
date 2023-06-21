using System.ComponentModel.DataAnnotations;

namespace NetApp.Dtos;

public class RegisterRequest
{
    [Required]
    public string? Email { get; set; }

    [Required]
    public string? Username { get; set; }
}
