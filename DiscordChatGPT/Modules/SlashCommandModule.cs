using Discord.Interactions;
using DiscordChatGPT.Models.Modals;

namespace DiscordChatGPT.Modules;

public class SlashCommandModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("image", "Generates an image")]
    public async Task RunImageCommandAsync()
    {
        await RespondWithModalAsync<ImageGenerationModal>("image_generation");
    }
}