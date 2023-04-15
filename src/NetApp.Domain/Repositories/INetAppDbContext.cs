namespace NetApp.Domain.Repositories;

public interface INetAppDbContext
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
