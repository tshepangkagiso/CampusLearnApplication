namespace CampusLearn.Chatbot.API.Services.Chatbot;

public interface IChatbotService
{
    Task<string> ChatbotAsync(string userQuestion);
}
