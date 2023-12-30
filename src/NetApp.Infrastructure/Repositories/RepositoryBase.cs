using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NetApp.Infrastructure.Contexts;
using System.Linq.Expressions;

namespace NetApp.Infrastructure.Repositories;


public abstract class RepositoryBase<T>(NetAppDbContext dbContext) where T : class
{
    protected readonly NetAppDbContext _dbContext = dbContext;

    protected async Task AddAsync(T entity, CancellationToken cancellationToken = default) => await _dbContext.Set<T>().AddAsync(entity, cancellationToken);

    protected async Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default) => await _dbContext.Set<T>().FindAsync([id], cancellationToken: cancellationToken);

    protected IQueryable<T> GetAll() => _dbContext.Set<T>();

    protected void Update(T entity) => _dbContext.Set<T>().Update(entity);

    protected void Delete(T entity) => _dbContext.Set<T>().Remove(entity);

    protected async Task AddBulkAsync(IEnumerable<T> entities) => await _dbContext.Set<T>().AddRangeAsync(entities);

    protected async Task UpdateBulkAsync(Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> expression, CancellationToken cancellationToken = default) =>
        await _dbContext.Set<T>().ExecuteUpdateAsync(expression, cancellationToken);

    protected async Task DeleteBulkAsync(IQueryable<T> query, CancellationToken cancellationToken = default) =>
        await query.ExecuteDeleteAsync(cancellationToken);

}
