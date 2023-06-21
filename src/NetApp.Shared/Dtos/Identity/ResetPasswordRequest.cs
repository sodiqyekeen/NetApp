using System.ComponentModel.DataAnnotations;

namespace NetApp.Dtos;

public class ResetPasswordRequest
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string? Token { get; set; }

    [Required]
    [MinLength(6)]
    public string? Password { get; set; }

    [Required]
    [Compare(nameof(Password))]
    public string? ConfirmPassword { get; set; }
}


public class UpdatePasswordRequest
{
    [Required]
    public string? CurrentPassword { get; set; }

    [Required]
    [MinLength(6)]
    public string? NewPassword { get; set; }

    [Required]
    [Compare(nameof(NewPassword))]
    public string? ConfirmPassword { get; set; }
}