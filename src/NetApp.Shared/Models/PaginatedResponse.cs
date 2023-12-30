namespace NetApp.Models;

public class PaginatedResponse<T>
{
    public PaginatedResponse()
    {
        Data = Enumerable.Empty<T>();
    }

    public PaginatedResponse(IEnumerable<T> data, int totalItems, int pageSize, int pageNumber)
    {
        TotalItems = totalItems;
        PageSize = pageSize;
        PageNumber = pageNumber;
        Data = data?? Enumerable.Empty<T>();
        if (PageSize > 0)
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);
    }

    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public IEnumerable<T> Data { get; set; }

}
