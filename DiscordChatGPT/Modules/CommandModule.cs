using Discord;
using Discord.Commands;
using DiscordChatGPT.Services;

namespace DiscordChatGPT.Modules;

public class CommandModule : ModuleBase<SocketCommandContext>
{
    private readonly OpenAiService _openAiService;

    public CommandModule(OpenAiService openAiService)
    {
        _openAiService = openAiService;
    }
    
    [Command("image")]
    [Summary("Generates an image")]
    public async Task GenerateImage([Remainder] string prompt)
    {
        await Context.Channel.SendMessageAsync("Generating an image...");
        await Context.Channel.TriggerTypingAsync();
        
        var imageData = await _openAiService.GenerateImage(prompt);

        if (imageData == null)
            return;

        var embed = new EmbedBuilder()
            .WithTitle("Generated Image")
            .WithFooter(imageData.RevisedPrompt)
            .WithImageUrl(imageData.Url)
            .Build();
        await ReplyAsync(embed: embed);
    }
}