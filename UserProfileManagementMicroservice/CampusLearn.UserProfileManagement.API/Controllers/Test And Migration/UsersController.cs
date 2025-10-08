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


    //http://localhost:6000/users/migration
    //route creates a new database in sqlserver
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
                return Ok(new { message = "Migrations applied successfully. To UserProfileDB" });
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
            return BadRequest(new {error = ex.Message.ToString()});
        }
    }

    //http://localhost:6000/users/
    //route is for testing if api works
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

}
