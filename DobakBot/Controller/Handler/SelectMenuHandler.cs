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
        private AnimalRaceController AnimalRace = BotController.Instance.animalRace;

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
                case "slot_run": await OnSlotRun(arg); return;
                case "weapon_remove": await OnWeaponRemove(arg); return;
                case "race_bet": await OnRaceBet(arg); return;
                default: return;
            }
        }

        private async Task OnRaceBet(SocketMessageComponent arg)
        {
            var data = arg.Data.Values.First();
            foreach (var item in AnimalRace.Bettings.Values)
            {
                if (item.Any(x=>x.ID== arg.User.Id))
                {
                    await arg.RespondAsync($"베팅은 한번만 가능합니다.", ephemeral: true);
                    return;
                }
            }
            var user = DB.GetUserByDiscordId(arg.User.Id);
            if (user == null)
            {
                await arg.RespondAsync($"등록되지 않은 사용자입니다.", ephemeral: true);
                return;
            }
            var mb = new ModalBuilder()
               .WithTitle($"{data} 베팅 | 현재 보유 코인 : {user.coin}")
               .WithCustomId("race_bet")
               .AddTextInput("베팅 금액", data, placeholder: "숫자만 입력", required: true);
            await arg.RespondWithModalAsync(mb.Build());
        }

        private async Task OnWeaponRemove(SocketMessageComponent arg)
        {
            var msg = await arg.Channel.GetMessageAsync((ulong)WeaponPay.MessageId);
            if (msg == null)
            {
                await arg.RespondAsync("DB를 찾을수 없음", ephemeral: true);
                return;
            }
            var wps = Weapon.ListFromJson(msg.Content);
            var data = arg.Data.Values.First();
            wps.Remove(wps.Single(x => x.Name == data));
            await arg.Channel.ModifyMessageAsync((ulong)WeaponPay.MessageId, x => x.Content = Weapon.ListToJson(wps));
            await arg.RespondAsync("제거 성공", ephemeral: true);
        }

        private async Task OnSlotRun(SocketMessageComponent arg)
        {
            var channel = arg.Channel as SocketTextChannel;
            var guild = channel.Guild;
            await arg.Message.DeleteAsync();
            var msg = await channel.GetMessagesAsync(limit: 100).SingleAsync();
            if (msg.Count > 1)
            {
                await channel.DeleteMessageAsync(msg.First());
            }

            var nick = guild.GetUser(arg.User.Id).Nickname;
            int money;
            if (!int.TryParse(arg.Data.Values.First(), out money))
            {
                await arg.RespondAsync($"{arg} 일치 하지 않는 값입니다.\n", ephemeral:true);
                return;
            }

            var user = DB.GetUserByDiscordId(arg.User.Id);
            if (user == null)
            {
                await arg.RespondAsync($"등록되지 않은 사용자입니다.", ephemeral: true);
                return;
            }
            if (user.coin < money)
            {
                await arg.RespondAsync($"{nick}님의 :coin:이 부족합니다 ({user.coin - money}:coin:).", ephemeral: true);
                return;
            }
            DB.TrySubtractUserCoin(arg.User.Id, money);
            _ = RunSlotMachine(money, nick, arg);
        }

        private async Task RunSlotMachine(int money, string nick , SocketMessageComponent arg)
        {
            var slot = new SlotMachine(money);
            await slot.SetResult();
            var embeds = slot.getEmbeds(nick);
            var msg = await arg.Channel.SendMessageAsync(embed:embeds[0]);
            await Task.Delay(2000);
            await msg.ModifyAsync(msg => msg.Embed = embeds[1]);
            await Task.Delay(1010);
            await msg.ModifyAsync(msg => msg.Embed = embeds[2]);
            await Task.Delay(1010);
            await msg.ModifyAsync(msg => msg.Embed = embeds[3]);
            if (slot.ResultCoin == 0) return;
            if (slot.SlotResult == SlotResult.JackPot)
            {
                var nc = (arg.Channel as SocketTextChannel).Guild.Channels.Single(x => x.Name == "🥇｜명예의전당") as SocketTextChannel;
                _ = nc.SendMessageAsync(embed: embeds[3]);
            }
            DB.TryAddUserCoin(arg.User.Id, slot.ResultCoin);
        }

        private async Task OnCustomerReturnSelectMenu(SocketMessageComponent arg)
        {
            var channel = arg.Channel as SocketTextChannel;
            var guild = channel.Guild;
            var user = arg.User.Id;
            var nick = guild.GetUser(user).Nickname;
            var data = arg.Data.Values.First();

            var dbuser = DB.GetUserByDiscordId(user);
            if (dbuser == null)
            {
                await arg.RespondAsync($"{nick} 등록되지 않은 사용자입니다.", ephemeral: true);
                return;
            }

            int money;
            if (data == "all") money = dbuser.coin;
            else if (!int.TryParse(data, out money))
            {
                await arg.RespondAsync($"{data} 일치 하지 않는 값입니다.", ephemeral: true);
                return;
            }

            if (dbuser.coin < money)
            {
                await arg.RespondAsync($"{nick}님의 :coin:이 부족하여 환전이 불가능합니다.\n" +
                    $"(현재 잔액 : {dbuser.coin}:coin:) (요청금액 : {money}:coin:).", ephemeral: true);
                return;
            }
            var notifiyChannel = guild.GetCategoryChannel((ulong)channel.CategoryId).Channels.Single(x => x.Name == "딜러-사무실") as SocketTextChannel;
            var Cr = new CoinReceipt(nick, money, user, false, tip:(int)(money * 0.1));
            var msg = CoinReceipt.toJson(Cr);
            var comp = new ComponentBuilder().WithButton("승인", "dealer_accept").WithButton("거부", "dealer_deny");
            await notifiyChannel.SendMessageAsync(msg, components: comp.Build());
            await arg.RespondAsync($"요청 되었습니다! 딜러의 확인까지 기달려주세요.", ephemeral: true);
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

            var notifiyChannel = guild.GetCategoryChannel((ulong)channel.CategoryId).Channels.Single(x => x.Name == "딜러-사무실") as SocketTextChannel;
            var Cr = new CoinReceipt(nick, money, user, true);
            var msg = CoinReceipt.toJson(Cr);
            var comp = new ComponentBuilder().WithButton("승인", "dealer_accept").WithButton("거부", "dealer_deny");
            await notifiyChannel.SendMessageAsync(msg, components: comp.Build());
            await arg.RespondAsync($"요청 되었습니다! 딜러의 확인까지 기달려주세요.", ephemeral: true);
        }

        private async Task OnWeaponPaySelectMenu(SocketMessageComponent arg)
        {
            var data = arg.Data.Values.First();
            var nf = (arg.Channel as SocketTextChannel).Guild.Channels.Single(x => x.Name == "📖ㅣ총기지급대장") as SocketTextChannel;
            WeaponPay ctx;
            if (!WeaponPay.WeaponPayMap.TryRemove(arg.User.Id, out ctx))
            {
                await arg.RespondAsync($"유저를 찾을수 없네요.. 한번 더 실행해주세요.", ephemeral: true);
                return;
            }

            var Weapon = ctx.Weapons.SingleOrDefault(x => x.Name == data);
            if (Weapon == null)
            {
                await arg.RespondAsync($"DB 오류...", ephemeral: true);
                return;
            }
            ctx.Weapon = Weapon;
            ctx.UserName = (arg.User as IGuildUser).Nickname;

            var builder = new EmbedBuilder();
            builder.Description = ctx.ToString();
            builder.Color = Color.Green;

            await nf.SendMessageAsync(embed: builder.Build());
            await arg.DeferAsync();
        }
    }
}
