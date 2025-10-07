namespace CampusLearn.TopicsManagement.API.Controllers;

[Route("[controller]")]
[ApiController]
public class TopicController : ControllerBase
{
    private readonly TopicsDbContext context;
    private readonly ITopicMessagePublisher publisher;
    public TopicController(TopicsDbContext context, ITopicMessagePublisher publisher)
    {
        this.context = context;
        this.publisher = publisher;
    }

    [HttpGet("migration")]
    public IActionResult Get()
    {
        try
        {
            // Check if database exists
            if (this.context.Database.CanConnect())
            {
                Console.WriteLine("Database exists. Applying migrations...");
                this.context.Database.Migrate();
                Console.WriteLine("Migrations applied successfully.");
                return Ok(new { message = "Migrations applied successfully. To TopicsDB" });
            }
            else
            {
                Console.WriteLine("Database does not exist. Creating database and applying migrations...");
                this.context.Database.Migrate();
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
            await this.publisher.PublishNewTopicMessageAsync(message);

            return Ok("Hello, This is the Topics API.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message.ToString()}");
        }
    }

    [HttpGet("health/database")]
    public async Task<IActionResult> CheckDatabaseHealth()
    {
        try
        {
            // Check if database is reachable
            var canConnect = await context.Database.CanConnectAsync();

            if (!canConnect)
            {
                return StatusCode(503, new
                {
                    status = "Unhealthy",
                    message = "Database is not reachable",
                    timestamp = DateTime.UtcNow
                });
            }

            return Ok(new
            {
                status = "Healthy",
                message = "Database is responding correctly",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(503, new
            {
                status = "Unhealthy",
                message = "Database health check failed",
                error = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
