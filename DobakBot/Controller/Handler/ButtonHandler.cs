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
                case "sell_upload": await OnSellUpload(arg); return;
                case "sell_alwayupload": await OnALwaySellUpload(arg); return;
                case "sell_buy": await OnSellBuy(arg); return;
                case "sell_upgrade": await OnSellUpgrade(arg); return;
                case "sell_ispolice_yes": await arg.RespondAsync("우수회원 관련 기능은 아직 준비중입니다.", ephemeral: true); return;
                case "sell_ispolice_no": await OnSellIsPoliceNo(arg); return;
                case "faction_join": await OnFactionJoin(arg); return;
                case "faction_permission": await OnFactionPermisson(arg); return;
                case "faction_report": await OnFactionReport(arg); return;
                default: return;
            }

        }

        private async Task OnFactionReport(SocketMessageComponent arg)
        {
            var channel = arg.Channel as SocketTextChannel;
            var guild = channel.Guild;
            var roomName = $"🚨｜{guild.GetUser(arg.User.Id).DisplayName.ToLower()}";
            var cate = guild.CategoryChannels.Single(x => x.Name == "{ 사원 신고 }");
            var temp = cate.Channels.SingleOrDefault(x => x.Name == roomName);
            if (temp != null)
            {
                await arg.RespondAsync($"{MentionUtils.MentionChannel(temp.Id)} 이미 만들어진 방이네요!", ephemeral: true);
                return;
            }
            var ch = await makeTicketRoom(arg, roomName, cate.Id);
            var embed = new EmbedBuilder();
            embed.Color = Color.Blue;
            embed.Title = "대명그룹 사원 신고서 템플릿";
            embed.Description = $"팩션원 신고 \n\n디스코드 아이디:\n\n인게임 아이디:\n\n신고 사유: 사유 및 증거자료(스샷 혹은 영상 포함)";
            await ch.SendMessageAsync(embed: embed.Build());
            await arg.RespondAsync(text: $"{MentionUtils.MentionChannel(ch.Id)}으로 안내드리겠습니다.", ephemeral: true);
        }

        private async Task OnFactionPermisson(SocketMessageComponent arg)
        {
            var channel = arg.Channel as SocketTextChannel;
            var guild = channel.Guild;
            var cate = guild.CategoryChannels.Single(x => x.Name == "{ 권한 요청 }");
            var notiChannel = cate.Channels.SingleOrDefault(x => x.Name == "권한 요청") as SocketTextChannel;
            var embed = new EmbedBuilder();
            embed.Color = Color.Blue;
            embed.Title = "권한 요청";
            embed.Description = $"{MentionUtils.MentionUser(arg.User.Id)}님의 권한요청";
            await notiChannel.SendMessageAsync(embed: embed.Build());
            await arg.RespondAsync(text: $"권한 요청이 발송되엇습니다. 잠시만 기다려주세요.", ephemeral: true);
        }

        private async Task OnFactionJoin(SocketMessageComponent arg)
        {
            var channel = arg.Channel as SocketTextChannel;
            var guild = channel.Guild;
            var roomName = $"📝｜{guild.GetUser(arg.User.Id).DisplayName.ToLower()}";
            var cate = guild.CategoryChannels.Single(x => x.Name == "{ 입사 지원서 }");
            var temp = cate.Channels.SingleOrDefault(x => x.Name == roomName);
            if (temp != null)
            {
                await arg.RespondAsync($"{MentionUtils.MentionChannel(temp.Id)} 이미 만들어진 방이네요!", ephemeral: true);
                return;
            }
            var ch = await makeTicketRoom(arg, roomName, cate.Id);
            var embed = new EmbedBuilder();
            embed.Color = Color.Blue;
            embed.Title = "입사 지원서 템플릿";
            embed.Description = $"(( IN CHARACTER ))\n" +
                $"PERSONAL INFORMATION\n\n" +
                $"성함 : \n나이 : \n전화번호 : \n국적 : \n입사동기: (내용)(성의없거나 짧은 글은 거절될 수 있습니다. )\n\n" +
                $"((OUT OF CHRACTER))\n" +
                $"나 이: \nRP경력 :  TGF: RP - (팩션이름) / LARP - (팩션이름)등\n사용하시던 닉네임:\n\n" +
                $"IC CHARACTER 성장배경: (내용)(성의없거나 짧은 글은 거절될 수 있습니다. )\n\n\n" +
                $"(/ 스탯 사진 첨부해 주세요.)\n\n" +
                $"[!] 개명기록을 두려워하지 마세요.당신이 누구던 마음가짐이 제대로 잡혀있다면 괜찮습니다.\n" +
                $"[!] 본인의 부주의로 인하여 팩션 내 불이익 발생시 해고 또는 CK에 동의하십니까 ? (Y / N)\n" +
                $"[!] 장기 미접속 시 팩션에서 강제해고 조치될 수 있습니다. 동의하십니까 ? (Y / N)\n" +
                $"\n\nThank you for applying!";
            await ch.SendMessageAsync(embed: embed.Build());
            await arg.RespondAsync(text: $"{MentionUtils.MentionChannel(ch.Id)}으로 안내드리겠습니다.", ephemeral: true);
        }

        private async Task OnSellIsPoliceNo(SocketMessageComponent arg)
        {
            var channel = arg.Channel as SocketTextChannel;
            var guild = channel.Guild;
            var user = guild.GetUser(arg.User.Id);
            var role = guild.Roles.Single(x => x.Name == "🦹🏻우수 회원");
            await user.AddRoleAsync(role);
            await arg.RespondAsync("환영합니다.", ephemeral: true);
        }

        private async Task OnSellUpgrade(SocketMessageComponent arg)
        {
            var cb = new ComponentBuilder()
                .WithButton("네", "sell_ispolice_yes")
                .WithButton("아니요", "sell_ispolice_no");
            await arg.RespondAsync("경찰공무원에게는 추가적인 혜택이 있습니다.\n 당신은 경찰입니까?", components: cb.Build(), ephemeral: true);
        }

        private async Task OnSellBuy(SocketMessageComponent arg)
        {
            var embed = arg.Message.Embeds.First();
            var eb = new EmbedBuilder()
            {
                Color = Color.Green,
                Title = embed.Title,
                Description = embed.Description,
            }.AddField("✅ 판매 완료", $"구매자 : {arg.User.Mention}");
            await arg.Message.ModifyAsync(x =>
            {
                x.Embed = eb.Build();
                x.Components = new ComponentBuilder().Build();
            });
        }

        private async Task OnALwaySellUpload(SocketMessageComponent arg)
        {
            var mb = new ModalBuilder()
                .WithTitle("판매글 작성")
                .WithCustomId("sell_alwayupload")
                .AddTextInput("글 제목", "title", placeholder: "ex) 다이아몬드 싸게 팝니다.", required: true)
                .AddTextInput("판매 물건 이름", "name", placeholder: "판매할 물건 이름", required: true)
                .AddTextInput("판매 금액", "price", placeholder: "판매 금액(숫자만)", required: true)
                .AddTextInput("연락처", "phone", placeholder: "연락 받을 핸드폰 번호", required: true);
            await arg.RespondWithModalAsync(mb.Build());
        }

        private async Task OnSellUpload(SocketMessageComponent arg)
        {
            var mb = new ModalBuilder()
                .WithTitle("판매글 작성")
                .WithCustomId("sell_upload")
                .AddTextInput("글 제목", "title", placeholder: "ex) 다이아몬드 싸게 팝니다.", required: true)
                .AddTextInput("판매 물건 이름", "name", placeholder: "판매할 물건 이름", required: true)
                .AddTextInput("판매 금액", "price", placeholder: "판매 금액(숫자만)", required: true)
                .AddTextInput("연락처", "phone", placeholder: "연락 받을 핸드폰 번호", required: true);
            await arg.RespondWithModalAsync(mb.Build());
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
            await AnimalRace.Channel.ModifyMessageAsync((ulong)AnimalRace.BettingMsgId, x => x.Components = new ComponentBuilder().Build());
            await  arg.DeferAsync();
            var winners = await RunAnimalRace();
            if (!DB.TryAddUsersCoin(winners)) await arg.RespondAsync("TryAddUsersCoin => DB에러");
            AnimalRace.Clear();
        }

        private async Task<BettingMembers> RunAnimalRace()
        {
            BettingMembers WinnerMembers;
            var race = AnimalRace.MakeAnimalRace();
            var msg = await AnimalRace.Channel.SendMessageAsync("", false, race.GetEmbed(isStart: true));

            while (!race.isDone)
            {
                await Task.Delay(1500);
                var embed = race.GetEmbed();
                if (embed == null) break;
                await msg.ModifyAsync(msg => msg.Embed = embed);
            }

            WinnerMembers = race.WinnerMembers;

            if (race.WinnerMembers == null)
            {
                await Task.Delay(5000);
                WinnerMembers = await RunAnimalRace();
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
                .AddTextInput("2번마 이름", "animal2_name", placeholder: "ex) 전설의 백마", required: true)
                .AddTextInput("2번마 이모티콘", "animal2_emoji", placeholder: "ex) :horse_racing: (채팅에 이모티콘 치고 복붙)", required: true);
            await arg.RespondWithModalAsync(mb.Build());
        }

        private async Task OnRaceCancel(SocketMessageComponent arg)
        {
            await AnimalRace.Channel.SendMessageAsync("이 경마는 취소 되었습니다.");
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

        private async Task<RestTextChannel> makeTicketRoom(
            SocketMessageComponent arg, 
            string roomName, 
            ulong catgoryId, 
            List<SocketRole>? roles = null, 
            List<SocketRole>? denyroles = null)
        {
            var guild = (arg.Channel as SocketTextChannel).Guild;
            var ch = await guild.CreateTextChannelAsync(roomName, x => x.CategoryId = catgoryId);
            var denyper = new OverwritePermissions(viewChannel: PermValue.Deny, sendMessages: PermValue.Deny);
            var userPer = new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow);
            if (roles != null)
                roles.ForEach(async x => await ch.AddPermissionOverwriteAsync(x, userPer));
            if (denyroles != null)
                denyroles.ForEach(async x => await ch.AddPermissionOverwriteAsync(x, denyper));
            await ch.AddPermissionOverwriteAsync(guild.EveryoneRole, denyper);
            await ch.AddPermissionOverwriteAsync(arg.User, userPer);
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
            var dealerPer = guild.Roles.Where(x => x.Name == "CASINO Dealer").ToList();
            var guestPer = guild.Roles.Where(x => x.Name == "CASINO Guest").ToList();
            var ch = await makeTicketRoom(arg, roomName, (ulong)channel.CategoryId, dealerPer, guestPer);
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
            var dealerPer = guild.Roles.Where(x => x.Name == "CASINO Dealer").ToList();
            var guestPer = guild.Roles.Where(x => x.Name == "CASINO Guest").ToList();
            var ch = await makeTicketRoom(arg, roomName, (ulong)channel.CategoryId, dealerPer, guestPer);
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
