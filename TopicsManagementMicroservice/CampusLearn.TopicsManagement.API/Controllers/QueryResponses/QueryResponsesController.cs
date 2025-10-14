using CampusLearn.TopicsManagement.API.Controllers.QueryResponses.DTOs;

namespace CampusLearn.TopicsManagement.API.Controllers.QueryResponses;

[Route("[controller]")]
[ApiController]
public class QueryResponsesController(TopicsDbContext context, MinioService minio) : ControllerBase
{
    // Tutor creates response (with text and optional media)
    [HttpPost("querytopics/{queryTopicId}/tutors/{tutorId}/responses")]
    public async Task<IActionResult> CreateTutorResponse([FromRoute] int queryTopicId,[FromRoute] int tutorId,[FromForm] CreateTutorResponseRequest request)
    {
        try
        {
            // Verify query topic exists and is assigned to this tutor
            var queryTopic = await context.QueryTopics
                .FirstOrDefaultAsync(q => q.QueryTopicID == queryTopicId && q.AssignedTutorID == tutorId);

            if (queryTopic == null)
                return BadRequest("Query topic not found or not assigned to this tutor");

            var response = new QueryResponse
            {
                Comment = request.Comment,
                MediaContentUrl = null, 
                ResponseCreationDate = DateTime.UtcNow,
                TutorID = tutorId,
                QueryTopicID = queryTopicId,
                IsSolution = request.IsSolution
            };

            if(request.MediaContent != null)
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

            await context.QueryResponses.AddAsync(response);

            // Update query topic last activity
            queryTopic.LastActivity = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return Ok(new
            {
                Response = response,
                Message = "Tutor response created successfully"
            });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to create tutor response", ex);
            return BadRequest("Failed to create tutor response");
        }
    }

    // Student creates response (text only)
    [HttpPost("querytopics/{queryTopicId}/students/{studentId}/responses")]
    public async Task<IActionResult> CreateStudentResponse([FromRoute] int queryTopicId,[FromRoute] int studentId,[FromBody] CreateStudentResponseRequest request)
    {
        try
        {
            // Verify query topic exists and belongs to this student
            var queryTopic = await context.QueryTopics
                .FirstOrDefaultAsync(q => q.QueryTopicID == queryTopicId && q.StudentID == studentId);

            if (queryTopic == null)
                return BadRequest("Query topic not found or not owned by this student");

            var response = new QueryResponse
            {
                Comment = request.Comment,
                MediaContentUrl = null, // Students can't upload media
                ResponseCreationDate = DateTime.UtcNow,
                TutorID = -1, // This is a student response
                QueryTopicID = queryTopicId
            };

            await context.QueryResponses.AddAsync(response);

            // Update query topic last activity
            queryTopic.LastActivity = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return Ok(new
            {
                Response = response,
                Message = "Student response created successfully"
            });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to create student response", ex);
            return BadRequest("Failed to create student response");
        }
    }

    // Get all responses for a query topic
    [HttpGet("querytopics/{queryTopicId}/responses")]
    public async Task<IActionResult> GetResponsesByQueryTopic([FromRoute] int queryTopicId)
    {
        try
        {
            var responses = await context.QueryResponses
                .Where(r => r.QueryTopicID == queryTopicId)
                .OrderBy(r => r.ResponseCreationDate)
                .ToListAsync();

            return Ok(new
            {
                QueryTopicId = queryTopicId,
                TotalResponses = responses.Count,
                Responses = responses
            });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to get query topic responses", ex);
            return BadRequest("Failed to get query topic responses");
        }
    }

    // Get response by ID
    [HttpGet("responses/{responseId}")]
    public async Task<IActionResult> GetResponseById([FromRoute] int responseId)
    {
        try
        {
            var response = await context.QueryResponses
                .FirstOrDefaultAsync(r => r.QueryResponseID == responseId);

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

    // Tutor updates their response (can update text, media, mark as solution)
    [HttpPut("responses/{responseId}/tutors/{tutorId}")]
    public async Task<IActionResult> UpdateTutorResponse([FromRoute] int responseId,[FromRoute] int tutorId,[FromForm] UpdateTutorResponseRequest request)
    {
        try
        {
            var response = await context.QueryResponses
                .FirstOrDefaultAsync(r => r.QueryResponseID == responseId && r.TutorID == tutorId);

            if (response == null)
                return BadRequest("Response not found or not owned by this tutor");

            response.Comment = request.Comment ?? response.Comment;

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
            response.IsSolution = request.IsSolution;

            await context.SaveChangesAsync();

            return Ok(response);
        }
        catch (Exception ex)
        {
            Log.Error("Failed to update tutor response", ex);
            return BadRequest("Failed to update tutor response");
        }
    }


    // Delete response (both tutor and student)
    [HttpDelete("responses/{responseId}")]
    public async Task<IActionResult> DeleteResponse([FromRoute] int responseId)
    {
        try
        {
            var response = await context.QueryResponses.FindAsync(responseId);
            if (response == null)
                return NotFound("Response not found");

            context.QueryResponses.Remove(response);
            await context.SaveChangesAsync();

            return Ok(new { Message = "Response deleted successfully" });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to delete response", ex);
            return BadRequest("Failed to delete response");
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
