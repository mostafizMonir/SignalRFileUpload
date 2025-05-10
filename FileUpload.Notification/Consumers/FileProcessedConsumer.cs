using FileUpload.Contracts;
using FileUpload.Notification.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace FileUpload.Notification.Consumers;

public class FileProcessedConsumer : IConsumer<FileProcessed>
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<FileProcessedConsumer> _logger;

    public FileProcessedConsumer(IHubContext<NotificationHub> hubContext, ILogger<FileProcessedConsumer> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<FileProcessed> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received FileProcessed event for file: {FileName}", message.FileName);

        try
        {
            await _hubContext.Clients.Group(message.UserId)
                .SendAsync("FileProcessed", new
                {
                    message.FileId,
                    message.FileName,
                    message.Success,
                    message.Message,
                    message.ProcessedAt
                });

            _logger.LogInformation("Notification sent to user {UserId} for file {FileName}", 
                message.UserId, message.FileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to user {UserId} for file {FileName}", 
                message.UserId, message.FileName);
        }
    }
} 