using Microsoft.Extensions.Localization;
using NetApp.Shared;
using System.Net;
using System.Net.Http.Headers;

namespace NetApp.UI.Infrastructure;
public class AuthenticationHeaderHandler : DelegatingHandler
{
    private readonly IStorageService _storageService;
    private readonly ISnackbar _snackbar;
    private readonly IStringLocalizer<NetAppLocalizer> _localizer;
    public AuthenticationHeaderHandler(IStorageService storageService, ISnackbar snackbar, IStringLocalizer<NetAppLocalizer> localizer)
    {
        _storageService = storageService;
        _snackbar = snackbar;
        _localizer = localizer;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization?.Scheme != "Bearer")
        {
            var savedToken = await _storageService.GetItemAsync<string>(ApplicationConstants.Storage.AuthToken);

            if (!string.IsNullOrWhiteSpace(savedToken))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
        }

        try
        {
            return await base.SendAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine(ex.ToString());
#endif
            _snackbar.Add(_localizer[ApplicationConstants.ErrorMessages.ServerError], Severity.Error);
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }
}
