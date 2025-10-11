namespace CampusLearn.UserProfileManagement.API.Controllers;

[Route("[controller]")]
[ApiController]
public class MigrationsController(IDataSeeder dataSeeder, UserManagementDbContext context) : ControllerBase
{
    //http://localhost:6000/migrations/modules
    //route creates a new database in sqlserver
    [HttpPost("modules")]
    public async Task<IActionResult> SeedModules()
    {
        try
        {
            await dataSeeder.SeedModulesAsync();
            return Ok(new { message = "Modules seeded successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    //http://localhost:6000/migrations/migration
    //route creates a new database in sqlserver
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
                return Ok(new { message = "Migrations applied successfully. To UserProfileDB" });
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
            return BadRequest(new {error = ex.Message.ToString()});
        }
    }
}
