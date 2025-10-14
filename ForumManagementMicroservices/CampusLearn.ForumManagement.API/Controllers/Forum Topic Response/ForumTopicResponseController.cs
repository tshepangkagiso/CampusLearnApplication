namespace CampusLearn.ForumManagement.API.Controllers.Forum_Topic_Response;

[Route("[controller]")]
[ApiController]
public class ForumTopicResponseController(ForumDbContext context, MinioService minio) : ControllerBase
{
    // Create forum response (Anyone can respond with text/media)
    [HttpPost("forumtopics/{topicId}/responses")]
    public async Task<IActionResult> CreateForumResponse([FromRoute] int topicId,[FromForm] CreateForumResponseRequest request)
    {
        try
        {
            // Verify forum topic exists
            var topic = await context.ForumTopics.FindAsync(topicId);
            if (topic == null)
                return BadRequest("Forum topic not found");

            if (topic.IsLocked)
                return BadRequest("This topic is locked and cannot receive new responses");

            var response = new ForumTopicResponse
            {
                Comment = request.Comment,
                MediaContentUrl = "", 
                ResponseCreationDate = DateTime.UtcNow,
                UserProfileID = request.UserProfileID,
                ForumTopicID = topicId,
                IsAnonymous = request.IsAnonymous,
                AnonymousName = request.IsAnonymous ? request.AnonymousName : null
            };

            if (request.MediaContent != null)
            {
                var file = request.MediaContent;

                // Validate file size
                if (file.Length > 100 * 1024 * 1024) // 100MB limit
                {
                    Console.WriteLine($"File size {file.Length} exceeds limit");
                    return BadRequest("File size exceeds limit");
                }

                using Stream data = file.OpenReadStream();
                response.MediaContentUrl = $"{Guid.NewGuid()}_{file.FileName}";
                await minio.UploadFileAsync(response.MediaContentUrl, data, file.ContentType);
            }

            await context.ForumTopicResponses.AddAsync(response);

            // Update forum topic last activity
            topic.LastActivity = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return Ok(new
            {
                Response = response,
                Message = "Forum response created successfully"
            });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to create forum response", ex);
            return BadRequest("Failed to create forum response");
        }
    }


    // Get all responses for a forum topic
    [HttpGet("forumtopics/{topicId}/responses")]
    public async Task<IActionResult> GetResponsesByForumTopic([FromRoute] int topicId)
    {
        try
        {
            var responses = await context.ForumTopicResponses
                .Where(r => r.ForumTopicID == topicId)
                .OrderBy(r => r.ResponseCreationDate)
                .ToListAsync();

            return Ok(new
            {
                TopicId = topicId,
                TotalResponses = responses.Count,
                Responses = responses
            });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to get forum topic responses", ex);
            return BadRequest("Failed to get forum topic responses");
        }
    }

    // Get response by ID
    [HttpGet("responses/{responseId}")]
    public async Task<IActionResult> GetResponseById([FromRoute] int responseId)
    {
        try
        {
            var response = await context.ForumTopicResponses
                .FirstOrDefaultAsync(r => r.ResponseID == responseId);

            if (response == null)
                return NotFound("Response not found");

            return Ok(response);
        }
        catch (Exception ex)
        {
            Log.Error("Failed to get response", ex);
            return BadRequest("Failed to get response");
        }
    }

    // Update forum response (Owner only)
    [HttpPut("responses/{responseId}/users/{userProfileId}")]
    public async Task<IActionResult> UpdateForumResponse(
        [FromRoute] int responseId,
        [FromRoute] int userProfileId,
        [FromForm] UpdateForumResponseRequest request)
    {
        try
        {
            var response = await context.ForumTopicResponses
                .FirstOrDefaultAsync(r => r.ResponseID == responseId && r.UserProfileID == userProfileId);

            if (response == null)
                return BadRequest("Response not found or not owned by this user");

            if (request.MediaContent != null)
            {
                var file = request.MediaContent;

                // Validate file size
                if (file.Length > 100 * 1024 * 1024) // 100MB limit
                {
                    Console.WriteLine($"File size {file.Length} exceeds limit");
                    return BadRequest("File size exceeds limit");
                }

                using Stream data = file.OpenReadStream();
                response.MediaContentUrl = $"{Guid.NewGuid()}_{file.FileName}";
                await minio.UploadFileAsync(response.MediaContentUrl, data, file.ContentType);
            }

            response.Comment = request.Comment ?? response.Comment;

            // Update forum topic last activity
            var topic = await context.ForumTopics.FindAsync(response.ForumTopicID);
            if (topic != null)
            {
                topic.LastActivity = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();

            return Ok(response);
        }
        catch (Exception ex)
        {
            Log.Error("Failed to update forum response", ex);
            return BadRequest("Failed to update forum response");
        }
    }

    // Delete forum response (Owner only)
    [HttpDelete("responses/{responseId}/users/{userProfileId}")]
    public async Task<IActionResult> DeleteForumResponse([FromRoute] int responseId, [FromRoute] int userProfileId)
    {
        try
        {
            var response = await context.ForumTopicResponses
                .FirstOrDefaultAsync(r => r.ResponseID == responseId && r.UserProfileID == userProfileId);

            if (response == null)
                return BadRequest("Response not found or not owned by this user");

            context.ForumTopicResponses.Remove(response);

            // Update forum topic last activity
            var topic = await context.ForumTopics.FindAsync(response.ForumTopicID);
            if (topic != null)
            {
                topic.LastActivity = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();

            return Ok(new { Message = "Forum response deleted successfully" });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to delete forum response", ex);
            return BadRequest("Failed to delete forum response");
        }
    }

    // Upvote forum response (Anyone can upvote)
    [HttpPost("responses/{responseId}/upvote")]
    public async Task<IActionResult> UpvoteForumResponse([FromRoute] int responseId)
    {
        try
        {
            var response = await context.ForumTopicResponses.FindAsync(responseId);
            if (response == null)
                return NotFound("Response not found");

            response.ResponseUpVote++;

            // Update forum topic last activity
            var topic = await context.ForumTopics.FindAsync(response.ForumTopicID);
            if (topic != null)
            {
                topic.LastActivity = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();

            return Ok(new
            {
                ResponseId = responseId,
                Upvotes = response.ResponseUpVote,
                Message = "Response upvoted successfully"
            });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to upvote forum response", ex);
            return BadRequest("Failed to upvote forum response");
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
