using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Net;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using DSharpPlus.VoiceNext;

namespace Stella
{
    internal class Program
    {

        static void Main(string[] args)
        {
            //MainAsync().GetAwaiter().GetResult();
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {


            var token = Environment.GetEnvironmentVariable("STELLA_TOKEN");

            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = token,
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Debug,
                Intents = DiscordIntents.MessageContents | DiscordIntents.AllUnprivileged
            });

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new[] { "!" }
            });

            commands.RegisterCommands<Commands>();

            var endpoint = new ConnectionEndpoint
            {
                Hostname = "127.0.0.1", // From your server configuration.
                Port = 2333 // From your server configuration
            };

            var lavalinkConfig = new LavalinkConfiguration
            {
                Password = "youshallnotpass", // From your server configuration.
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };

            var lavalink = discord.UseLavalink();

            var config = new SlashCommandsConfiguration();

            var slashcommands = discord.UseSlashCommands(config);
            discord.UseVoiceNext();
            slashcommands.RegisterCommands<SlashCommands>(873564790629482516);
            slashcommands.RegisterCommands<SlashCommands>(111536006364180480);
            var a = slashcommands.RegisteredCommands;


            await discord.ConnectAsync();
            await lavalink.ConnectAsync(lavalinkConfig);
            await Task.Delay(-1);
        }


    }
}