namespace CampusLearn.MediaContent.API.Controllers;

[Route("[controller]")]
[ApiController]
public class MediaController : ControllerBase
{
    private readonly MinioService minioService;

    public MediaController(MinioService minioService)
    {
        this.minioService = minioService;
    }

    [HttpGet]
    public IActionResult GetResponse()
    {
        try
        {
            return Ok("Hello, This is the media API.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message.ToString()}");
        }
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        using var stream = file.OpenReadStream();

        await minioService.UploadStreamAsync("mybucket", file.FileName, stream);

        var url = await minioService.GetFileUrlAsync("mybucket", file.FileName);

        return Ok(new { Url = url });
    }


    [HttpGet("view-bytes/{fileName}")]
    public async Task<IActionResult> ViewFileBytes(string fileName)
    {
        var url = await minioService.GetFileUrlAsync("mybucket", fileName);

        using var http = new HttpClient();
        var data = await http.GetByteArrayAsync(url);
        var contentType = GetContentType(fileName);
        return File(data, contentType);
    }

    [HttpGet("view-stream/{fileName}")]
    public async Task<IActionResult> ViewFileStream(string fileName)
    {
        var url = await minioService.GetFileUrlAsync("mybucket", fileName);

        using var http = new HttpClient();
        var stream = await http.GetStreamAsync(url);
        var contentType = GetContentType(fileName);
        return File(stream, contentType);
    }

    [HttpGet("view-direct/{fileName}")]
    public async Task<IActionResult> ViewFileDirect(string fileName)
    {
        try
        {
            var response = await minioService.GetObjectStreamAsync("mybucket", fileName);
            var contentType = GetContentType(fileName);

            return File(response.ResponseStream, contentType);
        }
        catch (AmazonS3Exception ex) when (ex.ErrorCode == "NoSuchKey")
        {
            return NotFound();
        }
    }

    [HttpDelete("delete/{fileName}")]
    public async Task<IActionResult> DeleteFile(string fileName)
    {
        try
        {
            await minioService.DeleteFileAsync("mybucket", fileName);
            return Ok(new { Message = $"File '{fileName}' deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = $"Failed to delete file: {ex.Message}" });
        }
    }
    private static string GetContentType(string fileName)
    {
        var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fileName, out var contentType))
        {
            contentType = "application/octet-stream"; // fallback
        }
        return contentType;
    }

}
