using Discord;
using Discord.WebSocket;

namespace DiscordChatGPT.Handlers;

public class SlashCommandHandler
{
    public static async Task HandleSlashCommandAsync(SocketSlashCommand command)
    {
        switch (command.Data.Name)
        {
            case "help":
                await HandleCommandHelpAsync(command);
                break;
        }
    }

    private static async Task HandleCommandHelpAsync(SocketSlashCommand command)
    {
        var embed = new EmbedBuilder()
            .WithTitle("Commands")
            .WithDescription("!image (square|wide|portrait) (standard|hd) {prompt}")
            .Build();
        await command.RespondAsync(embed: embed);
    }
}