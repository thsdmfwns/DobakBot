﻿using Discord;
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
                case "Weapon_Cancel": await OnWeaponCancelButton(arg); return;
                case "customer_Wallet": await OnCustomerWalletButton(arg); return;
                case "customer_pay": await OnCustomerPayButton(arg); return;
                case "customer_return": await OnCustomerReturnButton(arg); return;
                case "dealer_accept": await OnDealerAcceptButton(arg); return;
                case "dealer_deny": await OnDealerDenyButton(arg); return;
                case "slot_roomCreate": await OnSlotRoomCreateButton(arg); return;
                case "slot_run": await OnSlotRunButton(arg); return;
                case "slot_odd": await OnSlotOddButton(arg); return;
                case "info_create": await OnInfoRoomCreateButton(arg); return;
                case "weapon_add": await OnWeaponAdd(arg); return;
                case "weapon_remove": await OnWeaponRemove(arg); return;
                case "weapon_apply": await OnWeaponApply(arg); return;
                case "weaponpay_supply": await OnWeaponPay(arg); return;
                case "weaponpay_sell": await OnWeaponPay(arg, isSell:true); return;
                default: return;
            }

        }

        private async Task OnWeaponPay(SocketMessageComponent arg, bool isSell = false)
        {
            if (WeaponPay.ChannelId == null || WeaponPay.MessageId == null)
            {
                await arg.RespondAsync($"DB를 찾을수 없음. 갱신부탁", ephemeral: true);
                return;
            }

            var kind = isSell ? WeaponPayKind.Sell : WeaponPayKind.supply;
            WeaponPay.WeaponPayMap.AddOrUpdate(arg.User.Id, new WeaponPay() { Kind = kind }, (key, oldval) => oldval = new WeaponPay() { Kind = kind });
            var mb = new ModalBuilder()
            .WithTitle("무기 갯수")
            .WithCustomId("weaponpay_count")
            .AddTextInput("갯수", "count", placeholder: "숫자만 입력!", required: true)
            .AddTextInput("소비자 이름", "name", placeholder: "ex) Boggu_Lee (비울시 자기 닉네임)");
            await arg.RespondWithModalAsync(mb.Build());
        }

        private async Task OnWeaponApply(SocketMessageComponent arg)
        {
            WeaponPay.MessageId = arg.Message.Id;
            WeaponPay.ChannelId = arg.Channel.Id;
            await arg.DeferAsync();
        }

        private async Task OnWeaponRemove(SocketMessageComponent arg)
        {
            if (arg.Message.Content == null || arg.Message.Content == "empty")
            {
                await arg.RespondAsync($"텅텅비었네요.", ephemeral: true);
                return;
            }
            WeaponPay.MessageId = arg.Message.Id;
            WeaponPay.ChannelId = arg.Channel.Id;
            var weapons = Weapon.ListFromJson(arg.Message.Content);

            var menuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("무기 선택")
                .WithCustomId("weapon_remove")
                .WithMinValues(1)
                .WithMaxValues(1);
            foreach (var weapon in weapons)
            {
                menuBuilder.AddOption(weapon.Name, weapon.Name);
            }

            var cb = new ComponentBuilder().WithSelectMenu(menuBuilder);
            await arg.RespondAsync("무기선택", components:cb.Build(), ephemeral:true);
        }

        private async Task OnWeaponAdd(SocketMessageComponent arg)
        {
            WeaponPay.MessageId = arg.Message.Id;
            var mb = new ModalBuilder()
            .WithTitle("무기 추가")
            .WithCustomId("weapon_add")
            .AddTextInput("무기 이름", "weapon_name", placeholder: "ex) 글락", required : true)
            .AddTextInput("보급 가격", "weapon_price", placeholder:"ex) 2000 (숫자만 입력하세요!)", required : true)
            .AddTextInput("판매 가격", "weapon_sellprice", placeholder:"ex) 2000 (숫자만 입력하세요!)", required : true)
            .AddTextInput("단위", "weapon_unit", placeholder:"ex) 개, 정, 복", required : true);
            await arg.RespondWithModalAsync(mb.Build());
        }

        private async Task OnSlotOddButton(SocketMessageComponent arg)
        {
            const string OddText =
           ":shinto_shrine:　 :shinto_shrine:　 :shinto_shrine:  =　 BET X 4\n\n" +
           ":grapes:　 :grapes:　 :grapes:  =　 BET X 6\n\n" +
           ":tangerine:　 :tangerine:　 :tangerine:  =　 BET X 8\n\n" +
           ":bell:　 :bell:　 :bell:  =　 BET X 10\n\n" +
           ":flower_playing_cards:　 :flower_playing_cards:　 :flower_playing_cards:  =　 BET X 12\n\n" +
           ":cherries:　 　　 　     =　  BET X 1.3\n\n" +
           ":cherries:　 :cherries:　 　     =　  BET X 2\n\n" +
           ":cherries:　 :cherries:　 :cherries:  =　 BET X 3";
            var builder = new EmbedBuilder();
            builder.Title = "파칭코 배율";
            builder.Description = OddText;
            builder.Color = Color.Orange;
            await arg.RespondAsync(embed: builder.Build(), ephemeral:true);
        }

        private async Task OnSlotRunButton(SocketMessageComponent arg)
        {
            var comp = new ComponentBuilder().WithSelectMenu(GetMoneySelectMenu("slot_run"));
            await arg.RespondAsync($"베팅 금액을 선택해 주세요.", components: comp.Build());
        }

        private async Task OnSlotRoomCreateButton(SocketMessageComponent arg)
        {
            var channel = arg.Channel as SocketTextChannel;
            var guild = channel.Guild;
            var nick = guild.GetUser(arg.User.Id).Nickname;
            var roomName = $"🎰｜{nick.ToLower()}";
            var cate = guild.CategoryChannels.Single(x => x.Id == channel.CategoryId);
            var temp = cate.Channels.SingleOrDefault(x => x.Name == roomName);
            if (temp != null)
            {
                await arg.RespondAsync($"{MentionUtils.MentionChannel(temp.Id)} 이미 만들어진 방이네요!", ephemeral: true);
                return;
            }
            var ch = await guild.CreateTextChannelAsync(roomName, x => x.CategoryId = cate.Id);
            var dealerPer = guild.Roles.Single(x => x.Name == "CASINO Dealer");
            var guestPer = guild.Roles.Single(x => x.Name == "CASINO Guest");
            var per = new OverwritePermissions(viewChannel: PermValue.Deny, sendMessages: PermValue.Deny);
            var userPer = new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Deny);
            await ch.AddPermissionOverwriteAsync(guild.EveryoneRole, per);
            await ch.AddPermissionOverwriteAsync(arg.User, userPer);
            await ch.AddPermissionOverwriteAsync(dealerPer, userPer);
            await ch.AddPermissionOverwriteAsync(guestPer, per);

            var comp = new ComponentBuilder()
                .WithButton("슬롯머신 돌리기", "slot_run", style:ButtonStyle.Primary)
                .WithButton("슬롯머신 배율 보기", "slot_odd", style:ButtonStyle.Danger)
                .WithButton("지갑보기", "customer_Wallet", style:ButtonStyle.Success);
            var embed = new EmbedBuilder();
            embed.Color = Color.Blue;
            embed.Title = "슬롯머신 도우미";
            embed.Description = $"환영합니다 {MentionUtils.MentionUser(arg.User.Id)}님.";
            await ch.SendMessageAsync(embed: embed.Build(), components: comp.Build());
            await arg.DeferAsync();
        }

        private async Task OnInfoRoomCreateButton(SocketMessageComponent arg)
        {
            var channel = arg.Channel as SocketTextChannel;
            var guild = channel.Guild;
            var nick = guild.GetUser(arg.User.Id).Nickname;
            var cate = guild.CategoryChannels.Single(x => x.Id == channel.CategoryId);
            var roomName = $"📖｜{nick.ToLower()}";
            var temp = cate.Channels.SingleOrDefault(x => x.Name == roomName);
            if (temp != null)
            {
                await arg.RespondAsync($"{MentionUtils.MentionChannel(temp.Id)} 이미 만들어진 방이네요!", ephemeral: true);
                return;
            }
            var ch = await guild.CreateTextChannelAsync(roomName, x => x.CategoryId = cate.Id);
            var dealerPer = guild.Roles.Single(x => x.Name == "CASINO Dealer");
            var guestPer = guild.Roles.Single(x => x.Name == "CASINO Guest");
            var per = new OverwritePermissions(viewChannel: PermValue.Deny, sendMessages: PermValue.Deny);
            var userPer = new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow);
            await ch.AddPermissionOverwriteAsync(guild.EveryoneRole, per);
            await ch.AddPermissionOverwriteAsync(arg.User, userPer);
            await ch.AddPermissionOverwriteAsync(dealerPer, userPer);
            await ch.AddPermissionOverwriteAsync(guestPer, per);
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
            var count = cr.IsPay ? ":coin:" : "$";
            var contentmsg = $"{cr.Nickname}님의 {cr.Kind}요청은 성사되었습니다. ({cr.TotalMoney}{count})";
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
            var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("금액 선택")
            .WithCustomId("customerreturn_select")
            .WithMinValues(1)
            .WithMaxValues(1);
            menuBuilder.AddOption("잔금 전액", "all");
            for (int i = 1; i < 21; i++)
            {
                var item = (i * 500).ToString();
                menuBuilder.AddOption(item, item);
            }
            var comp = new ComponentBuilder().WithSelectMenu(menuBuilder);
            await arg.RespondAsync($"환전할 금액을 선택해주세요.", components: comp.Build(), ephemeral: true);
        }

        private async Task OnCustomerPayButton(SocketMessageComponent arg)
        {
            var comp = new ComponentBuilder().WithSelectMenu(GetMoneySelectMenu("customerpay_select"));
            await arg.RespondAsync($"충전할 금액을 선택해주세요.", components: comp.Build(), ephemeral: true);
        }

        private SelectMenuBuilder GetMoneySelectMenu(string id, int limit = 21)
        {
            var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("금액 선택")
            .WithCustomId(id)
            .WithMinValues(1)
            .WithMaxValues(1);
            for (int i = 1; i < limit; i++)
            {
                var item = (i * 500).ToString();
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
