using NetApp.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NetApp.Domain.Enums;
using NetApp.Domain.Entities;
using NetApp.Infrastructure.Models;
using NetApp.Domain.Common;

namespace NetApp.Infrastructure.Contexts;

public abstract class AuditableContext : IdentityDbContext<NetAppUser, NetAppRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, NetAppRoleClaim, IdentityUserToken<string>>
{
    public AuditableContext()
    {

    }
    protected AuditableContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<Audit> AuditTrails => Set<Audit>();

    public virtual async Task<int> SaveChangesAsync(string userId, CancellationToken cancellationToken)
    {
        var auditEntries = OnBeforeSaveChanges(userId);
        var result = await base.SaveChangesAsync(cancellationToken);
        await OnAfterSaveChanges(auditEntries, cancellationToken);
        return result;
    }

    private List<AuditEntry> OnBeforeSaveChanges(string userId)
    {
        ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditEntry>();
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is Audit || entry.Entity is not IAuditableEntity || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry(entry)
            {
                TableName = entry.Entity.GetType().Name,
                UserId = userId
            };
            auditEntries.Add(auditEntry);
            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary)
                {
                    auditEntry.TemporaryProperties.Add(property);
                    continue;
                }

                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue!;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.AuditType = AuditType.Create;
                        auditEntry.NewValues[propertyName] = property.CurrentValue!;
                        break;

                    case EntityState.Deleted:
                        auditEntry.AuditType = AuditType.Delete;
                        auditEntry.OldValues[propertyName] = property.OriginalValue!;
                        break;

                    case EntityState.Modified when property.IsModified && property.OriginalValue != property.CurrentValue:
                        auditEntry.ChangedColumns.Add(propertyName);
                        auditEntry.AuditType = AuditType.Update;
                        auditEntry.OldValues[propertyName] = property.OriginalValue!;
                        auditEntry.NewValues[propertyName] = property.CurrentValue!;
                        break;
                }
            }
        }

        foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
        {
            AuditTrails.Add(auditEntry.ToAudit());
        }

        return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
    }

    private Task OnAfterSaveChanges(List<AuditEntry> auditEntries, CancellationToken cancellationToken = new())
    {
        if (auditEntries == null || auditEntries.Count == 0)
            return Task.CompletedTask;

        foreach (var auditEntry in auditEntries)
        {
            LoadProperties(auditEntry);
            AuditTrails?.Add(auditEntry.ToAudit());
        }
        return SaveChangesAsync(cancellationToken);
    }

    private static void LoadProperties(AuditEntry auditEntry)
    {
        foreach (var prop in auditEntry.TemporaryProperties)
        {
            if (prop.Metadata.IsPrimaryKey())
            {
                auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue!;
            }
            else
            {
                auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue!;
            }
        }
    }
}