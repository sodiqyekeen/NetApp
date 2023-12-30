using NetApp.Domain.Common;

namespace NetApp.Domain.Entities;

public class EmailTemplate : IAuditableEntity<int>
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
}
