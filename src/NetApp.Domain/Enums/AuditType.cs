namespace NetApp.Domain.Enums;

[Flags]
public enum AuditType : ushort
{
    Create = 1,
    Update = 4,
    Delete = 8
}
