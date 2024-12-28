using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using DiscordChatGPT.Models.OpenAi;

namespace DiscordChatGPT.Services;

public class OpenAiService
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;

    private const string ChatModel = "gpt-4o-mini";
    private const string ImageModel = "dall-e-3";
    
    private readonly List<ChatMessage> _history = new();

    public OpenAiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = Environment.GetEnvironmentVariable("OPENAI_KEY");
    }

    public async Task<string> CreateChatCompletions(string message)
    {
        _history.Add(new ChatMessage { Role = "user", Content = message });
        
        var requestBody = new ChatCompletionsRequest
        {
            Model = ChatModel,
            Messages = _history,
        };
        var jsonContent = JsonSerializer.Serialize(requestBody);

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");
        request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        var responseBody = await response.Content.ReadFromJsonAsync<ChatCompletion>();
        var assistantChat = responseBody?.Choices.FirstOrDefault()?.Message;
        if (assistantChat != null)
        {
            _history.Add(assistantChat);
        }

        EnsureLengthOfHistory();
        
        return responseBody?.Choices.FirstOrDefault()?.Message.Content ?? "";
    }

    private void EnsureLengthOfHistory()
    {
        const int maxLength = 10000;
        var length = _history.Sum(chat => chat.Content.Length);
        while (length > maxLength && _history.Count > 1)
        {
            length -= _history[0].Content.Length;
            _history.RemoveAt(0);
        }
    }

    public async Task<ImageData?> GenerateImage(
        string prompt, AspectRatio size = AspectRatio.Square, Quality quality = Quality.Standard)
    {
        var requestBody = new ImageGenerationsRequest
        {
            Model = ImageModel,
            Prompt = prompt,
            Size = size.ToStringValue(),
            Quality = quality.ToStringValue(),
        };
        var jsonContent = JsonSerializer.Serialize(requestBody);

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/images/generations");
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");
        request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        var responseBody = await response.Content.ReadFromJsonAsync<ImageGeneration>();
        return responseBody?.Data.FirstOrDefault();
    }
}