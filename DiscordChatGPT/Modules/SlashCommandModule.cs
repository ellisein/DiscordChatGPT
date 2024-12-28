using Discord;
using Discord.Interactions;
using DiscordChatGPT.Models.Modals;

namespace DiscordChatGPT.Modules;

public class SlashCommandModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("image", "Generate an image")]
    public async Task RunImageCommandAsync()
    {
        await RespondWithModalAsync<ImageGenerationModal>("image_generation");
    }
}