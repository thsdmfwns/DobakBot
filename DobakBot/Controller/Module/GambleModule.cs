using Discord;
using Discord.Commands;
using Discord.Rest;
using DobakBot.Controller;
using DobakBot.Controller.Attribute;
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
        private DBController DB = GambleController.Instance.DB;
        [Command("help")]
        public async Task HelpCommand()
        {
            string text = string.Empty;
            text += "!파칭코 배팅금액 (ex : !파칭코 1000)\n";
            text += "!경마\n";
            await ReplyAsync(text);
        }

        [RequireChannel("파칭코")]
        [Command("파칭코")]
        public async Task SlotMachineCommand([Remainder] string arg)
        {
            int money;
            if (!int.TryParse(arg, out money))
            {
                await ReplyAsync($"{arg} 일치 하지 않는 값입니다.\n" + "!파칭코 배팅금액 (ex : !파칭코 1000)");
                return;
            }

            var user = DB.GetUserByDiscordId(Context.User.Id);
            if (user == null)
            {
                await ReplyAsync($"등록되지 않은 사용자입니다.");
                return;
            }
            if (user.coin < money)
            {
                await ReplyAsync($"{Context.Guild.GetUser(Context.User.Id).Nickname}님의 :coin:이 부족합니다 ({user.coin - money}:coin:).");
                return;
            }

            _ = RunSlotMachine(money);
        }

        private async Task RunSlotMachine(int money)
        {
            var slot = new SlotMachine(money);
            await slot.setValue();
            DB.TryAddUserCoin(Context.User.Id, slot.Coin);

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
