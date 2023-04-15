using Microsoft.EntityFrameworkCore;
using NetApp.Application.Common;
using NetApp.Application.Interfaces.Identity;
using NetApp.Domain.Common;
using NetApp.Domain.Repositories;

namespace NetApp.Infrastructure.Contexts;

public class NetAppDbContext : AuditableContext, INetAppDbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeService _dateTimeService;

    public NetAppDbContext(DbContextOptions<NetAppDbContext> options, ICurrentUserService currentUserService, IDateTimeService dateTimeService) : base(options)
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
        foreach (var entry in ChangeTracker.Entries<IEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = _currentUserService.UserId;
                    entry.Entity.CreatedOn= _dateTimeService.Now;
                    break;
                case EntityState.Modified when entry.Entity is IAuditableEntity auditableEntity:
                    auditableEntity.LastModifiedBy = _currentUserService.UserId;
                    auditableEntity.LastModifiedOn = _dateTimeService.Now;
                    break;
            }
        }
        await base.SaveChangesAsync(_currentUserService.UserId, cancellationToken);
    }
}
