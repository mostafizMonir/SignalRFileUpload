using MassTransit;
using FileUpload.Contracts;

namespace FileUpload.Worker;

public class FileUploadedConsumer : IConsumer<FileUploaded>
{
    private readonly ILogger<FileUploadedConsumer> _logger;

    public FileUploadedConsumer(ILogger<FileUploadedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<FileUploaded> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing file: {FileName}", message.FileName);

        try
        {
            // Simulate file processing
            await Task.Delay(5000); // Simulate work for 5 seconds

            var processedFilePath = message.FilePath + "_processed";
            await File.WriteAllTextAsync(processedFilePath, "Processed content");

            await context.Publish(new FileProcessed
            {
                FileId = message.FileId,
                FileName = message.FileName,
                ProcessedFilePath = processedFilePath,
                ProcessedAt = DateTime.UtcNow,
                UserId = message.UserId,
                Success = true,
                Message = "File processed successfully"
            });

            _logger.LogInformation("File processed successfully: {FileName}", message.FileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing file: {FileName}", message.FileName);
            
            await context.Publish(new FileProcessed
            {
                FileId = message.FileId,
                FileName = message.FileName,
                ProcessedAt = DateTime.UtcNow,
                UserId = message.UserId,
                Success = false,
                Message = $"Error processing file: {ex.Message}"
            });
        }
    }
}
