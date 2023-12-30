using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetApp.Domain.Common;
using NetApp.Domain.Entities;

namespace NetApp.Infrastructure.Contexts;

public class NetAppDbContext : AuditableContext //, INetAppDbContext
{
    private readonly ISessionService _currentUserService;
    private readonly IDateTimeService _dateTimeService;

    public NetAppDbContext(DbContextOptions<NetAppDbContext> options, ISessionService currentUserService, IDateTimeService dateTimeService) : base(options)
    {
        _currentUserService = currentUserService;
        _dateTimeService = dateTimeService;
    }

    public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();
    public DbSet<Session> Sessions => Set<Session>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        BuildIdentityModel(modelBuilder);

        modelBuilder.Entity<EmailTemplate>().ToTable("EmailTemplate");
        modelBuilder.Entity<Session>().ToTable("Session");
    }

    private static void BuildIdentityModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NetAppUser>(user =>
        {
            user.ToTable("User", "Identity");
            user.Property(e => e.Id).ValueGeneratedOnAdd();
            user.HasMany(e => e.Sessions).WithOne().HasForeignKey(s => s.CreatedBy).OnDelete(DeleteBehavior.Cascade).IsRequired();
            user.HasMany(e => e.UserRoles).WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
        });

        modelBuilder.Entity<NetAppRole>(role =>
        {
            role.ToTable(name: "Role", "Identity");
            role.HasMany(e => e.UserRoless).WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
            role.HasMany(e => e.RoleClaims).WithOne(e => e.Role).HasForeignKey(rc => rc.RoleId).IsRequired();
        });
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRole", "Identity");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim", "Identity");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin", "Identity");
        modelBuilder.Entity<NetAppRoleClaim>()
            .ToTable("RoleClaim", "Identity")
            .HasOne(e => e.Role)
            .WithMany(p => p.RoleClaims)
            .HasForeignKey(e => e.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserToken", "Identity");
    }

    public async Task SaveDbChangesAsync(CancellationToken cancellationToken)
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
