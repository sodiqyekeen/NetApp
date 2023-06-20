using System.Net.Http.Headers;

namespace NetApp.UI.Infrastructure;
public class AuthenticationHeaderHandler : DelegatingHandler
{
    private readonly IStorageService _storageService;
    public AuthenticationHeaderHandler(IStorageService storageService) =>
    _storageService = storageService;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization?.Scheme != "Bearer")
        {
            var savedToken = await _storageService.GetItemAsync<string>(ApplicationConstants.Storage.AuthToken);

            if (!string.IsNullOrWhiteSpace(savedToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
