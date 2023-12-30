using Microsoft.AspNetCore.Identity;
using NetApp.Domain.Common;

namespace NetApp.Infrastructure.Identity.Models;

public class NetAppRole : IdentityRole, IAuditableEntity<string>
{
    public string Description { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public ICollection<NetAppRoleClaim> RoleClaims { get; set; } = new HashSet<NetAppRoleClaim>();
    public ICollection<IdentityUserRole<string>> UserRoless { get; set; } = new HashSet<IdentityUserRole<string>>();

    public NetAppRole() : base()
    {
    }

    public NetAppRole(string roleName, string roleDescription) : base(roleName)
    {
        Description = roleDescription;
    }

}
