namespace CampusLearn.UserProfileManagement.API.Controllers;

[Route("[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserManagementDbContext context;
    public UsersController(UserManagementDbContext context)
    {
        this.context = context;
    }

    [HttpGet("migration")]
    public async Task<IActionResult> Get()
    {
        try
        {
            // Check if database exists
            if (this.context.Database.CanConnect())
            {
                Console.WriteLine("Database exists. Applying migrations...");
                await this.context.Database.MigrateAsync();
                Console.WriteLine("Migrations applied successfully.");
                return Ok();
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
            return BadRequest();
        }
    }

    [HttpGet]
    public IActionResult GetResponse()
    {
        try
        {
            return Ok("Hello, This is the users API.");
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
