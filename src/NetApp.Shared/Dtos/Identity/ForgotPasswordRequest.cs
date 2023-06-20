using System.ComponentModel.DataAnnotations;

namespace NetApp.Application.Dtos.Identity;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
}
