namespace NetApp.Models;
public class PagingOptions
{
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 10;
    private const int DefaultPageIndex = 1;

    private int _pageSize = DefaultPageSize;
    private int _pageIndex = 1;

    public int? PageIndex { get => _pageIndex; set => _pageIndex = value ?? DefaultPageIndex; }
    public int? PageSize { get => _pageSize; set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value ?? DefaultPageSize; }
}
