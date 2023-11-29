using System.ComponentModel.DataAnnotations;

namespace NetApp.Dtos;

public class AuthenticationRequest
{
    private string? _email;
    [Required]
    [EmailAddress]
    public string Email
    {
        get => string.IsNullOrWhiteSpace(_email) ? "" : _email.Trim();
        set => _email = value;
    }

    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    public bool RememberMe { get; set; }
}
