using Discord;
using Discord.Commands;
using Discord.Rest;
using DobakBot.Controller;
using DobakBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DobakBot
{
    public class GambleModule : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task HelpCommand()
        {
            string text = string.Empty;
            text += "!파칭코\n";
            text += "베팅시작 : !베팅시작 별명#이모티콘 별명#이모티콘 (ex : !베팅시작 토끼#:rabbit2: 거북이#:turtle:)\n";
            text += "베팅 : !베팅 베팅대상 베팅금액 (ex : !베팅 토끼 100) \n";
            text += "경마시작 : !경마시작\n";
            await ReplyAsync(text);
        }

        [Command("파칭코")]
        public async Task SlotMachineCommand()
        {
            _ = RunSlotMachine();
        }

        private async Task RunSlotMachine()
        {
            var slot = new SlotMachine();
            await slot.setValue();
            var embeds = slot.getEmbeds(Context.Guild.GetUser(Context.User.Id).Nickname);
            var msg = await Context.Channel.SendMessageAsync("", false, embeds[0]);
            await Task.Delay(2000);
            await msg.ModifyAsync(msg => msg.Embed = embeds[1]);
            await Task.Delay(1010);
            await msg.ModifyAsync(msg => msg.Embed = embeds[2]);
            await Task.Delay(1010);
            await msg.ModifyAsync(msg => msg.Embed = embeds[3]);
            
        }
    }
}
