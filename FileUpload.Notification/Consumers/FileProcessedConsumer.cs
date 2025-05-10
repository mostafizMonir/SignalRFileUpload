using FileUpload.Contracts;
using FileUpload.Notification.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace FileUpload.Notification.Consumers;

public class FileProcessedConsumer : IConsumer<FileProcessed>
{
    private readonly ILogger<FileProcessedConsumer> _logger;
    private readonly IHubContext<NotificationHub> _hubContext;

    public FileProcessedConsumer(ILogger<FileProcessedConsumer> logger, IHubContext<NotificationHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<FileProcessed> context)
    {
        var message = context.Message;
        _logger.LogInformation("File processed: {FileName}", message.FileName);

        // Send notification to all connected clients
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", 
            $"File {message.FileName} has been processed successfully at {message.ProcessedAt:g}");
    }
} 