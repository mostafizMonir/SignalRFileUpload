namespace FileUpload.Contracts;

public class FileProcessed
{
    public Guid FileId { get; set; }
    public string FileName { get; set; }
    public string ProcessedFilePath { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string UserId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
} 