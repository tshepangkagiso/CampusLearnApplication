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
                //var responseNotifications = await publisher.PublishToNotifications<NewTopicMessage>(message);
                //var responsePrivateMessages = await publisher.PublishToPrivateMessages<NewTopicMessage>(message);
                var response = await publisher.Publish<NewTopicMessage>(message);
                return Ok(new
                {
                    Query = query,
                    TutorAssigned = tutorId,
                    Response = response
                    //Notifications = responseNotifications,
                    //PrivateMessages = responsePrivateMessages
                });
            }

            return BadRequest("Failed to make topic query");
        }
        catch(Exception ex)
        {
            Log.Error("Failed to make topic query", ex);
            return BadRequest("Failed to make topic query");
        }
    }
}
