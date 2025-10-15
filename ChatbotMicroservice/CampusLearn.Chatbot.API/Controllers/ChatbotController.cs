namespace CampusLearn.Chatbot.API.Controllers;

[Route("[controller]")]
[ApiController]
public class ChatbotController(IChatbotService chatbot) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        try
        {
            var response = await chatbot.ChatbotAsync(request.Question);

            if (response.Trim().ToLower().StartsWith("error")) return BadRequest("Failed to process request");

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
}
