
namespace NetApp.Api.Middlewares;
public class CancellationTokenMiddleware
{
private readonly RequestDelegate _next;

    public CancellationTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        CancellationToken cancellationToken = context.RequestAborted;

        // Check if a cancellation token is provided in the request headers
        if (context.Request.Headers.TryGetValue("X-Cancellation-Token", out var tokenHeaderValue))
        {
            
            // if (CancellationToken.TryParse(tokenHeaderValue, out var customCancellationToken))
            // {
            //     cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, customCancellationToken).Token;
            // }
        }

        context.RequestAborted = cancellationToken;

        await _next(context);
    }
}
