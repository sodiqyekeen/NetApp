namespace NetApp.Models;

public class PaginatedResponse<T> 
{
    public int Count { get; set; }
    public int PageSize { get; set; }
    public int PageIndex { get; set; }
    public IEnumerable<T> Data { get; set; } = new List<T>();

}
