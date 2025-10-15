namespace CampusLearn.Chatbot.API.Settings;

public class AgentSettings
{
    public string Key { get; set; } = string.Empty;
    public string Url {  get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string ChatbotRole { get; set; } = string.Empty;
    public int MaxTokenUsageAllowed {  get; set; }
}
