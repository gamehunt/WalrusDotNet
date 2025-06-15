using System.Reflection;
using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;
using DisCatSharp.Enums;
using Walrus.Commands;

namespace Walrus;
class Program {
    static void Main(string[] args)
    {
        MainAsync().GetAwaiter().GetResult();
    }

    static async Task MainAsync()
    {
        DotNetEnv.Env.Load();
        var client = new DiscordClient(
                new DiscordConfiguration()
                {
                    Token = DotNetEnv.Env.GetString("TOKEN"),
                    TokenType = TokenType.Bot,
                    Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContent,
                    AutoReconnect = true,
                    Proxy = HttpClient.DefaultProxy,
                });
    
        client.RegisterEventHandlers(Assembly.GetExecutingAssembly());
    
        var appCommands = client.UseApplicationCommands();
    
        ulong guildId = ulong.Parse(DotNetEnv.Env.GetString("GUILD_ID"));

        appCommands.RegisterGuildCommands<UtilCommands>(guildId);
    
    #if !DEBUG
        appCommands.RegisterGlobalCommands<UtilCommands>();
    #endif

        client.Ready += async (c, ev) => {
            await c.UpdateStatusAsync(new DiscordActivity() {
                ActivityType = ActivityType.Watching,
                Name = "за кубами" 
            });
        };

        client.MessageCreated += (c, ev) => {
            if(ev.Author.IsBot) {
                return Task.CompletedTask;
            }

            _ = Task.Run(async () =>
            {
                string r = RollEngine.Evaluate(ev.Message.Content);

                if(r.Length > 0 && r.Length < 2000) {
                    await ev.Message.RespondAsync(r);
                }
            });

	        return Task.CompletedTask;
        };
    
        await client.ConnectAsync();
        await Task.Delay(-1);
    }
}

