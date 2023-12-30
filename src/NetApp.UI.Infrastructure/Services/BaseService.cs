using Microsoft.Extensions.Localization;
using NetApp.Shared;
using System.Net.Http.Json;

namespace NetApp.UI.Infrastructure.Services;
public abstract class BaseService
{
    protected readonly HttpClient _httpClient;
    private readonly ISnackbar _snackbar;
    private readonly IStringLocalizer<NetAppLocalizer> _localizer;
    public BaseService(HttpClient httpClient, ISnackbar snackbar, IStringLocalizer<NetAppLocalizer> localizer)
    {
        _httpClient = httpClient;
        _snackbar=snackbar;
        _localizer = localizer;
    }

    public async Task<TResponse> GetAsync<TResponse>(string uri, CancellationToken cancellation = default)
    {
        try
        {
            var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), cancellation);
            if (response.IsSuccessStatusCode)
            {
                return (await response.Content.ReadFromJsonAsync<Response<TResponse>>(cancellationToken: cancellation))!.Data!;
            }
            _snackbar.Add(_localizer["An error occurred."], Severity.Error);
            return typeof(TResponse).IsGenericType ? Activator.CreateInstance<TResponse>()! : default!;
        }
        catch (Exception ex)
        {
            _snackbar.Add(_localizer["An error occurred."], Severity.Error);
#if DEBUG
            Console.WriteLine(ex.ToString());
#endif
            return typeof(TResponse).IsGenericType ? Activator.CreateInstance<TResponse>()! : default!;
        }
    }

    public async Task<Response<TResponse>> PostAsync<TRequest, TResponse>(string uri, TRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(uri, request);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<Response<TResponse>>())!;
            var failedResponse = await response.Content.ReadFromJsonAsync<Response>();
            return Response<TResponse>.Fail(failedResponse!.Message);
        }
        catch (TaskCanceledException)
        {
            return Response<TResponse>.Fail("Timeout");
        }
    }
    public async Task<IResponse> PostAsync<TRequest>(string uri, TRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(uri, request);
            return response.IsSuccessStatusCode ? Response.Success() : (IResponse)(await response.Content.ReadFromJsonAsync<Response>())!;
        }
        catch (TaskCanceledException)
        {
            return Response.Fail("Timeout");
        }
    }

    public async Task<IResponse> PostAsync(string uri)
    {
        try
        {
            var response = await _httpClient.PostAsync(uri, null);
            return (await response.Content.ReadFromJsonAsync<Response>())!;
        }
        catch (TaskCanceledException)
        {
            return Response.Fail("Timeout");
        }
    }

    public async Task<IResponse> PutAsync<TRequest>(string uri, TRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(uri, request);
            return (await response.Content.ReadFromJsonAsync<Response>())!;
        }
        catch (TaskCanceledException)
        {
            return Response.Fail("Timeout");
        }
    }

    public async Task<IResponse> DeleteAsync(string uri)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(uri);
            return response.IsSuccessStatusCode ? Response.Success() : (await response.Content.ReadFromJsonAsync<IResponse>())!;
        }
        catch (TaskCanceledException)
        {
            return Response.Fail("Timeout");
        }
    }
}
