using System.ComponentModel.DataAnnotations;

namespace NetApp.Dtos;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
}
