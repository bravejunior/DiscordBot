using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stella
{

    public class Commands : BaseCommandModule
    {

        [Command("ping")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.RespondAsync("Pong!");
        }

        [Command("dnd")]
        public async Task Dnd(CommandContext ctx)
        {
            await ctx.RespondAsync("@everyone It's time for D&D you WHOOOOOOOOOOOOOOOORES!");
        }

        [Command]
        public async Task Join(CommandContext ctx)
        {

            var lava = ctx.Client.GetLavalink();

            bool isDm = ctx.Channel.IsPrivate;

            if (isDm)
            {
                await ctx.RespondAsync("You can't do this in a DM, have to be in a server :/");
                return;
            }


            var state = ctx.Member.VoiceState;

            if (state == null)
            {
                await ctx.RespondAsync("You have to be in a voice channel in order for me to join, I'm so terribly sorry.");
                return;
            }

            var channel = ctx.Member.VoiceState.Channel;

            if (!lava.ConnectedNodes.Any())
            {
                await ctx.RespondAsync("The Lavalink connection is not established");
                return;
            }

            var node = lava.ConnectedNodes.Values.First();

            if (channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Not a valid voice channel.");
                return;
            }

            await node.ConnectAsync(channel);
            await ctx.RespondAsync($"Joined {channel.Name}!");
        }

        [Command]
        public async Task Leave(CommandContext ctx)
        {
            bool isDm = ctx.Channel.IsPrivate;

            if (isDm)
            {
                await ctx.RespondAsync("You can't do this in a DM, have to be in a server :/");
                return;
            }

            var state = ctx.Member.VoiceState;

            if (state == null)
            {
                await ctx.RespondAsync("You have to be in my channel you want me to leave, I'm so sorry.");
                return;
            }

            var channel = ctx.Member.VoiceState.Channel;

            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.RespondAsync("The Lavalink connection is not established");
                return;
            }

            var node = lava.ConnectedNodes.Values.First();

            if (channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Not a valid voice channel.");
                return;
            }

            var conn = node.GetGuildConnection(channel.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            await conn.DisconnectAsync();
            await ctx.RespondAsync($"Left {channel.Name}!");

        }

        [Command]
        public async Task Play(CommandContext ctx, Uri url)
        {
            bool isDm = ctx.Channel.IsPrivate;

            if (isDm)
            {
                await ctx.RespondAsync("You can't do this in a DM, have to be in a server :/");
                return;
            }

            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("I can't play :(");
                return;
            }

            var loadResult = await node.Rest.GetTracksAsync(url);

            var track = loadResult.Tracks.First();

            await conn.PlayAsync(track);

            await ctx.RespondAsync($"Now playing {track.Title}!");

        }

        //[Command]
        //public async Task Play(CommandContext ctx, [RemainingText] string search)
        //{

        //    if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
        //    {
        //        await ctx.RespondAsync("You are not in a voice channel.");
        //        return;
        //    }

        //    var lava = ctx.Client.GetLavalink();
        //    var node = lava.ConnectedNodes.Values.First();
        //    var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

        //    if (conn == null)
        //    {
        //        await ctx.RespondAsync("Lavalink is not connected.");
        //        return;
        //    }


        //    var loadResult = await node.Rest.GetTracksAsync(search);

        //    //If something went wrong on Lavalink's end                          
        //    if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed

        //        //or it just couldn't find anything.
        //        || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
        //    {
        //        await ctx.RespondAsync($"Track search failed for {search}.");
        //        return;
        //    }

        //    var track = loadResult.Tracks.First();

        //    await conn.PlayAsync(track);

        //    await ctx.RespondAsync($"Now playing {track.Title}!");

        //}

        [Command]
        public async Task Pause(CommandContext ctx)
        {
            bool isDm = ctx.Channel.IsPrivate;

            if (isDm)
            {
                await ctx.RespondAsync("You can't do this in a DM, have to be in a server :/");
                return;
            }
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("There are no tracks loaded.");
                return;
            }

            await conn.PauseAsync();
        }

        //[Command]
        //public async Task SayBitch(CommandContext ctx)
        //{
        //    bool isDm = ctx.Channel.IsPrivate;

        //    if (isDm)
        //    {
        //        await ctx.RespondAsync("You can't do this in a DM, have to be in a server :/");
        //        return;
        //    }
        //    if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
        //    {
        //        await ctx.RespondAsync("You are not in a voice channel.");
        //        return;
        //    }

        //    var lava = ctx.Client.GetLavalink();
        //    var node = lava.ConnectedNodes.Values.First();
        //    var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

        //    if (conn == null)
        //    {
        //        await ctx.RespondAsync("Lavalink is not connected.");
        //        return;
        //    }

        //    if (conn.CurrentState.CurrentTrack == null)
        //    {
        //        await ctx.RespondAsync("There are no tracks loaded.");
        //        return;
        //    }

        //    await conn.PauseAsync();
        //}

    }

}