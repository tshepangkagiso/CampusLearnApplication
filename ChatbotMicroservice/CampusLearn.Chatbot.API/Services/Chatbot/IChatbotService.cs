namespace CampusLearn.Chatbot.API.Services.Chatbot;

public interface IChatbotService
{
    Task<string> CampusLearnAutomationAgentAsync(string userQuestion, string moduleCode);
    Task<string> ChatbotAsync(string userQuestion);
}
