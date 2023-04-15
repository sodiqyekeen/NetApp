using Microsoft.AspNetCore.Identity;
using NetApp.Domain.Common;

namespace NetApp.Infrastructure.Identity.Models;

public class NetAppRole : IdentityRole, IAuditableEntity<string>
{
    public string? Description { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public ICollection<NetAppRoleClaim> RoleClaims { get; set; }

    public NetAppRole() : base()
    {
        RoleClaims = new HashSet<NetAppRoleClaim>();
    }

    public NetAppRole(string roleName, string? roleDescription = null) : base(roleName)
    {
        RoleClaims = new HashSet<NetAppRoleClaim>();
        Description = roleDescription;
    }

}
