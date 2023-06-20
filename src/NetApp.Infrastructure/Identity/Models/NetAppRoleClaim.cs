using Microsoft.AspNetCore.Identity;

namespace NetApp.Infrastructure.Identity.Models;

public class NetAppRoleClaim : IdentityRoleClaim<string> //, IEntity<int>
{
    public NetAppRole? Role { get; set; }
}
