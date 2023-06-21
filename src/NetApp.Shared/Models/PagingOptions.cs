namespace NetApp.Models;
public class PagingOptions//(int PageSize, int PageIndex=1)
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;
    public int PageIndex { get; set; } = 1;
    public int PageSize { get => _pageSize; set => _pageSize = value > MaxPageSize ? MaxPageSize : value; }
}
