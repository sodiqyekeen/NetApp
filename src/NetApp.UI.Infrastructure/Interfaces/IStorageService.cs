﻿namespace NetApp.UI.Infrastructure;
public interface IStorageService
{
    Task SetItemAsync<T>(string key, T value);
    Task<T?> GetItemAsync<T>(string key);
    Task RemoveItemAsync(string key);
    Task ClearAsync();
}
