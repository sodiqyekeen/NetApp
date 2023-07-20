using System.Net.Http.Json;

namespace NetApp.UI.Infrastructure.Services;
public abstract class BaseService
{
    protected readonly HttpClient _httpClient;
    public BaseService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TResponse> GetAsync<TResponse>(string uri, CancellationToken cancellation = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<IResponse<TResponse>>(uri, cancellation);
            return response!.Data!;
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine(ex.ToString());
#endif
            return typeof(TResponse).IsGenericType ? Activator.CreateInstance<TResponse>()! : default!;
        }
    }

    public async Task<IResponse<TResponse>> PostAsync<TRequest, TResponse>(string uri, TRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(uri, request);
        if (response.IsSuccessStatusCode)
            return (await response.Content.ReadFromJsonAsync<IResponse<TResponse>>())!;
        var failedResponse = await response.Content.ReadFromJsonAsync<IResponse>();
        return Response<TResponse>.Fail(failedResponse!.Message);
    }
    public async Task<IResponse> PostAsync<TRequest>(string uri, TRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(uri, request);
        return (await response.Content.ReadFromJsonAsync<IResponse>())!;
    }

    public async Task<IResponse> PostAsync(string uri)
    {
        var response = await _httpClient.PostAsync(uri, null);
        return (await response.Content.ReadFromJsonAsync<IResponse>())!;
    }

    public async Task<IResponse> PutAsync<TRequest>(string uri, TRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync(uri, request);
        return (await response.Content.ReadFromJsonAsync<IResponse>())!;
    }

    public async Task<IResponse> DeleteAsync(string uri)
    {
        var response = await _httpClient.DeleteAsync(uri);
        return response.IsSuccessStatusCode ? Response.Success() : (await response.Content.ReadFromJsonAsync<IResponse>())!;
    }
}
