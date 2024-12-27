using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordChatGPT.Models.OpenAi;
using DiscordChatGPT.Services;

namespace DiscordChatGPT.Handlers;

public class ChatHandler
{
    private readonly IServiceProvider _services;
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly OpenAiService _openAiService;

    public ChatHandler(IServiceProvider services, DiscordSocketClient client,
        CommandService commands, OpenAiService openAiService)
    {
        _services = services;
        _client = client;
        _commands = commands;
        _openAiService = openAiService;
    }

    public async Task Initialize()
    {
        _client.MessageReceived += HandleChatAsync;
        await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
            services: _services);
    }

    private async Task HandleChatAsync(SocketMessage messageParam)
    {
        if (messageParam is not SocketUserMessage message)
            return;

        if (message.Author.IsBot)
            return;

        var argPos = 0;
        if (message.HasMentionPrefix(_client.CurrentUser, ref argPos))
        {
            await message.Channel.TriggerTypingAsync();
            var response = await _openAiService.CreateChatCompletions(message.Content);
            await SendMessageAsync(message.Channel, response);
        }
        else if (message.HasCharPrefix('!', ref argPos))
        {
            var context = new SocketCommandContext(_client, message);

            await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: _services);
        }
    }

    private static async Task SendMessageAsync(IMessageChannel channel, string message)
    {
        const int maxMessageLength = 2000;

        for (var i = 0; i < message.Length; i += maxMessageLength)
        {
            var chunk = message.Substring(i, Math.Min(maxMessageLength, message.Length - i));
            await channel.SendMessageAsync(chunk);
        }
    }
}

public class CommandModule : ModuleBase<SocketCommandContext>
{
    private readonly OpenAiService _openAiService;

    public CommandModule(OpenAiService openAiService)
    {
        _openAiService = openAiService;
    }
    
    [Command("echo")]
    [Summary("Echo a message")]
    public async Task EchoAsync(string message)
    {
        await Context.Channel.SendMessageAsync($"Echo: {message}");
    }

    [Command("image")]
    [Summary("Generates an image")]
    public async Task GenerateImage([Remainder] string prompt)
    {
        await Context.Channel.SendMessageAsync("Generating an image...");

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