namespace CampusLearn.Chatbot.API.Controllers.DTOs;

public class AiAgentDTO
{
    public string sessionId { get; set; } = "CampusLearn";
    public string action { get; set; } = "sendMessage";
    public string chatInput { get; set; } = string.Empty;
}
