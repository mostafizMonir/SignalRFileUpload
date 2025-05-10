using Microsoft.AspNetCore.SignalR;
using Serilog;
using Microsoft.Extensions.Logging;

namespace FileUpload.Notification.Hubs;

public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, Context.User?.Identity?.Name ?? "anonymous");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        if (exception != null)
        {
            _logger.LogError(exception, "Client disconnected with error: {ConnectionId}", Context.ConnectionId);
        }
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.User?.Identity?.Name ?? "anonymous");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendNotification(string message)
    {
        _logger.LogInformation("Sending notification to all clients: {Message}", message);
        await Clients.All.SendAsync("ReceiveNotification", message);
    }
} 