namespace NetApp.Models;

public class PaginatedResponse<T>
{
    public PaginatedResponse(IEnumerable<T> data, int totalItems, int pageSize, int pageNumber)
    {
        TotalItems = totalItems;
        PageSize = pageSize;
        PageNumber = pageNumber;
        Data = data?? new List<T>();
        if (PageSize > 0)
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);
    }

    public int TotalItems { get; }
    public int TotalPages { get; }
    public int PageSize { get; }
    public int PageNumber { get; }
    public IEnumerable<T> Data { get; } = new List<T>();

}
