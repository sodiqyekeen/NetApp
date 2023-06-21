using Microsoft.EntityFrameworkCore;

namespace NetApp.Infrastructure;
public static class QueryableExtensions
{
    public static async Task<PaginatedResponse<T>> ToPaginatedResponse<T>(IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedResponse<T>
        {
            Data = items,
            Count = count,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

}
