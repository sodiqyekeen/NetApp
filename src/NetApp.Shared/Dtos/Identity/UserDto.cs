namespace NetApp.Dtos;

public class UserDto
{
    public string Id { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? OtherNames { get; set; }
    public string Email { get; set; } = null!;
    public List<string> Role { get; set; } = new List<string>();
    public bool Active { get; set; }
    public DateTime? LastLoginOn { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public bool EmailConfirmed { get; set; }
    public string Intials => FirstName[..1] + LastName[..1];
    public string FullName => $"{FirstName} {LastName}";
}
