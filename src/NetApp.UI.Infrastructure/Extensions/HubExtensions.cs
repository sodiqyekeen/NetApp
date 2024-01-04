
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace NetApp.UI.Infrastructure.Extensions;
public static class HubExtensions
{
    public static HubConnection TryInitialize(this HubConnection? hubConnection, IConfiguration configuration, IStorageService storage)
    {
        if (hubConnection is null)
        {
            var url = configuration["ApiUrl"] + SharedConstants.SignalR.HubUrl;
            hubConnection = new HubConnectionBuilder()
                .WithUrl(new Uri(url), option => option.AccessTokenProvider = async () => await storage.GetItemAsync<string>(ApplicationConstants.Storage.AuthToken))
                .WithAutomaticReconnect()
                .Build();
        }
        return hubConnection;
    }

    public static async Task<bool> ConnectWithRetryAsync(this HubConnection hubConnection, CancellationToken cancellationToken)
    {
        while (true)
        {
            try
            {
                await hubConnection.StartAsync(cancellationToken);
                return true;
            }
            catch when (cancellationToken.IsCancellationRequested)
            {
                return false;
            }
            catch (Exception)
            {
                await Task.Delay(5000, cancellationToken);
            }
        }
    }

    public static async ValueTask InvokeHubMethodAsync(this HubConnection hubConnection, string methodName)
    {
        if (hubConnection?.State is HubConnectionState.Connected)
        {
            await hubConnection.SendAsync(methodName);
        }

    }
}
