using Microsoft.AspNetCore.Identity;
using NetApp.Domain.Common;
using NetApp.Domain.Entities;

namespace NetApp.Infrastructure.Identity.Models;

public class NetAppUser : IdentityUser<string>, IAuditableEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? OtherNames { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public string? LastModifiedBy { get; set; }
    public bool Active { get; set; }
    public DateTime? LastLoginOn { get; set; }

    public ICollection<Session> Sessions { get; set; } = new HashSet<Session>();
    public ICollection<IdentityUserRole<string>> UserRoles { get; set; } = new HashSet<IdentityUserRole<string>>();
}