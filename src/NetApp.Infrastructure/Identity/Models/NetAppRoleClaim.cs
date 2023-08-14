using Microsoft.AspNetCore.Identity;
using NetApp.Domain.Common;

namespace NetApp.Infrastructure.Identity.Models;

public class NetAppRoleClaim : IdentityRoleClaim<string> //, IEntity<int>
{
    public override int Id { get; set; }
    public NetAppRole? Role { get; set; }
}
