namespace CampusLearn.TopicsManagement.API.Controllers;

[Route("[controller]")]
[ApiController]
public class TopicController(TopicsDbContext context,ITopicMessagePublisher publisher, MinioService minio) : ControllerBase
{

    [HttpGet("migration")]
    public IActionResult Get()
    {
        try
        {
            // Check if database exists
            if (context.Database.CanConnect())
            {
                Console.WriteLine("Database exists. Applying migrations...");
                context.Database.Migrate();
                Console.WriteLine("Migrations applied successfully.");
                return Ok(new { message = "Migrations applied successfully. To TopicsDB" });
            }
            else
            {
                Console.WriteLine("Database does not exist. Creating database and applying migrations...");
                context.Database.Migrate();
                Console.WriteLine("Database created and migrations applied successfully.");
                return BadRequest();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message.ToString(), "An error occurred while migrating the database.");
            return BadRequest(new { error = ex.Message.ToString() });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetResponse()
    {
        try
        {
            NewTopicMessage message = new NewTopicMessage("Topic", "First Attempt From Topic");
            await publisher.PublishNewTopicMessageAsync(message);

            return Ok("Hello, This is the Topics API.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message.ToString()}");
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


    //Retrieving file sent
    [HttpGet("file/{fileName}")]
    public async Task<IActionResult> GetFile(string fileName)
    {
        try
        {
            var (data, contentType) = await minio.OnRetrieveFile(fileName);

            // Return file with correct content type for display
            return File(data, contentType);
        }
        catch (Exception ex)
        {
            return NotFound($"File not found: {ex.Message}");
        }
    }
}
