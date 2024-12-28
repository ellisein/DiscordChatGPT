using Discord.Interactions;

namespace DiscordChatGPT.Handlers;

public class MenuHandler : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("test_menu")]
    public async Task OnTestMenuSelectedAsync(string[] selected)
    {
    }
}