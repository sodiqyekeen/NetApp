using Microsoft.JSInterop;


namespace NetApp.UI.Infrastructure.Services;
internal class LocalStorageService : IStorageService
{
    private const string Prefix = "NetApp.";
    private readonly IJSRuntime _jsRuntime;
    public LocalStorageService(IJSRuntime jsRuntime) =>
        _jsRuntime = jsRuntime;

    public async Task ClearAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.clear");
    }

    public async Task<T?> GetItemAsync<T>(string key)
    {
        var data = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
        return data == null ? default : data.FromJson<T>();
    }

    public async Task RemoveItemAsync(string key) =>
     await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", GetKey(key));

    public async Task SetItemAsync<T>(string key, T value) =>
     await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value.ToJson());

    private static string GetKey(string key) => $"{Prefix}{key}";
}
