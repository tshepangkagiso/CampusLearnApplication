namespace CampusLearn.TopicsManagement.API.Controllers.QueryResponses;

[Route("[controller]")]
[ApiController]
public class QueryResponsesController(TopicsDbContext context, MinioService minio) : ControllerBase
{
}
