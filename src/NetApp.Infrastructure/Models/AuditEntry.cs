using NetApp.Domain.Enums;
using NetApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace NetApp.Infrastructure.Models;

public class AuditEntry
{
    public AuditEntry(EntityEntry entry) => Entry = entry;

    public EntityEntry Entry { get; }
    public string? UserId { get; set; }
    public string TableName { get; set; } = null!;
    public Dictionary<string, object> KeyValues { get; } = new();
    public Dictionary<string, object> OldValues { get; } = new();
    public Dictionary<string, object> NewValues { get; } = new();
    public List<PropertyEntry> TemporaryProperties { get; } = new();
    public AuditType AuditType { get; set; }
    public List<string> ChangedColumns { get; } = new();
    public bool HasTemporaryProperties => TemporaryProperties.Any();

    public Audit ToAudit() =>
    new Audit
    {
        UserId = UserId,
        Type = AuditType.ToString(),
        TableName = TableName,
        DateTime = DateTime.UtcNow,
        PrimaryKey = KeyValues.ToJson(),
        OldValues = OldValues.Count == 0 ? null : OldValues.ToJson(),
        NewValues = NewValues.Count == 0 ? null : NewValues.ToJson(),
        AffectedColumns = ChangedColumns.Count == 0 ? null : ChangedColumns.ToJson()
    };
}