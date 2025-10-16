using Microsoft.AspNetCore.Mvc;

namespace CampusLearn.Chatbot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ChatbotController : ControllerBase
{
    private readonly IChatbotService _chatbot;
    private readonly ILogger<ChatbotController> _logger;

    public ChatbotController(IChatbotService chatbot, ILogger<ChatbotController> logger)
    {
        _chatbot = chatbot;
        _logger = logger;
    }

    [HttpOptions]
    public IActionResult Options() => Ok();

    [HttpPost]
    [Consumes("application/json")]
    public async Task<ActionResult<ChatResponseDto>> Chat([FromBody] ChatRequestDto? request)
    {
        if (request is null)
            return BadRequest(new ProblemDetails { Title = "Invalid body", Detail = "Request body is required." });

        if (string.IsNullOrWhiteSpace(request.Question))
            return BadRequest(new ProblemDetails { Title = "Invalid question", Detail = "Question cannot be empty." });

        try
        {
            var answer = await _chatbot.ChatbotAsync(request.Question, HttpContext.RequestAborted);
            var lower = answer.Trim().ToLowerInvariant();

           if (lower.StartsWith("error"))
            {
                _logger.LogWarning("Upstream AI error for student {StudentId}, module {ModuleCode}: {Answer}",
                    request.StudentId, request.ModuleCode, answer);

                return StatusCode(StatusCodes.Status502BadGateway, new ProblemDetails
                {
                    Title = "AI upstream error",
                    Detail = "Failed to process the request via the AI backend. Please try again."
                });
            }

            if (lower.StartsWith("escalate"))
            {
                var payload = new ChatResponseDto(
                    Response: "Your matter was complex and has been escalated to a tutor. You’ll get a response soon.",
                    StudentId: request.StudentId,
                    ModuleCode: request.ModuleCode,
                    Escalated: true
                );
                return Accepted(payload);
            }

            return Ok(new ChatResponseDto(
                Response: answer,
                StudentId: request.StudentId,
                ModuleCode: request.ModuleCode,
                Escalated: false
            ));
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status499ClientClosedRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error in Chat endpoint for student {StudentId}, module {ModuleCode}",
                request.StudentId, request.ModuleCode);

            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Server error",
                Detail = "An unexpected error occurred while processing your request."
            });
        }
    }
}

// ---------- DTOs ----------

public sealed record ChatRequestDto(
    string Question,
    string? StudentId,
    string? ModuleCode
);

public sealed record ChatResponseDto(
    string Response,
    string? StudentId,
    string? ModuleCode,
    bool Escalated
);
