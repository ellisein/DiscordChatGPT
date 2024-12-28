using Discord.WebSocket;

namespace DiscordChatGPT.Handlers;

public class ButtonHandler
{
    public async Task OnButtonExecutedAsync(SocketMessageComponent component)
    {
        switch (component.Data.CustomId)
        {
            case "":
                break;
        }
    }
}