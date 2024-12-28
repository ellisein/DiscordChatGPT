using Discord.WebSocket;

namespace DiscordChatGPT.Handlers;

public class MenuHandler
{
    public async Task OnSelectMenuExecutedAsync(SocketMessageComponent component)
    {
        switch (component.Data.CustomId)
        {
            case "":
                break;
        }
    }
}