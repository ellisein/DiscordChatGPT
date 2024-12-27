using System.Text.Json.Serialization;

namespace DiscordChatGPT.Models.OpenAi;

public class ImageGenerationsRequest
{
    [JsonPropertyName("prompt")]
    public string Prompt { get; init; } = "";
    
    [JsonPropertyName("model")]
    public string Model { get; init; } = "";
    
    [JsonPropertyName("quality")]
    public string Quality { get; init; } = "standard";
}

public class ImageGeneration
{
    [JsonPropertyName("data")]
    public List<ImageData> Data { get; init; } = new();
}

public class ImageData
{
    [JsonPropertyName("revised_prompt")]
    public string RevisedPrompt { get; init; } = "";
    
    [JsonPropertyName("url")]
    public string Url { get; init; } = "";    
}