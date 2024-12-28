using Discord;
using Discord.Commands;
using DiscordChatGPT.Models.OpenAi;
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
        
        var size = AspectRatio.None;
        var split = prompt.Split();
        if (split.Length > 1)
        {
            size = AspectRatioExtensions.From(split[0]);
            if (size != AspectRatio.None)
            {
                prompt = string.Join(" ", split.Skip(1));
            }
        }

        var quality = Quality.None;
        split = prompt.Split();
        if (split.Length > 1)
        {
            quality = QualityExtensions.From(split[0]);
            if (quality != Quality.None)
            {
                prompt = string.Join(" ", split.Skip(1));
            }
        }
        
        var imageData = await _openAiService.GenerateImage(prompt, size, quality);

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