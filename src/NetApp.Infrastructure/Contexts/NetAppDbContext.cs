using Microsoft.EntityFrameworkCore;
using NetApp.Domain.Common;
using NetApp.Domain.Repositories;

namespace NetApp.Infrastructure.Contexts;

public class NetAppDbContext : AuditableContext, INetAppDbContext
{
    private readonly ISessionService _currentUserService;
    private readonly IDateTimeService _dateTimeService;

    public NetAppDbContext(DbContextOptions<NetAppDbContext> options, ISessionService currentUserService, IDateTimeService dateTimeService) : base(options)
    {
        _currentUserService = currentUserService;
        _dateTimeService = dateTimeService;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

    }

    async Task INetAppDbContext.SaveChangesAsync(CancellationToken cancellationToken)
    {
        string userId = string.IsNullOrWhiteSpace(_currentUserService.UserId) ? "System" : _currentUserService.UserId;

        foreach (var entry in ChangeTracker.Entries<IEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy ??= userId;
                    entry.Entity.CreatedOn = _dateTimeService.Now;
                    break;
                case EntityState.Modified when entry.Entity is IAuditableEntity auditableEntity:
                    auditableEntity.LastModifiedBy = _currentUserService.UserId;
                    auditableEntity.LastModifiedOn = _dateTimeService.Now;
                    break;
            }
        }
        await base.SaveChangesAsync(userId, cancellationToken);
    }
}
