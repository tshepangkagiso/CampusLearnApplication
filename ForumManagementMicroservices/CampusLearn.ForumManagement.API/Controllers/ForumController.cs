using CampusLearn.Code.Library.RabbitMQ.ForumRabbitMQ;
using CampusLearn.ForumManagement.API.RabbitMQ;

namespace CampusLearn.ForumManagement.API.Controllers;

[Route("[controller]")]
[ApiController]

public class ForumController(ForumDbContext context,MinioService minio, RabbitMqPublisher publisher) : ControllerBase
{
    [HttpPost("rabbitmq-publish")]
    public async Task<IActionResult> OnPublish([FromBody] NewForumMessage message)
    {
        try
        {
            if (message == null) return BadRequest("Empty message");
            var response = await publisher.Publish(message);
            return Ok(new { message = message, response = response });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to publish");
            return BadRequest($"Failed to publish: {ex.Message}");
        }
    }

    [HttpGet("buckets")]
    public async Task<IActionResult> GetBuckets()
    {
        try
        {
            var buckets = await minio.ListBucketsAsync();
            return Ok(buckets);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to get buckets");
            return BadRequest($"Failed to get buckets: {ex.Message}");
        }
    }


    //serves as example on how to upload any media, for testing only
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        try
        {
            Console.WriteLine($"Upload request received. File: {file?.FileName}, Size: {file?.Length}");

            if (file == null || file.Length == 0)
            {
                Console.WriteLine("No file or empty file provided");
                return BadRequest("No file provided");
            }

            // Validate file size
            if (file.Length > 100 * 1024 * 1024) // 100MB limit
            {
                Console.WriteLine($"File size {file.Length} exceeds limit");
                return BadRequest("File size exceeds limit");
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            Console.WriteLine($"Generated filename: {uniqueFileName}");

            using Stream data = file.OpenReadStream();
            Console.WriteLine($"Stream created. CanRead: {data.CanRead}, Length: {data.Length}");

            //await minio.UploadStreamAsync(uniqueFileName , data);

            await minio.UploadFileAsync(uniqueFileName, data, file.ContentType);
            Console.WriteLine("File uploaded successfully");
            return Ok(new
            {
                Message = "File successfully uploaded",
                FileName = uniqueFileName,
                OriginalName = file.FileName,
                Size = file.Length
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Controller exception: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            return BadRequest($"Failed to upload file: {ex.Message}");
        }
    }

}
