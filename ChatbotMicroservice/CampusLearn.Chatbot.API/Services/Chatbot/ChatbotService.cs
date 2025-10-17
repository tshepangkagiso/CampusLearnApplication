namespace CampusLearn.Chatbot.API.Services.Chatbot;

public class ChatbotService(HttpClient client, IOptions<AgentSettings> options) : IChatbotService
{
    private readonly AgentSettings agentSettings = options.Value;
    private readonly List<object> conversationHistory = new();

    public async Task<string> ChatbotAsync(string userQuestion)
    {
        // Check if we've reached the 3-message limit
        if (conversationHistory.Count >= 6) // 3 user + 3 assistant messages
        {
            conversationHistory.Clear(); // Reset for next conversation
            return "Escalate";
        }

        // Build messages with history
        var messages = new List<object>
        {
            new { role = "system", content = agentSettings.ChatbotRole }
        };

        // Add conversation history
        messages.AddRange(conversationHistory);

        // Add current question
        messages.Add(new { role = "user", content = userQuestion });

        // Configure the API call
        var requestData = new
        {
            model = agentSettings.Model,
            messages = messages,
            max_tokens = agentSettings.MaxTokenUsageAllowed
        };

        var json = JsonSerializer.Serialize(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {agentSettings.Key}");
        var response = await client.PostAsync(agentSettings.Url, content);

        if (!response.IsSuccessStatusCode)
        {
            return $"Error: {response.StatusCode}";
        }

        var responseContent = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(responseContent))
        {
            return "Error: Empty response from AI service";
        }

        using var document = JsonDocument.Parse(responseContent);

        var chatbotResponse = document.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "No response";

        // Store conversation history (both question and response)
        conversationHistory.Add(new { role = "user", content = userQuestion });
        conversationHistory.Add(new { role = "assistant", content = chatbotResponse });

        return chatbotResponse;
    }


    public async Task<string> CampusLearnAutomationAgentAsync(string userQuestion, string moduleCode)
    {
        string url = "http://n8n:5678/webhook/05177e7a-eb69-4098-bbb5-2176425c2088/chat";

        var request = new AiAgentDTO
        {
            sessionId = "CampusLearn",
            action = "sendMessage",
            chatInput = $"moduleCode: {moduleCode}, question: {userQuestion}"
        };

        var response = await client.PostAsJsonAsync(url, request);
        var responseContent = await response.Content.ReadAsStringAsync();

        try
        {
            var outerResponse = JsonSerializer.Deserialize<N8nResponse>(responseContent);
            var innerResponse = JsonSerializer.Deserialize<N8nInnerResponse>(outerResponse.response);
            return innerResponse.output;
        }
        catch (JsonException ex)
        {
            // Log the error and return raw response for debugging
            Console.WriteLine($"JSON parsing error: {ex.Message}");
            return responseContent;
        }
    }
}
public class N8nResponse
{
    public string response { get; set; } = string.Empty;
}

public class N8nInnerResponse
{
    public string output { get; set; } = string.Empty;
}
