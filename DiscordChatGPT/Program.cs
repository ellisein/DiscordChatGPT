using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordChatGPT.Handlers;
using DiscordChatGPT.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordChatGPT;

public class Program
{
    private static DiscordSocketClient _client;
    private static IServiceProvider _services;
    
    public static async Task Main()
    {
        _services = ConfigureServices();
        
        _client = _services.GetRequiredService<DiscordSocketClient>();
        _client.Log += Log;
        _client.Ready += OnClientReady;
        
        var chatHandler = _services.GetRequiredService<ChatHandler>();
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
            .AddSingleton<ChatHandler>()
            .AddSingleton<ButtonHandler>()
            .AddSingleton<MenuHandler>();
        return collection.BuildServiceProvider();
    }

    private static async Task OnClientReady()
    {
        var interactionService = new InteractionService(_client);
        await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        await interactionService.RegisterCommandsGloballyAsync();

        _client.InteractionCreated += async interaction =>
        {
            var scope = _services.CreateScope();
            var ctx = new SocketInteractionContext(_client, interaction);
            await interactionService.ExecuteCommandAsync(ctx, scope.ServiceProvider);
        };
        
        var buttonHandler = _services.GetRequiredService<ButtonHandler>();
        _client.ButtonExecuted += buttonHandler.OnButtonExecutedAsync;
        
        var menuHandler = _services.GetRequiredService<MenuHandler>();
        _client.SelectMenuExecuted += menuHandler.OnSelectMenuExecutedAsync;
    }
    
    private static Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}