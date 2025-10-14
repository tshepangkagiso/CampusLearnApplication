using CampusLearn.Code.Library.RabbitMQ.TopicRabbitMQ;
using CampusLearn.TopicsManagement.API.Controllers.QueryTopics.DTOs;


namespace CampusLearn.TopicsManagement.API.Controllers.QueryTopics;

[Route("[controller]")]
[ApiController]
public class QueryTopicsController(TopicsDbContext context, RabbitMqPublisher publisher) : ControllerBase
{
    [HttpPost("queries/{tutorId}")]
    public async Task<IActionResult> CreateQuery([FromBody] CreateQueryRequest request, [FromRoute] int tutorId)
    {
        try
        {
            // 1. Create query in local database
            var query = new QueryTopic
            {
                QueryTopicTitle = request.Title,
                QueryTopicDescription = request.Description,
                RelatedModuleCode = request.ModuleCode,
                StudentID = request.StudentId,
                TopicCreationDate = DateTime.UtcNow
            };

            await context.QueryTopics.AddAsync(query);
            await context.SaveChangesAsync();

            // incase there is no tutor
            if (tutorId > 0)
            {
                query.AssignedTutorID = tutorId;
                query.IsAssigned = true;
                query.AssignedAt = DateTime.UtcNow;
                await context.SaveChangesAsync();

                var message = new NewTopicMessage(
                    Title: query.QueryTopicTitle,
                    ModuleCode: query.RelatedModuleCode,
                    QueryId: query.QueryTopicID,
                    StudentId: query.StudentID,
                    TutorId: tutorId,
                    CreatedAt: DateTime.UtcNow
                );

                // 3. Publish event to RabbitMQ ONLY if tutor was found
                var response = await publisher.Publish<NewTopicMessage>(message);
                return Ok(new
                {
                    Query = query,
                    TutorAssigned = tutorId,
                    Response = response
                });
            }

            return BadRequest("Failed to make topic query");
        }
        catch (Exception ex)
        {
            Log.Error("Failed to make topic query", ex);
            return BadRequest("Failed to make topic query");
        }
    }

    // Get all query topics
    [HttpGet("queries")]
    public async Task<IActionResult> GetAllQueryTopics()
    {
        try
        {
            var queries = await context.QueryTopics
                .Include(q => q.Responses)
                .OrderByDescending(q => q.TopicCreationDate)
                .ToListAsync();

            return Ok(queries);
        }
        catch (Exception ex)
        {
            Log.Error("Failed to get query topics", ex);
            return BadRequest("Failed to get query topics");
        }
    }

    // Get query topic by ID
    [HttpGet("queries/{queryId}")]
    public async Task<IActionResult> GetQueryTopicById([FromRoute] int queryId)
    {
        try
        {
            var query = await context.QueryTopics
                .Include(q => q.Responses)
                .FirstOrDefaultAsync(q => q.QueryTopicID == queryId);

            if (query == null)
                return NotFound("Query topic not found");

            return Ok(query);
        }
        catch (Exception ex)
        {
            Log.Error("Failed to get query topic", ex);
            return BadRequest("Failed to get query topic");
        }
    }

    // Get all query topics by student ID
    [HttpGet("students/{studentId}/queries")]
    public async Task<IActionResult> GetQueryTopicsByStudentId([FromRoute] int studentId)
    {
        try
        {
            var queries = await context.QueryTopics
                .Where(q => q.StudentID == studentId)
                .Include(q => q.Responses)
                .OrderByDescending(q => q.TopicCreationDate)
                .ToListAsync();

            return Ok(new
            {
                StudentId = studentId,
                TotalQueries = queries.Count,
                Queries = queries
            });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to get student query topics", ex);
            return BadRequest("Failed to get student query topics");
        }
    }

    // Get all query topics by tutor ID
    [HttpGet("tutors/{tutorId}/queries")]
    public async Task<IActionResult> GetQueryTopicsByTutorId([FromRoute] int tutorId)
    {
        try
        {
            var queries = await context.QueryTopics
                .Where(q => q.AssignedTutorID == tutorId)
                .Include(q => q.Responses)
                .OrderByDescending(q => q.TopicCreationDate)
                .ToListAsync();

            return Ok(new
            {
                TutorId = tutorId,
                TotalQueries = queries.Count,
                Queries = queries
            });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to get tutor query topics", ex);
            return BadRequest("Failed to get tutor query topics");
        }
    }

    // Update query topic
    [HttpPut("queries/{queryId}")]
    public async Task<IActionResult> UpdateQueryTopic([FromRoute] int queryId, [FromBody] UpdateQueryRequest request)
    {
        try
        {
            var query = await context.QueryTopics.FindAsync(queryId);
            if (query == null)
                return NotFound("Query topic not found");

            query.QueryTopicTitle = request.Title ?? query.QueryTopicTitle;
            query.QueryTopicDescription = request.Description ?? query.QueryTopicDescription;
            query.IsResolved = request.IsResolved;
            query.IsUrgent = request.IsUrgent;
            query.LastActivity = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return Ok(query);
        }
        catch (Exception ex)
        {
            Log.Error("Failed to update query topic", ex);
            return BadRequest("Failed to update query topic");
        }
    }

    // Delete query topic
    [HttpDelete("queries/{queryId}")]
    public async Task<IActionResult> DeleteQueryTopic([FromRoute] int queryId)
    {
        try
        {
            var query = await context.QueryTopics.FindAsync(queryId);
            if (query == null)
                return NotFound("Query topic not found");

            context.QueryTopics.Remove(query);
            await context.SaveChangesAsync();

            return Ok(new { Message = "Query topic deleted successfully" });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to delete query topic", ex);
            return BadRequest("Failed to delete query topic");
        }
    }
}
