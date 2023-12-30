using Microsoft.AspNetCore.Identity;

namespace NetApp.Infrastructure.Identity.Models;

public class NetAppRoleClaim : IdentityRoleClaim<string>
{
    public override int Id { get; set; }
    public NetAppRole Role { get; set; } = null!;
}
