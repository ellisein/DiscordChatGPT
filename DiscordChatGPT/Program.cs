using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordChatGPT.Handlers;
using DiscordChatGPT.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordChatGPT;

public class Program
{
    public static async Task Main()
    {
        var servicesProvider = ConfigureServices();
        
        var client = servicesProvider.GetRequiredService<DiscordSocketClient>();
        client.Log += Log;
        
        var chatHandler = servicesProvider.GetRequiredService<ChatHandler>();
        await chatHandler.InstallCommandsAsync();
        
        var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
        
        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();
        
        await Task.Delay(-1);
    }

    private static IServiceProvider ConfigureServices()
    {
        var collection = new ServiceCollection()
            .AddHttpClient()
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<OpenAiService>()
            .AddSingleton<CommandService>()
            .AddSingleton<ChatHandler>();
        return collection.BuildServiceProvider();
    }
    
    private static Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}