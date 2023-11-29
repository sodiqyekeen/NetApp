namespace NetApp.Models;
public class PagingOptions
{
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 10;

    private int _pageSize = DefaultPageSize;

    public int? PageIndex { get; set; } = 1;
    public int? PageSize { get => _pageSize; set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value ?? DefaultPageSize; }
}
