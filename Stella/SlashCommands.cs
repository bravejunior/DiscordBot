using DSharpPlus;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stella
{
    internal class SlashCommands : ApplicationCommandModule
    {
        public SlashCommands(IServiceProvider provider) { }


        [SlashCommand("slash-command-test", "literally just testing slash commands")]
        public async Task TestCommandAsync(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("Yeah that works!");
        }


        [SlashCommand("join", "I'll join the voice call you're currently in if possible.")]
        public async Task Join(InteractionContext ctx)
        {

            var lava = ctx.Client.GetLavalink();

            bool isDm = ctx.Channel.IsPrivate;

            if (isDm)
            {
                await ctx.CreateResponseAsync("You can't do this in a DM, have to be in a server :/");
                return;
            }


            var state = ctx.Member.VoiceState;

            if (state == null)
            {
                await ctx.CreateResponseAsync("You have to be in a voice channel in order for me to join, I'm so terribly sorry.");
                return;
            }

            var channel = ctx.Member.VoiceState.Channel;

            if (!lava.ConnectedNodes.Any())
            {
                await ctx.CreateResponseAsync("The Lavalink connection is not established");
                return;
            }

            var node = lava.ConnectedNodes.Values.First();

            if (channel.Type != ChannelType.Voice)
            {
                await ctx.CreateResponseAsync("Not a valid voice channel.");
                return;
            }

            await node.ConnectAsync(channel);
            await ctx.CreateResponseAsync($"Joined {channel.Name}!");
        }

        [SlashCommand("leave", "I'll leave the voice call. You're required to be in the same call in order for me to leave.")]
        public async Task Leave(InteractionContext ctx)
        {
            bool isDm = ctx.Channel.IsPrivate;

            if (isDm)
            {
                await ctx.CreateResponseAsync("You can't do this in a DM, have to be in a server :/");
                return;
            }

            var state = ctx.Member.VoiceState;

            if (state == null)
            {
                await ctx.CreateResponseAsync("You have to be in my channel you want me to leave, I'm so sorry.");
                return;
            }

            var channel = ctx.Member.VoiceState.Channel;

            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.CreateResponseAsync("The Lavalink connection is not established");
                return;
            }

            var node = lava.ConnectedNodes.Values.First();

            if (channel.Type != ChannelType.Voice)
            {
                await ctx.CreateResponseAsync("Not a valid voice channel.");
                return;
            }

            var conn = node.GetGuildConnection(channel.Guild);

            if (conn == null)
            {
                await ctx.CreateResponseAsync("Lavalink is not connected.");
                return;
            }

            await conn.DisconnectAsync();
            await ctx.CreateResponseAsync($"Left {channel.Name}!");

        }

        [SlashCommand("play", "I'll play the youtube URL provided (if I'm in the voice call and if it's a valid url")]
        public async Task Play(InteractionContext ctx, [OptionAttribute("url", "the url to the song")] string url)
        {
            bool isDm = ctx.Channel.IsPrivate;

            Uri uri = new Uri(url);

            if (isDm)
            {
                await ctx.CreateResponseAsync("You can't do this in a DM, have to be in a server :/");
                return;
            }

            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.CreateResponseAsync("You are not in a voice channel.");
                return;
            }

            //var url = "a";

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.CreateResponseAsync("I can't play :(");
                return;
            }

            var loadResult = await node.Rest.GetTracksAsync(uri);

            var track = loadResult.Tracks.First();

            await conn.PlayAsync(track);

            await ctx.CreateResponseAsync($"Now playing {track.Title}!");

        }

        [SlashCommand("pause", "I'll pause the music I'm currently playing")]
        public async Task Pause(InteractionContext ctx)
        {
            bool isDm = ctx.Channel.IsPrivate;

            if (isDm)
            {
                await ctx.CreateResponseAsync("You can't do this in a DM, have to be in a server :/");
                return;
            }
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.CreateResponseAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.CreateResponseAsync("Lavalink is not connected.");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.CreateResponseAsync("There are no tracks loaded.");
                return;
            }

            await conn.PauseAsync();
        }

        [SlashCommand("resume", "I'll resume the music")]
        public async Task Resume(InteractionContext ctx)
        {
            bool isDm = ctx.Channel.IsPrivate;

            if (isDm)
            {
                await ctx.CreateResponseAsync("You can't do this in a DM, have to be in a server :/");
                return;
            }
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.CreateResponseAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.CreateResponseAsync("Lavalink is not connected.");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.CreateResponseAsync("There are no tracks loaded.");
                return;
            }

            await conn.ResumeAsync();
        }

        [SlashCommand("seek", "I'll start playing at selected time (hours+minutes+seconds)")]
        public async Task FastForward(InteractionContext ctx, [OptionAttribute("hours", "time in hours")] long hours,
                                                              [OptionAttribute("minutes", "time in minutes")] long minutes,
                                                              [OptionAttribute("seconds", "time in seconds")] long seconds)
        {
            bool isDm = ctx.Channel.IsPrivate;

            if (isDm)
            {
                await ctx.CreateResponseAsync("You can't do this in a DM, have to be in a server :/");
                return;
            }
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.CreateResponseAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.CreateResponseAsync("Lavalink is not connected.");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.CreateResponseAsync("There are no tracks loaded.");
                return;
            }

            int h = Convert.ToInt32(hours);
            int m = Convert.ToInt32(minutes);
            int s = Convert.ToInt32(seconds);

            TimeSpan span = new TimeSpan(h, m, s);
            await conn.SeekAsync(span);
        }



    }
}
