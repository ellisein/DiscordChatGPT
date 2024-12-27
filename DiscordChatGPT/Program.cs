using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordChatGPT.Handlers;
using DiscordChatGPT.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordChatGPT;

public class Program
{
    private static DiscordSocketClient _client;
    
    public static async Task Main()
    {
        var servicesProvider = ConfigureServices();
        
        _client = servicesProvider.GetRequiredService<DiscordSocketClient>();
        _client.Log += Log;
        _client.Ready += OnClientReady;
        
        var chatHandler = servicesProvider.GetRequiredService<ChatHandler>();
        await chatHandler.Initialize();
        
        var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
        
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        
        await Task.Delay(-1);
    }

    private static IServiceProvider ConfigureServices()
    {
        var socketConfig = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
        };
        var collection = new ServiceCollection()
            .AddHttpClient()
            .AddSingleton(socketConfig)
            .AddSingleton<DiscordSocketClient>(provider =>
            {
                var cfg = provider.GetRequiredService<DiscordSocketConfig>();
                return new DiscordSocketClient(cfg);
            })
            .AddSingleton<OpenAiService>()
            .AddSingleton<CommandService>()
            .AddSingleton<ChatHandler>();
        return collection.BuildServiceProvider();
    }

    private static async Task OnClientReady()
    {
        var globalCommand = new SlashCommandBuilder()
            .WithName("test")
            .WithDescription("Test Command")
            .Build();
        await _client.CreateGlobalApplicationCommandAsync(globalCommand);
        
        _client.SlashCommandExecuted += SlashCommandHandler.HandleSlashCommandAsync;
    }
    
    private static Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}