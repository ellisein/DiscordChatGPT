using Discord;
using Discord.Interactions;
using DiscordChatGPT.Models.Modals;
using DiscordChatGPT.Models.OpenAi;
using DiscordChatGPT.Services;

namespace DiscordChatGPT.Modules;

public class ModalHandlerModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly OpenAiService _openAiService;

    public ModalHandlerModule(OpenAiService openAiService)
    {
        _openAiService = openAiService;
    }
    
    [ModalInteraction("image_generation")]
    public async Task HandleImageCommand(ImageGenerationModal modal)
    {
        await DeferAsync();
        
        var imageData = await _openAiService.GenerateImage(
            modal.Prompt,
            AspectRatioExtensions.From(modal.AspectRatio),
            QualityExtensions.From(modal.Quality));

        if (imageData == null)
            return;

        var embed = new EmbedBuilder()
            .WithTitle("Generated Image")
            .WithDescription($"Prompt: {modal.Prompt}")
            .WithFooter(imageData.RevisedPrompt)
            .WithImageUrl(imageData.Url)
            .Build();
        var button = new ComponentBuilder()
            .WithButton("RETRY", $"image_generation:{modal.Prompt}:{modal.AspectRatio}:{modal.Quality}",
                ButtonStyle.Secondary)
            .Build();
        await FollowupAsync(embed: embed, components: button);
    }
}