using Microsoft.AspNetCore.Mvc;
using FileUpload.Contracts;
using MassTransit;

namespace FileUpload.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IWebHostEnvironment _environment;

    public FileUploadController(IPublishEndpoint publishEndpoint, IWebHostEnvironment environment)
    {
        _publishEndpoint = publishEndpoint;
        _environment = environment;
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var fileId = Guid.NewGuid();
        var uploadsFolder = Path.Combine(_environment.ContentRootPath, "uploads");
        
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, $"{fileId}_{file.FileName}");
        
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileUploaded = new FileUploaded
        {
            FileId = fileId,
            FileName = file.FileName,
            FilePath = filePath,
            UploadedAt = DateTime.UtcNow,
            UserId = User.Identity?.Name ?? "anonymous" // In a real app, get the actual user ID
        };

        await _publishEndpoint.Publish(fileUploaded);

        return Ok(new { FileId = fileId, FileName = file.FileName });
    }
} 