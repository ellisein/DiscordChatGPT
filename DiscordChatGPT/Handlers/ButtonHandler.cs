using Discord;
using Discord.Interactions;
using DiscordChatGPT.Models.OpenAi;
using DiscordChatGPT.Services;

namespace DiscordChatGPT.Handlers;

public class ButtonHandler : InteractionModuleBase<SocketInteractionContext>
{
    private readonly OpenAiService _openAiService;

    public ButtonHandler(OpenAiService openAiService)
    {
        _openAiService = openAiService;
    }
    
    [ComponentInteraction("image_generation:*:*:*")]
    public async Task OnTestButtonClickedAsync(string prompt, string aspectRatio, string quality)
    {
        await DeferAsync();
        
        var imageData = await _openAiService.GenerateImage(
            prompt,
            AspectRatioExtensions.From(aspectRatio),
            QualityExtensions.From(quality));

        if (imageData == null)
            return;

        var embed = new EmbedBuilder()
            .WithTitle("Generated Image")
            .WithDescription($"Prompt: {prompt}")
            .WithFooter(imageData.RevisedPrompt)
            .WithImageUrl(imageData.Url)
            .Build();
        var button = new ComponentBuilder()
            .WithButton("RETRY", $"image_generation:{prompt}:{aspectRatio}:{quality}",
                ButtonStyle.Secondary)
            .Build();
        await FollowupAsync(embed: embed, components: button);
    }
}