using Microsoft.AspNetCore.SignalR;

namespace Order.Notifications.Hubs;

public class OrderNotificationHub:Hub
{
    public async Task SendOrderNotification(DummyOrder order)
    {
        await Clients.All.SendAsync("OrderStatusUpdated", order);
    }
}
