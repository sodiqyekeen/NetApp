using Microsoft.AspNetCore.Http;

namespace NetApp.Models;
public class PagingOptions
{
    private const string pageIndexKey = "pageIndex";
    private const string pageSizeKey = "pageSize";

    public int PageIndex { get; set; }
    public int PageSize { get; set; }

    public static ValueTask<PagingOptions> BindAsync(HttpContext httpContext)
    {
        if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

        var result = new PagingOptions();
        if (!string.IsNullOrWhiteSpace(httpContext.Request.Query[pageIndexKey]) && int.TryParse(httpContext.Request.Query[pageIndexKey], out var parsedPageIndex))
            result.PageIndex = parsedPageIndex;
        if (!string.IsNullOrWhiteSpace(httpContext.Request.Query[pageSizeKey]) && int.TryParse(httpContext.Request.Query[pageSizeKey], out var parsedPageSize))
            result.PageSize = parsedPageSize;

        return ValueTask.FromResult(result);
    }   
}
