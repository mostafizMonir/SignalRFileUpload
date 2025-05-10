namespace FileUpload.Contracts;

public class FileUploaded
{
    public Guid FileId { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public DateTime UploadedAt { get; set; }
    public string UserId { get; set; }
} 