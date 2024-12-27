using System.Text.Json.Serialization;

namespace DiscordChatGPT.Models.OpenAi;

public class ChatCompletionsRequest
{
    [JsonPropertyName("model")]
    public string Model { get; init; } = "";
    
    [JsonPropertyName("messages")]
    public List<ChatMessage> Messages { get; init; } = new();
}

public class ChatCompletion
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";
    
    [JsonPropertyName("choices")]
    public List<ChatCompletionChoice> Choices { get; init; } = new();
}

public class ChatMessage
{
    [JsonPropertyName("role")]
    public string Role { get; init; } = "";
    
    [JsonPropertyName("content")]
    public string Content { get; init; } = "";
}

public class ChatCompletionChoice
{
    [JsonPropertyName("index")]
    public int Index { get; init; }

    [JsonPropertyName("message")]
    public ChatMessage Message { get; init; } = new();
}