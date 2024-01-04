using Microsoft.AspNetCore.SignalR;

namespace NetApp.Infrastructure.Hubs;

public class NetAppHub : Hub
{

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync(SharedConstants.SignalR.OnConnected, Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public async Task NotifyRolesUpdated()
    {
        await Clients.Others.SendAsync(SharedConstants.SignalR.OnRolesUpdated);
    }
}