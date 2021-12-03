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
            text += "파칭코 : 파칭코 채널을 입장후 !파칭코 배팅금액 (ex : !파칭코 1000)\n";
            text += "경마 : 경마 채널을 입장후 경마의 베팅이 시작 됫을때 !경마 베팅 베팅이름 베팅금액 (ex : !경마 토끼 1000)\n";
            text += "지갑보기 : 카지노의 아무 채널에서 !지갑\n";
            await ReplyAsync(text);
        }

        [Command("지갑")]
        public async Task GetuserCommand()
        {
            var user = DB.GetUserByDiscordId(Context.User.Id);
            if (user == null)
            {
                await ReplyAsync($"등록되지 않은 사용자입니다.");
                return;
            }
            var nick = Context.Guild.GetUser(user.id).Nickname;
            await ReplyAsync($"{nick}님의 현재 남은:coin:은 {user.coin}:coin: 입니다.");
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
            if (money > 1000)
            {
                await ReplyAsync($"베팅 최대 금액을 초과 했습니다. (베팅최대금액 : 1000:coin:)");
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

            DB.TrySubtractUserCoin(Context.User.Id, money);
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
