using Discord;
using Discord.WebSocket;
using DobakBot.Controller.Controller;
using DobakBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Controller
{
    class ButtonHandler
    {
        private DBController DB = BotController.Instance.DB;
        private WeaponPayController WeaponPay = BotController.Instance.WeaponPay;


        public ButtonHandler(DiscordSocketClient client)
        {
            client.ButtonExecuted += Client_ButtonExecuted;
        }

        private async Task Client_ButtonExecuted(SocketMessageComponent arg)
        {
            switch (arg.Data.CustomId)
            {
                case "casino_enter": await OnEnterButton(arg); return;
                case "Weapon_Suply": await OnWeaponButton(arg, WeaponPayKind.supply); return;
                case "Weapon_Sell": await OnWeaponButton(arg, WeaponPayKind.Sell); return;
                case "Weapon_DCSell": await OnWeaponButton(arg, WeaponPayKind.DCSell); return;
                case "Weapon_Cancel": await OnWeaponCancelButton(arg); return;
                case "customer_Wallet": await OnCustomerWalletButton(arg); return;
                case "customer_pay": await OnCustomerPayButton(arg); return;
                case "customer_return": await OnCustomerReturnButton(arg); return;
                case "dealer_accept": await OnDealerAcceptButton(arg); return;
                case "dealer_deny": await OnDealerDenyButton(arg); return;
                case "slot_roomCreate": await OnSlotRoomCreateButton(arg); return;
                case "slot_run": await OnSlotRunButton(arg); return;
                case "slot_odd": await OnSlotOddButton(arg); return;
                default: return;
            }

        }

        private async Task OnSlotOddButton(SocketMessageComponent arg)
        {
            const string OddText =
           ":shinto_shrine:　 :shinto_shrine:　 :shinto_shrine:  =　 BET X 8\n\n" +
           ":grapes:　 :grapes:　 :grapes:  =　 BET X 13\n\n" +
           ":tangerine:　 :tangerine:　 :tangerine:  =　 BET X 16\n\n" +
           ":bell:　 :bell:　 :bell:  =　 BET X 10\n\n" +
           ":flower_playing_cards:　 :flower_playing_cards:　 :flower_playing_cards:  =　 BET X 20\n\n" +
           ":cherries:　 　 　 　      =　  BET X 2\n\n" +
           ":cherries:　 :cherries:　 　     =　  BET X 5\n\n" +
           ":cherries:　 :cherries:　 :cherries:  =　 BET X 10";
            var builder = new EmbedBuilder();
            builder.Title = "파칭코 배율";
            builder.Description = OddText;
            builder.Color = Color.Orange;
            await arg.RespondAsync(embed: builder.Build(), ephemeral:true);
        }

        private async Task OnSlotRunButton(SocketMessageComponent arg)
        {
            var comp = new ComponentBuilder().WithSelectMenu(GetMoneySelectMenu("slot_run"));
            await arg.RespondAsync($"베팅 금액을 선택해 주세요.", components: comp.Build(), ephemeral: true);
        }

        private async Task OnSlotRoomCreateButton(SocketMessageComponent arg)
        {
            var channel = arg.Channel as SocketTextChannel;
            var guild = channel.Guild;
            var nick = guild.GetUser(arg.User.Id).Nickname;
            var roomName = $"{nick}님의_슬롯머신";
            if (guild.Channels.SingleOrDefault(x=> x.Name == roomName) != null)
            {
                await arg.RespondAsync($"@{roomName} 이미 만들어진 방이네요!", ephemeral: true);
                return;
            }
            var cate = guild.CategoryChannels.Single(x => x.Name == "Yamaguchi Kuma Slot");
            var ch = await guild.CreateTextChannelAsync(roomName, x => x.CategoryId = cate.Id);
            var dealerPer = guild.Roles.Single(x => x.Name == "CASINO Dealer");
            var guestPer = guild.Roles.Single(x => x.Name == "CASINO Guest");
            var per = new OverwritePermissions(viewChannel: PermValue.Deny, sendMessages: PermValue.Deny);
            var userPer = new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Deny);
            await ch.AddPermissionOverwriteAsync(guild.EveryoneRole, per);
            await ch.AddPermissionOverwriteAsync(arg.User, userPer);
            await ch.AddPermissionOverwriteAsync(dealerPer, userPer);
            await ch.AddPermissionOverwriteAsync(guestPer, userPer);

            var comp = new ComponentBuilder()
                .WithButton("슬롯머신 돌리기", "slot_run", style:ButtonStyle.Primary)
                .WithButton("슬롯머신 배율 보기", "slot_odd", style:ButtonStyle.Danger)
                .WithButton("지갑보기", "customer_Wallet", style:ButtonStyle.Success);
            var embed = new EmbedBuilder();
            embed.Color = Color.Blue;
            embed.Title = "슬롯머신 도우미";
            embed.Description = $"환영합니다 {nick}님.";
            await ch.SendMessageAsync(embed: embed.Build(), components: comp.Build());
            await arg.DeferAsync();
        }

        private async Task OnDealerAcceptButton(SocketMessageComponent arg)
        {
            var channel = arg.Channel as SocketTextChannel;
            var guild = channel.Guild;
            var nc = guild.Channels.Single(x => x.Name == "🔔｜환전-알림") as SocketTextChannel;
            var cr = CoinReceipt.fromJson(arg.Message.CleanContent);
            if (cr.IsPay)
            {
                if (!DB.TryAddUserCoin(cr.Id, cr.Money))
                {
                    await arg.RespondAsync($"TryAddUserCoin Error \nID : {cr.Id}, Money {cr.Money}");
                    return;
                }
            }
            else
            {
                if (!DB.TrySubtractUserCoin(cr.Id, cr.Money))
                {
                    await arg.RespondAsync($"TrySubtractUserCoin Error \nID : {cr.Id}, Money {cr.Money}");
                    return;
                }
            }
            var contentmsg = $"{cr.Nickname}님의 {cr.Kind}요청은 성사되었습니다. ({cr.Money}$)";
            await arg.Message.ModifyAsync(msg => {
                msg.Components = new ComponentBuilder().Build();
                msg.Content = contentmsg;
            });
            await nc.SendMessageAsync(contentmsg);
        }

        private async Task OnDealerDenyButton(SocketMessageComponent arg)
        {
            var channel = arg.Channel as SocketTextChannel;
            var guild = channel.Guild;
            var nc = guild.Channels.Single(x => x.Name == "🔔｜환전-알림") as SocketTextChannel;
            var cr = CoinReceipt.fromJson(arg.Message.CleanContent);
            var contentmsg = $"{cr.Nickname}님의 {cr.Kind}요청은 취소됫습니다.";
            await arg.Message.ModifyAsync(msg => {
                msg.Components = new ComponentBuilder().Build();
                msg.Content = contentmsg;
            });
            await nc.SendMessageAsync(contentmsg);
        }

        private async Task OnCustomerReturnButton(SocketMessageComponent arg)
        {
            var comp = new ComponentBuilder().WithSelectMenu(GetMoneySelectMenu("customerreturn_select"));
            await arg.RespondAsync($"환전할 금액을 선택해주세요.", components: comp.Build(), ephemeral: true);
        }

        private async Task OnCustomerPayButton(SocketMessageComponent arg)
        {
            var comp = new ComponentBuilder().WithSelectMenu(GetMoneySelectMenu("customerpay_select"));
            await arg.RespondAsync($"충전할 금액을 선택해주세요.", components: comp.Build(), ephemeral: true);
        }

        private SelectMenuBuilder GetMoneySelectMenu(string id)
        {
            var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("금액 선택")
            .WithCustomId(id)
            .WithMinValues(1)
            .WithMaxValues(1);
            for (int i = 1; i < 21; i++)
            {
                var item = (i * 1000).ToString();
                menuBuilder.AddOption(item, item);
            }
            return menuBuilder;
        }
        private async Task OnCustomerWalletButton(SocketMessageComponent arg)
        {
            var user = DB.GetUserByDiscordId(arg.User.Id);
            if (user == null)
            {
                await arg.RespondAsync($"등록되지 않은 사용자입니다.", ephemeral: true);
                return;
            }
            var channel = arg.Channel as SocketTextChannel;
            var guild = channel.Guild;
            var nick = guild.GetUser(user.id).Nickname;
            await arg.RespondAsync($"{nick}님의 현재 남은:coin:은 {user.coin}:coin: 입니다.", ephemeral: true);
        }

        private async Task OnWeaponCancelButton(SocketMessageComponent arg)
        {
            WeaponPay temp;
            WeaponPay.WeaponPayMap.TryRemove(arg.User.Id, out temp);
            try
            {
                await arg.Message.DeleteAsync();
            }
            catch { }
            finally
            {
                await arg.RespondAsync($"장부를 취소 하셨습니다.", ephemeral: true);
            }
        }

        private async Task OnWeaponButton(SocketMessageComponent arg, WeaponPayKind kind)
        {
            await arg.Message.DeleteAsync();
            var id = arg.User.Id;
            if (!WeaponPay.WeaponPayMap.ContainsKey(id))
            {
                await arg.RespondAsync($"장부 도우미를 한번더 불려와 주세요!\n장부도우미 부르기 : !장부 무기갯수 (!장부 1)", ephemeral: true);
                return;
            }
            WeaponPay.WeaponPayMap[id].Kind = kind;
            var list = WeaponPay.WeaponPayMap[id].Weapons;
            var comp = new ComponentBuilder().WithSelectMenu(GetWeponSelectMenu(list));
            comp.WithButton(label: "취소", customId: "Weapon_Cancel", row:1);
            await arg.RespondAsync($"무기 또는 탄창을 하나만 선택해주세요. \n 선택후, 이메세지를 닫는것을 추천합니다.",components:comp.Build() ,ephemeral: true);

        }

        private SelectMenuBuilder GetWeponSelectMenu(List<Weapon> weapons)
        {
            var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("장비 선택")
            .WithCustomId("WeaponPay_SelectMenu")
            .WithMinValues(1)
            .WithMaxValues(1);
            weapons.ForEach(item => menuBuilder.AddOption(item.Name, item.Name));
            return menuBuilder;
        }

        private async Task OnEnterButton(SocketMessageComponent arg)
        {
            var channel = arg.Channel as SocketTextChannel;
            var guild = channel.Guild;
            var user = guild.GetUser(arg.User.Id);
            var role = guild.Roles.Single(x => x.Name == "CASINO Guest");

            if (user.Nickname == null || user.Nickname == string.Empty)
            {
                await arg.RespondAsync($"디스코드 채널의 별명을 IC상의 이름으로 설정해주세요!", ephemeral: true);
                return;
            }

            if (user.Roles.Contains(role))
            {
                await arg.RespondAsync($"이미 입장하신분이네요!", ephemeral: true);
                return;
            }

            if(!DB.InsertUser(arg.User.Id))
            {
                await channel.SendMessageAsync($"InsertUser => DB에러");
                return;
            }
            await user.AddRoleAsync(role);
            await arg.DeferAsync();
        }
    }
}
