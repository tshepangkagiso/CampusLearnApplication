namespace CampusLearn.Chatbot.API.Controllers;

[ApiController]
[Route("[controller]")]

public class ChatbotController(IChatbotService chatbot) : ControllerBase
{
    // deepseek chatbot 
    [HttpPost("chatbot")]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        try
        {
            var response = await chatbot.ChatbotAsync(request.Question);

            if (response.Trim().ToLower().StartsWith("error")) return BadRequest("");

            if (response.Trim().ToLower().StartsWith("escalate"))
            {
                //escalate the matter to a tutor here, by sending an email
                return Ok(new
                {
                    response = "Your matter was complex and this issue is now escalated to a tutor.",
                    studentId = request.StudentId,
                    modulecode = request.ModuleCode
                });
            }

            return Ok(new { response = response, studentId = request.StudentId, modulecode = request.ModuleCode });
        }
        catch (Exception ex)
        {
            return BadRequest("Failed to process request");
        }
    }



    // ai agent made with n8n and gemini ai
    [HttpPost("n8n/ai-agent")]
    public async Task<IActionResult> AiAgent([FromBody] ChatRequest request)
    {
        try
        {
            var response = await chatbot.CampusLearnAutomationAgentAsync(request.Question, request.ModuleCode);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to process request: {ex.Message}");
        }
    }
}
