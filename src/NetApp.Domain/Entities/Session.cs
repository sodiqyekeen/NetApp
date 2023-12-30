using NetApp.Domain.Common;
using NetApp.Domain.Enums;

namespace NetApp.Domain.Entities;

public class Session : IEntity<Guid>
{
    public Guid Id { get; set; }
    public SessionCategory Category { get; set; }
    public DateTime ValidUntil { get; set; }
    public string? SigningKey { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public string IpAddress { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
}
