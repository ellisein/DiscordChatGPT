using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using DiscordChatGPT.Models.OpenAi;

namespace DiscordChatGPT.Services;

public class OpenAiService
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;

    private const string GptModel = "gpt-4o-mini";

    public OpenAiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = Environment.GetEnvironmentVariable("OPENAI_KEY");
    }

    public async Task<string> CreateChatCompletions(string message)
    {
        var messages = new List<ChatMessage> { new ChatMessage { Role = "user", Content = message } };
        var requestBody = new ChatCompletionsRequest
        {
            Model = GptModel,
            Messages = messages
        };
        var jsonContent = JsonSerializer.Serialize(requestBody);

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");
        request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        var responseBody = await response.Content.ReadFromJsonAsync<ChatCompletion>();
        return responseBody?.Choices.FirstOrDefault()?.Message.Content ?? "";
    }
}