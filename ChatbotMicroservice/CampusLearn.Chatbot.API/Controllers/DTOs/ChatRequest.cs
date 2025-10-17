namespace CampusLearn.Chatbot.API.Controllers.DTOs;

public class ChatRequest
{
    public int StudentId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string ModuleCode { get; set; } = string.Empty;
}
