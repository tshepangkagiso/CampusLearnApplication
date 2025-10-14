using CampusLearn.Code.Library.RabbitMQ.ForumRabbitMQ;
using CampusLearn.ForumManagement.API.RabbitMQ;

namespace CampusLearn.ForumManagement.API.Controllers.Forum_Topic;

[Route("[controller]")]
[ApiController]
public class ForumTopicController(ForumDbContext context, RabbitMqPublisher publisher) : ControllerBase
{
    // Create forum topic (Anyone can create)
    [HttpPost("topics")]
    public async Task<IActionResult> CreateForumTopic([FromBody] CreateForumTopicRequest request)
    {
        try
        {
            var forumTopic = new ForumTopic
            {
                ForumTopicTitle = request.Title,
                ForumTopicDescription = request.Description,
                RelatedModuleCode = request.ModuleCode,
                UserProfileID = request.UserProfileID,
                IsAnonymous = request.IsAnonymous,
                AnonymousName = request.IsAnonymous ? request.AnonymousName : null,
                TopicCreationDate = DateTime.UtcNow,
                LastActivity = DateTime.UtcNow
            };

            await context.ForumTopics.AddAsync(forumTopic);
            await context.SaveChangesAsync();

            // Publish RabbitMQ event for notifications
            await publisher.Publish<NewForumMessage>(new NewForumMessage(
                Title: forumTopic.ForumTopicTitle,
                ModuleCode: forumTopic.RelatedModuleCode
            ));

            return Ok(new
            {
                Topic = forumTopic,
                Message = "Forum topic created successfully"
            });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to create forum topic", ex);
            return BadRequest("Failed to create forum topic");
        }
    }

    // Get all forum topics
    [HttpGet("topics")]
    public async Task<IActionResult> GetAllForumTopics()
    {
        try
        {
            var topics = await context.ForumTopics
                .Include(t => t.Responses)
                .OrderByDescending(t => t.TopicCreationDate)
                .ToListAsync();

            return Ok(new
            {
                TotalTopics = topics.Count,
                Topics = topics
            });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to get forum topics", ex);
            return BadRequest("Failed to get forum topics");
        }
    }

    // Get forum topic by ID
    [HttpGet("topics/{topicId}")]
    public async Task<IActionResult> GetForumTopicById([FromRoute] int topicId)
    {
        try
        {
            var topic = await context.ForumTopics
                .Include(t => t.Responses)
                .FirstOrDefaultAsync(t => t.ForumTopicID == topicId);

            if (topic == null)
                return NotFound("Forum topic not found");

            // Increment view count
            topic.ViewCount++;
            await context.SaveChangesAsync();

            return Ok(topic);
        }
        catch (Exception ex)
        {
            Log.Error("Failed to get forum topic", ex);
            return BadRequest("Failed to get forum topic");
        }
    }

    // Get forum topics by module code
    [HttpGet("modules/{moduleCode}/topics")]
    public async Task<IActionResult> GetForumTopicsByModule([FromRoute] string moduleCode)
    {
        try
        {
            var topics = await context.ForumTopics
                .Where(t => t.RelatedModuleCode == moduleCode)
                .Include(t => t.Responses)
                .OrderByDescending(t => t.TopicCreationDate)
                .ToListAsync();

            return Ok(new
            {
                ModuleCode = moduleCode,
                TotalTopics = topics.Count,
                Topics = topics
            });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to get module forum topics", ex);
            return BadRequest("Failed to get module forum topics");
        }
    }

    // Get forum topics by user ID
    [HttpGet("users/{userProfileId}/topics")]
    public async Task<IActionResult> GetForumTopicsByUser([FromRoute] int userProfileId)
    {
        try
        {
            var topics = await context.ForumTopics
                .Where(t => t.UserProfileID == userProfileId)
                .Include(t => t.Responses)
                .OrderByDescending(t => t.TopicCreationDate)
                .ToListAsync();

            return Ok(new
            {
                UserProfileId = userProfileId,
                TotalTopics = topics.Count,
                Topics = topics
            });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to get user forum topics", ex);
            return BadRequest("Failed to get user forum topics");
        }
    }

