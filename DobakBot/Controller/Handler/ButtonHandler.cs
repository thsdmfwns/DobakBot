using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DobakBot.Controller.Controller;
using DobakBot.Model;
using DobakBot.Utils;
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
        private AnimalRaceController AnimalRace = BotController.Instance.animalRace;


        public ButtonHandler(DiscordSocketClient client)
        {
            client.ButtonExecuted += Client_ButtonExecuted;
        }

        private async Task Client_ButtonExecuted(SocketMessageComponent arg)
        {
            switch (arg.Data.CustomId)
            {
                case "casino_enter": await OnEnterButton(arg); return;
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
                case "race_make": await OnRaceMake(arg); return;
                case "race_start": await OnRaceStart(arg); return;
                case "race_cancel": await OnRaceCancel(arg); return;
                default: return;
            }

        }

        private async Task OnRaceStart(SocketMessageComponent arg)
        {
            if (!AnimalRace.IsSetting)
            {
                await arg.RespondAsync("경기를 먼저 만들어 주세요.", ephemeral: true);
                return;
            }
            if (AnimalRace.TotalMoney == 0)
            {
                await arg.RespondAsync("베팅한사람이 아무도 없습니다.", ephemeral: true);
                return;
            }
            var guild = (arg.Channel as SocketTextChannel).Guild;
            var ch = guild.GetTextChannel((ulong)AnimalRace.ChannelId);
            await ch.ModifyMessageAsync((ulong)AnimalRace.BettingMsgId, x => x.Components = new ComponentBuilder().Build());
            await arg.DeferAsync();
            var winners = await RunAnimalRace(arg, ch);
            if (!DB.TryAddUsersCoin(winners)) await arg.RespondAsync("TryAddUsersCoin => DB에러");
            AnimalRace.Clear();
        }

        private async Task<BettingMembers> RunAnimalRace(SocketMessageComponent arg, SocketTextChannel channel)
        {
            BettingMembers WinnerMembers;
            var race = AnimalRace.AnimalRace;
            var msg = await channel.SendMessageAsync("", false, race.GetEmbed(isStart: true));

            while (!race.isRaceDone)
            {
                await Task.Delay(1250);
                var embed = race.GetEmbed();
                if (embed == null) break;
                await msg.ModifyAsync(msg => msg.Embed = embed);
            }

            WinnerMembers = race.WinnerMembers;

            if (race.WinnerMembers == null)
            {
                await Task.Delay(5000);
                WinnerMembers = await RunAnimalRace(arg, channel);
            }
            return WinnerMembers;
        }

        private async Task OnRaceMake(SocketMessageComponent arg)
        {
            if (AnimalRace.IsSetting)
            {
                await arg.RespondAsync("이미 경마 베팅이 시작되어 있습니다. 취소버튼을 눌러주세요.", ephemeral:true);
                return;
            }
            var mb = new ModalBuilder()
                .WithTitle("경기 생성")
                .WithCustomId("race_make")
                .AddTextInput("경기 이름", "race_name", placeholder: "ex)이봉구배 1회 경마", required: true)
                .AddTextInput("1번마 이름", "animal1_name", placeholder: "ex) 슈퍼 짱빠른 말", required: true)
                .AddTextInput("1번마 이모티콘", "animal1_emoji", placeholder: "ex) :horse_racing: (채팅에 이모티콘 치고 복붙)", required: true)
                .AddTextInput("1번마 이름", "animal2_name", placeholder: "ex) 전설의 백마", required: true)
                .AddTextInput("1번마 이모티콘", "animal2_emoji", placeholder: "ex) :horse_racing: (채팅에 이모티콘 치고 복붙)", required: true);
            await arg.RespondWithModalAsync(mb.Build());
        }

        private async Task OnRaceCancel(SocketMessageComponent arg)
        {
            var guild = (arg.Channel as SocketTextChannel).Guild;
            await (guild.GetChannel((ulong)AnimalRace.ChannelId) as SocketTextChannel).SendMessageAsync("이 경마는 취소 되었습니다.");
            AnimalRace.Clear();
            await arg.RespondAsync("취소 완료.", ephemeral: true);
        }

        private async Task OnWeaponPay(SocketMessageComponent arg, bool isSell = false)
        {
            if (WeaponPay.ChannelId == null || WeaponPay.MessageId == null)
            {
                await arg.RespondAsync($"DB를 찾을수 없음. 갱신부탁", ephemeral: true);
                return;
            }

            var kind = isSell ? WeaponPayKind.Sell : WeaponPayKind.supply;
            WeaponPay.WeaponPayMap.AddOrUpdate(arg.User.Id,
                new WeaponPay() { Kind = kind },
                (key, oldval) => oldval = new WeaponPay() { Kind = kind });
            var mb = new ModalBuilder()
            .WithTitle("무기 갯수")
            .WithCustomId("weapon_pay")
            .AddTextInput("갯수", "count", placeholder: "숫자만 입력!", required: true)
            .AddTextInput("소비자 이름", "name", placeholder: "ex) Boggu_Lee", value:(arg.User as IGuildUser).Nickname);
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
            var comp = new ComponentBuilder().WithSelectMenu(Utility.GetMoneySelectMenu("slot_run"));
            await arg.RespondAsync($"베팅 금액을 선택해 주세요.", components: comp.Build());
        }

        private async Task<RestTextChannel> makePrivateRoom(SocketMessageComponent arg, string roomName, ulong catgoryId)
        {
            var guild = (arg.Channel as SocketTextChannel).Guild;
            var ch = await guild.CreateTextChannelAsync(roomName, x => x.CategoryId = catgoryId);
            var dealerPer = guild.Roles.Single(x => x.Name == "CASINO Dealer");
            var guestPer = guild.Roles.Single(x => x.Name == "CASINO Guest");
            var denyper = new OverwritePermissions(viewChannel: PermValue.Deny, sendMessages: PermValue.Deny);
            var userPer = new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Deny);
            await ch.AddPermissionOverwriteAsync(guild.EveryoneRole, denyper);
            await ch.AddPermissionOverwriteAsync(arg.User, userPer);
            await ch.AddPermissionOverwriteAsync(dealerPer, userPer);
            await ch.AddPermissionOverwriteAsync(guestPer, denyper);
            return ch;
        }

        private async Task OnSlotRoomCreateButton(SocketMessageComponent arg)
        {
            var channel = arg.Channel as SocketTextChannel;
            var guild = channel.Guild;
            var roomName = $"🎰｜{guild.GetUser(arg.User.Id).Nickname.ToLower()}";
            var temp = guild.CategoryChannels.Single(x => x.Id == channel.CategoryId).Channels.SingleOrDefault(x => x.Name == roomName);
            if (temp != null)
            {
                await arg.RespondAsync($"{MentionUtils.MentionChannel(temp.Id)} 이미 만들어진 방이네요!", ephemeral: true);
                return;
            }
            var ch = await makePrivateRoom(arg, roomName, (ulong)channel.CategoryId);
            var comp = new ComponentBuilder()
                .WithButton("슬롯머신 돌리기", "slot_run", style: ButtonStyle.Primary)
                .WithButton("슬롯머신 배율 보기", "slot_odd", style: ButtonStyle.Danger)
                .WithButton("지갑보기", "customer_Wallet", style: ButtonStyle.Success);
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
            var catgoryCh = guild.CategoryChannels.Single(x => x.Id == channel.CategoryId);
            var roomName = $"📖｜{ guild.GetUser(arg.User.Id).Nickname.ToLower()}";
            var room = catgoryCh.Channels.SingleOrDefault(x => x.Name == roomName);
            if (room != null)
            {
                await arg.RespondAsync($"{MentionUtils.MentionChannel(room.Id)} 이미 만들어진 방이네요!", ephemeral: true);
                return;
            }
            var ch = await makePrivateRoom(arg, roomName, (ulong)channel.CategoryId);
            await arg.DeferAsync();
        }

        private async Task OnDealerAcceptButton(SocketMessageComponent arg)
        {
            var guild = (arg.Channel as SocketTextChannel).Guild;
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
            else if (!DB.TrySubtractUserCoin(cr.Id, cr.Money))
            {
                await arg.RespondAsync($"TrySubtractUserCoin Error \nID : {cr.Id}, Money {cr.Money}");
                return;
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
            var guild = (arg.Channel as SocketTextChannel).Guild;
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
            var menuBuilder = Utility.GetMoneySelectMenu("customerreturn_select");
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
            var comp = new ComponentBuilder().WithSelectMenu(Utility.GetMoneySelectMenu("customerpay_select"));
            await arg.RespondAsync($"충전할 금액을 선택해주세요.", components: comp.Build(), ephemeral: true);
        }
        private async Task OnCustomerWalletButton(SocketMessageComponent arg)
        {
            var user = DB.GetUserByDiscordId(arg.User.Id);
            if (user == null)
            {
                await arg.RespondAsync($"등록되지 않은 사용자입니다.", ephemeral: true);
                return;
            }
            var guild = (arg.Channel as SocketTextChannel).Guild;
            await arg.RespondAsync($"{guild.GetUser(user.id).Nickname}님의 현재 남은:coin:은 {user.coin}:coin: 입니다.", ephemeral: true);
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
