using FileUpload.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FileUpload.Worker.Consumers;

public class FileUploadedConsumer : IConsumer<FileUploaded>
{
    private readonly ILogger<FileUploadedConsumer> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public FileUploadedConsumer(ILogger<FileUploadedConsumer> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<FileUploaded> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing file: {FileName}", message.FileName);

        // Simulate file processing
        await Task.Delay(2000); // Simulate some processing time

        // Publish file processed event
        await _publishEndpoint.Publish(new FileProcessed
        {
            FileId = message.FileId,
            FileName = message.FileName,
            ProcessedAt = DateTime.UtcNow,
            Success = true
        });

        _logger.LogInformation("File processed successfully: {FileName}", message.FileName);
    }
} 