    // Update forum topic (Owner only)
    [HttpPut("topics/{topicId}/users/{userProfileId}")]
    public async Task<IActionResult> UpdateForumTopic([FromRoute] int topicId,[FromRoute] int userProfileId,[FromBody] UpdateForumTopicRequest request)
    {
        try
        {
            var topic = await context.ForumTopics
                .FirstOrDefaultAsync(t => t.ForumTopicID == topicId && t.UserProfileID == userProfileId);

            if (topic == null)
                return BadRequest("Forum topic not found or not owned by this user");

            topic.ForumTopicTitle = request.Title ?? topic.ForumTopicTitle;
            topic.ForumTopicDescription = request.Description ?? topic.ForumTopicDescription;
            topic.RelatedModuleCode = request.ModuleCode ?? topic.RelatedModuleCode;
            topic.LastActivity = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return Ok(topic);
        }
        catch (Exception ex)
        {
            Log.Error("Failed to update forum topic", ex);
            return BadRequest("Failed to update forum topic");
        }
    }

    // Delete forum topic (Owner only)
    [HttpDelete("topics/{topicId}/users/{userProfileId}")]
    public async Task<IActionResult> DeleteForumTopic([FromRoute] int topicId, [FromRoute] int userProfileId)
    {
        try
        {
            var topic = await context.ForumTopics
                .FirstOrDefaultAsync(t => t.ForumTopicID == topicId && t.UserProfileID == userProfileId);

            if (topic == null)
                return BadRequest("Forum topic not found or not owned by this user");

            context.ForumTopics.Remove(topic);
            await context.SaveChangesAsync();

            return Ok(new { Message = "Forum topic deleted successfully" });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to delete forum topic", ex);
            return BadRequest("Failed to delete forum topic");
        }
    }

    // Upvote forum topic (Anyone can upvote)
    [HttpPost("topics/{topicId}/upvote")]
    public async Task<IActionResult> UpvoteForumTopic([FromRoute] int topicId)
    {
        try
        {
            var topic = await context.ForumTopics.FindAsync(topicId);
            if (topic == null)
                return NotFound("Forum topic not found");

            topic.TopicUpVote++;
            await context.SaveChangesAsync();

            return Ok(new
            {
                TopicId = topicId,
                Upvotes = topic.TopicUpVote,
                Message = "Topic upvoted successfully"
            });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to upvote forum topic", ex);
            return BadRequest("Failed to upvote forum topic");
        }
    }

    // Pin/unpin topic (Admin only)
    [HttpPost("topics/{topicId}/pin")]
    public async Task<IActionResult> PinForumTopic([FromRoute] int topicId, [FromBody] bool isPinned)
    {
        try
        {
            var topic = await context.ForumTopics.FindAsync(topicId);
            if (topic == null)
                return NotFound("Forum topic not found");

            topic.IsPinned = isPinned;
            await context.SaveChangesAsync();

            return Ok(new
            {
                TopicId = topicId,
                IsPinned = topic.IsPinned,
                Message = isPinned ? "Topic pinned" : "Topic unpinned"
            });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to pin forum topic", ex);
            return BadRequest("Failed to pin forum topic");
        }
    }

    // Lock/unlock topic (Admin only)
    [HttpPost("topics/{topicId}/lock")]
    public async Task<IActionResult> LockForumTopic([FromRoute] int topicId, [FromBody] bool isLocked)
    {
        try
        {
            var topic = await context.ForumTopics.FindAsync(topicId);
            if (topic == null)
                return NotFound("Forum topic not found");

            topic.IsLocked = isLocked;
            await context.SaveChangesAsync();

            return Ok(new
            {
                TopicId = topicId,
                IsLocked = topic.IsLocked,
                Message = isLocked ? "Topic locked" : "Topic unlocked"
            });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to lock forum topic", ex);
            return BadRequest("Failed to lock forum topic");
        }
    }
}