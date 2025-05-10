using Microsoft.AspNetCore.SignalR;

namespace FileUpload.Notification.Hubs;

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, Context.User?.Identity?.Name ?? "anonymous");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.User?.Identity?.Name ?? "anonymous");
        await base.OnDisconnectedAsync(exception);
    }
} 