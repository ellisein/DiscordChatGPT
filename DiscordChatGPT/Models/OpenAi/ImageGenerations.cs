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
    
    [JsonPropertyName("size")]
    public string Size { get; init; } = "1024x1024";
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

public enum AspectRatio
{
    None,
    Square,
    Wide,
    Portrait,
}

public static class AspectRatioExtensions
{
    public static AspectRatio From(this string str)
    {
        return str.ToLower() switch
        {
            "square" => AspectRatio.Square,
            "wide" => AspectRatio.Wide,
            "portrait" => AspectRatio.Portrait,
            _ => AspectRatio.None,
        };
    }
    
    public static string ToStringValue(this AspectRatio ratio)
    {
        return ratio switch
        {
            AspectRatio.None => "1024x1024",
            AspectRatio.Square => "1024x1024",
            AspectRatio.Wide => "1792x1024",
            AspectRatio.Portrait => "1024x1792",
            _ => throw new ArgumentOutOfRangeException(nameof(ratio), ratio, null)
        };
    }
}

public enum Quality
{
    None,
    Standard,
    Hd,
}

public static class QualityExtensions
{
    public static Quality From(this string str)
    {
        return str.ToLower() switch
        {
            "standard" => Quality.Standard,
            "hd" => Quality.Hd,
            _ => Quality.None,
        };
    }
    
    public static string ToStringValue(this Quality quality)
    {
        return quality switch
        {
            Quality.None => "standard",
            Quality.Standard => "standard",
            Quality.Hd => "hd",
            _ => throw new ArgumentOutOfRangeException(nameof(quality), quality, null)
        };
    }
}