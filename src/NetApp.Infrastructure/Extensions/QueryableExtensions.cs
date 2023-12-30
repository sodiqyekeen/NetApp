using Microsoft.EntityFrameworkCore;

namespace NetApp.Infrastructure;
public static class QueryableExtensions
{
    //public static async Task<PaginatedResponse<TDto>> ToPaginatedResponse<T, TDto>(this IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    //{
    //    CheckPageNumberAndSize(ref pageNumber, ref pageSize);
    //    var totalItems = await source.CountAsync(cancellationToken);
    //    if (totalItems == 0) return new PaginatedResponse<TDto>(Enumerable.Empty<TDto>(), 0, 0, 0);

    //    //var config = new MapperConfiguration(cfg => cfg.CreateMap<T, TDto>());
    //    var mapper = config.CreateMapper();
    //    var items = await source.Skip((pageNumber -1) * pageSize).Take(pageSize).ProjectTo<TDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);

    //    return new PaginatedResponse<TDto>(items, totalItems, pageSize, pageNumber);
    //}

    public static async Task<PaginatedResponse<T>> ToPaginatedResponseAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        CheckPageNumberAndSize(ref pageNumber, ref pageSize);
        var totalItems = await source.CountAsync(cancellationToken);
        if (totalItems == 0) return new PaginatedResponse<T>(Enumerable.Empty<T>(), 0, 0, 0);

        var items = await source.Skip((pageNumber -1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PaginatedResponse<T>(items, totalItems, pageSize, pageNumber);
    }


    private static void CheckPageNumberAndSize(ref int pageNumber, ref int pageSize)
    {
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;
        if (pageSize > 100) pageSize = 100;
    }



}
