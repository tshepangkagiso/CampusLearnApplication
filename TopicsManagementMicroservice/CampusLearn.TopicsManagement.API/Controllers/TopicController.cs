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

}
