using Microsoft.AspNetCore.Identity;
using NetApp.Domain.Common;

namespace NetApp.Infrastructure.Identity.Models;

public class NetAppRoleClaim : IdentityRoleClaim<string>, IEntity<int>
{
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public NetAppRole? Role { get; set; }
    public string Group { get; set; } = null!;
    public string Description { get; set; } = null!;
}
