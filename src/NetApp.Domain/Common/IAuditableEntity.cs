namespace NetApp.Domain.Common;

public interface IAuditableEntity<TId> : IAuditableEntity, IEntity<TId>
{

}

public interface IAuditableEntity : IEntity
{
    string? LastModifiedBy { get; set; }
    DateTime? LastModifiedOn { get; set; }

}
