namespace NetApp.Domain.Common;

public abstract class EntityBase : IEntity
{
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
}

public abstract class AuditableEntity<TId> : IAuditableEntity<TId>
{
    public abstract TId Id { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}