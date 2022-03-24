using Discord;
using Discord.WebSocket;
using DobakBot.Controller.Controller;
using DobakBot.Model;
using DobakBot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Controller.Handler
{
    class SelectMenuHandler
    {
        private WeaponPayController WeaponPay = BotController.Instance.WeaponPay;
        private DBController DB = BotController.Instance.DB;

        public SelectMenuHandler(DiscordSocketClient client)
        {
            client.SelectMenuExecuted += Client_SelectMenuExecuted;
        }

        private async Task Client_SelectMenuExecuted(SocketMessageComponent arg)
        {
            switch (arg.Data.CustomId)
            {
                case "WeaponPay_SelectMenu": await OnWeaponPaySelectMenu(arg); return;
                case "customerpay_select": await OnCustomerPaySelectMenu(arg); return;
                case "customerreturn_select": await OnCustomerReturnSelectMenu(arg); return;
                default: return;
            }
        }

        private async Task OnCustomerReturnSelectMenu(SocketMessageComponent arg)
        {

            var channel = arg.Channel as SocketTextChannel;
            var guild = channel.Guild;
            var user = arg.User.Id;
            var nick = guild.GetUser(user).Nickname;
            var data = arg.Data.Values.First();

            int money;
            if (!int.TryParse(data, out money))
            {
                await arg.RespondAsync($"{data} 일치 하지 않는 값입니다.", ephemeral: true);
                return;
            }

            var dbuser = DB.GetUserByDiscordId(user);
            if (dbuser == null)
            {
                await arg.RespondAsync($"{nick} 등록되지 않은 사용자입니다.", ephemeral: true);
                return;
            }

            if (dbuser.coin < money)
            {
                await arg.RespondAsync($"{nick}님의 :coin:이 부족하여 환전이 불가능합니다.\n" +
                    $"(현재 잔액 : {dbuser.coin}:coin:) (요청금액 : {money}:coin:).", ephemeral: true);
                return;
            }

            var notifiyChannel = guild.Channels.Single(x => x.Name == "딜러-사무실") as SocketTextChannel;
            var Cr = new CoinReceipt(nick, money, user, false);
            var msg = CoinReceipt.toJson(Cr);
            var comp = new ComponentBuilder().WithButton("승인", "dealer_accept").WithButton("거부", "dealer_deny");
            await notifiyChannel.SendMessageAsync(msg, component: comp.Build());

            if (!DB.TrySubtractUserCoin(user, money))
            {
                await arg.RespondAsync($"TrySubtractUserCoin Error", ephemeral: true);
                return;
            }

            await arg.RespondAsync($"{nick}님이 {money}$ 환전하셨습니다.", ephemeral: true);
        }

        private async Task OnCustomerPaySelectMenu(SocketMessageComponent arg)
        {
            var channel = arg.Channel as SocketTextChannel;
            var guild = channel.Guild;
            var data = arg.Data.Values.First();
            await guild.DownloadUsersAsync();
            var user = arg.User.Id;
            var nick = guild.GetUser(user).Nickname;
            int money;
            if (!int.TryParse(data, out money))
            {
                await arg.RespondAsync($"{data} 일치 하지 않는 값입니다.\n", ephemeral: true);
                return;
            }

            var notifiyChannel = guild.Channels.Single(x => x.Name == "딜러-사무실") as SocketTextChannel;
            var Cr = new CoinReceipt(nick, money, user, true);
            var msg = CoinReceipt.toJson(Cr);
            var comp = new ComponentBuilder().WithButton("승인", "dealer_accept").WithButton("거부", "dealer_deny");
            await notifiyChannel.SendMessageAsync(msg, component: comp.Build());

            if (!DB.TryAddUserCoin(user, money))
            {
                await arg.RespondAsync($"TryAddUserCoin Error", ephemeral: true);
                return;
            }

            await arg.RespondAsync($"{nick}님이 {money}:coin: 충전하셨습니다.", ephemeral: true);
        }

        private async Task OnWeaponPaySelectMenu(SocketMessageComponent arg)
        {
            var data = arg.Data.Values.First();

            WeaponPay ctx;
            if (!WeaponPay.WeaponPayMap.TryRemove(arg.User.Id, out ctx))
            {
                await arg.RespondAsync($"장부 도우미를 한번더 불려와 주세요!\n장부도우미 부르기 : !장부 무기갯수 (!장부 1)", ephemeral: true);
                return;
            }

            var Weapon = ctx.Weapons.SingleOrDefault(x => x.Name == data);
            if (Weapon == null)
            {
                await arg.RespondAsync($"장부 도우미를 한번더 불려와 주세요!\n장부도우미 부르기 : !장부 무기갯수 (!장부 1)", ephemeral: true);
                return;
            }


            ctx.Weapon = Weapon;
            ctx.UserName = (arg.User as IGuildUser).Nickname;

            var builder = new EmbedBuilder();
            builder.Description = ctx.ToString();
            builder.Color = Color.Green;

            await arg.Channel.SendMessageAsync(embed: builder.Build());
            await arg.DeferAsync();
        }
    }
}
