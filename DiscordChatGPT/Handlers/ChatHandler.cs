﻿using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
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