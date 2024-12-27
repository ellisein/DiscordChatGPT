using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordChatGPT.Services;

namespace DiscordChatGPT.Handlers;

public class ChatHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly OpenAiService _openAiService;

    public ChatHandler(DiscordSocketClient client, CommandService commands, OpenAiService openAiService)
    {
        _client = client;
        _commands = commands;
        _openAiService = openAiService;
    }

    public async Task InstallCommandsAsync()
    {
        _client.MessageReceived += HandleChatAsync;
        await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                        services: null);
    }

    private async Task HandleChatAsync(SocketMessage messageParam)
    {
        if (messageParam is not SocketUserMessage message)
            return;

        var argPos = 0;
        if (!message.HasMentionPrefix(_client.CurrentUser, ref argPos) ||
            message.Author.IsBot)
            return;
        
        await message.Channel.TriggerTypingAsync();
        var response = await _openAiService.CreateChatCompletions(message.Content);
        await SendMessageAsync(message.Channel, response);
    }

    private static async Task SendMessageAsync(IMessageChannel channel, string message)
    {
        const int maxMessageLength = 2000;

        for (var i = 0; i < message.Length; i += maxMessageLength)
        {
            var chunk = message.Substring(i,Math.Min(maxMessageLength, message.Length - i));
            await channel.SendMessageAsync(chunk);
        }
    }
}