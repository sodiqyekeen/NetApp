namespace NetApp.Domain.Common;

public interface IEntity
{
    string CreatedBy { get; set; }
    DateTime CreatedOn { get; set; }
}

public interface IEntity<TId> : IEntity
{
    TId Id { get; set; }
}
