using Microsoft.AspNetCore.Identity;
using NetApp.Domain.Common;

namespace NetApp.Infrastructure.Identity.Models;

public class NetAppUser : IdentityUser, IAuditableEntity
{
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public string? LastModifiedBy { get; set; }
    public bool Active { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
