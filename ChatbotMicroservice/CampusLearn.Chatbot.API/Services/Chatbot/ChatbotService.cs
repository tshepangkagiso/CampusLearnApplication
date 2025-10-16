using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CampusLearn.Chatbot.API.Settings;
using Microsoft.Extensions.Options;

namespace CampusLearn.Chatbot.API.Services.Chatbot;

public class ChatbotService : IChatbotService
{
	private readonly HttpClient _client;
	private readonly AgentSettings _settings;

	private readonly Queue<ChatMessage> _history = new();
	private const int MaxPairs = 3; 

	private static readonly JsonSerializerOptions JsonOpts = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
	};

	public ChatbotService(HttpClient client, IOptions<AgentSettings> options)
	{
		_client = client ?? throw new ArgumentNullException(nameof(client));
		_settings = options?.Value ?? throw new ArgumentNullException(nameof(options));

		if (string.IsNullOrWhiteSpace(_settings.Url))
			throw new ArgumentException("AgentSettings.Url is required.");
		if (string.IsNullOrWhiteSpace(_settings.Model))
			throw new ArgumentException("AgentSettings.Model is required.");
		if (string.IsNullOrWhiteSpace(_settings.Key))
			throw new ArgumentException("AgentSettings.Key (API key) is required.");
	}

	public async Task<string> ChatbotAsync(string userQuestion)
		=> await ChatbotAsync(userQuestion, CancellationToken.None);

	public async Task<string> ChatbotAsync(string userQuestion, CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(userQuestion))
			return "Please provide a question.";

		if (CountPairs(_history) >= MaxPairs)
		{
			_history.Clear();
			return "Escalate";
		}

		var messages = new List<ChatMessage>
		{
			new("system", _settings.ChatbotRole ?? "You are a helpful academic assistant. Be concise and accurate.")
		};

		foreach (var m in _history) messages.Add(m);

		messages.Add(new ChatMessage("user", userQuestion));

		var req = new ChatRequest
		{
			Model = _settings.Model,
			Messages = messages,
			MaxTokens = _settings.MaxTokenUsageAllowed > 0 ? _settings.MaxTokenUsageAllowed : null,
			Temperature = _settings.Temperature is > 0 and <= 2 ? _settings.Temperature : 0.7
		};

		using var httpReq = new HttpRequestMessage(HttpMethod.Post, _settings.Url)
		{
			Content = new StringContent(JsonSerializer.Serialize(req, JsonOpts), Encoding.UTF8, "application/json")
		};

		httpReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.Key);
		httpReq.Headers.Accept.Clear();
		httpReq.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

		_client.Timeout = TimeSpan.FromSeconds(_settings.HttpTimeoutSeconds > 0 ? _settings.HttpTimeoutSeconds : 30);

		try
		{
			using var resp = await _client.SendAsync(httpReq, HttpCompletionOption.ResponseHeadersRead, ct);
			var body = await resp.Content.ReadAsStringAsync(ct);

			if (!resp.IsSuccessStatusCode)
			{
				var apiErr = TryExtractError(body);
				return $"Error: {(int)resp.StatusCode} {resp.ReasonPhrase}{(apiErr is null ? "" : $" – {apiErr}")}";
			}

			var content = TryExtractAssistantMessage(body) ?? "No response";

			_history.Enqueue(new ChatMessage("user", userQuestion));
			_history.Enqueue(new ChatMessage("assistant", content));
			TrimToLastPairs(_history, MaxPairs);

			if (CountPairs(_history) >= MaxPairs)
			{
				return content;
			}

			return content;
		}
		catch (TaskCanceledException) when (!ct.IsCancellationRequested)
		{
			return "Error: The AI service timed out. Please try again.";
		}
		catch (Exception ex)
		{
			return $"Error: {ex.Message}";
		}
	}

	public void ResetConversation() => _history.Clear();

	// ---------- helpers ----------

	private static int CountPairs(IEnumerable<ChatMessage> msgs)
	{
		int users = msgs.Count(m => m.Role == "user");
		int assists = msgs.Count(m => m.Role == "assistant");
		return Math.Min(users, assists);
	}

	private static void TrimToLastPairs(Queue<ChatMessage> q, int maxPairs)
	{
		while (CountPairs(q) > maxPairs)
		{
			if (q.Count == 0) break;
			q.Dequeue();
			if (q.Count == 0) break;
			q.Dequeue();
		}
	}

	private static string? TryExtractError(string json)
	{
		try
		{
			using var doc = JsonDocument.Parse(json);
			if (doc.RootElement.TryGetProperty("error", out var err))
			{
				if (err.ValueKind == JsonValueKind.Object && err.TryGetProperty("message", out var msg))
					return msg.GetString();
				return err.ToString();
			}
		}
		catch { /* ignore parse errors */ }
		return null;
	}

	private static string? TryExtractAssistantMessage(string json)
	{
		try
		{
			using var doc = JsonDocument.Parse(json);

			if (doc.RootElement.TryGetProperty("choices", out var choices) &&
				choices.ValueKind == JsonValueKind.Array &&
				choices.GetArrayLength() > 0)
			{
				var first = choices[0];
				if (first.TryGetProperty("message", out var msg) &&
					msg.TryGetProperty("content", out var content))
				{
					return content.GetString();
				}

				if (first.TryGetProperty("text", out var textNode))
					return textNode.GetString();
			}

			if (doc.RootElement.TryGetProperty("output_text", out var outText))
				return outText.GetString();
		}
		catch
		{

		}
		return null;
	}

	// ---------- DTOs ----------

	private sealed record ChatRequest
	{
		[JsonPropertyName("model")] public string Model { get; init; } = "";
		[JsonPropertyName("messages")] public List<ChatMessage> Messages { get; init; } = new();
		[JsonPropertyName("max_tokens")] public int? MaxTokens { get; init; }
		[JsonPropertyName("temperature")] public double? Temperature { get; init; }
	}

	private sealed record ChatMessage(
		[property: JsonPropertyName("role")] string Role,
		[property: JsonPropertyName("content")] string Content);
}
