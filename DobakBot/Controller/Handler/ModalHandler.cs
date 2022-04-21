using Discord;
using Discord.WebSocket;
using DobakBot.Controller.Controller;
using DobakBot.Model;
using DobakBot.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Controller.Handler
{
    internal class ModalHandler
    {
        private AnimalRaceController AnimalRace = BotController.Instance.animalRace;
        private WeaponPayController WeaponPay = BotController.Instance.WeaponPay;
        private DBController DB = BotController.Instance.DB;

        public ModalHandler(DiscordSocketClient client)
        {
            client.ModalSubmitted += Client_ModalSubmitted;
        }

        private async Task Client_ModalSubmitted(SocketModal arg)
        {
            switch (arg.Data.CustomId)
            {
                case "weapon_add": await onWeaponAdd(arg); return;
                case "weapon_pay": await onWeaponPay(arg); return;
                case "race_make": await onRaceMake(arg); return;
                case "race_bet": await onRaceBet(arg); return;
                case "sell_upload": await onSellUpload(arg); return;
                default:
                    break;
            }
            return;
        }

        private async Task onSellUpload(SocketModal arg)
        {
            var name = arg.Data.Components.Single(x => x.CustomId == "name").Value;
            int price;
            if (!int.TryParse(arg.Data.Components.Single(x => x.CustomId == "price").Value, out price))
            {
                await arg.RespondAsync($"가격은 숫자로만 입력해주세요.", ephemeral: true);
                return;
            }
            var phone = arg.Data.Components.Single(x => x.CustomId == "phone").Value;
            var nf = (await arg.GetChannelAsync() as SocketTextChannel).Guild.Channels.Single(x => x.Name == "💻｜판매-물건-목록") as SocketTextChannel;
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            var eb = new EmbedBuilder() {
            Color = Color.Orange,
            Title = $"{(arg.User as IGuildUser).Nickname}님의 판매 물건",
            Description = 
                $"판매 물건 : {name} \n"+
                $"판매 가격 : {price.ToString("C0", nfi)} \n"+
                $"연락처 : 📞{phone} \n",
            };
            var cb = new ComponentBuilder()
                .WithButton("물건 구매하기", customId: "sell_buy");

            await nf.SendMessageAsync(embed: eb.Build(), components: cb.Build());

            await arg.RespondAsync($"등록 완료!", ephemeral: true);
        }

        private async Task onRaceBet(SocketModal arg)
        {
            var comp = arg.Data.Components.First();
            var animal = comp.CustomId;
            var dbuser = DB.GetUserByDiscordId(arg.User.Id);
            var nick = (arg.User as IGuildUser).Nickname;
            if (dbuser == null)
            {
                await arg.RespondAsync($"{nick} 등록되지 않은 사용자입니다.", ephemeral: true);
                return;
            }

            int money;
            if (!int.TryParse(comp.Value, out money))
            {
                await arg.RespondAsync($"{comp.Value} 일치 하지 않는 값입니다.", ephemeral: true);
                return;
            }
            if (dbuser.coin < money)
            {
                await arg.RespondAsync($"{nick}님의 :coin:이 부족하여 환전이 불가능합니다.\n" +
                    $"(현재 잔액 : {dbuser.coin}:coin:) (요청금액 : {money}:coin:).", ephemeral: true);
                return;
            }

            if (!AnimalRace.TryAddBetting(new BettingMember(arg.User.Id, nick, money), animal))
            {
                {
                    await arg.RespondAsync($"베팅실패 흐에에");
                    return;
                }
            }

            if (!DB.TrySubtractUserCoin(arg.User.Id, money))
            {
                await arg.RespondAsync($"TrySubtractUserCoin Error \nID : {arg.User.Id}, Money {money}");
                return;
            }

            await arg.Channel.ModifyMessageAsync((ulong)AnimalRace.BettingMsgId, x=> x.Embed = AnimalRace.GetBettingPanel());
            await arg.RespondAsync($"베팅 완료.", ephemeral: true);
        }

        private async Task onRaceMake(SocketModal arg)
        {
            var list = new List<Animal>();
            var raceName = arg.Data.Components.Single(x => x.CustomId == $"race_name").Value;
            for (int i = 1; i < 3; i++)
            {
                var name = arg.Data.Components.Single(x => x.CustomId == $"animal{i}_name").Value;
                var emoji = arg.Data.Components.Single(x => x.CustomId == $"animal{i}_emoji").Value.Trim();
                if (emoji.First() != ':' || emoji.Last() != ':')
                {
                    await arg.RespondAsync($"오류! 이모티콘을 확인해 주세요!{emoji}", ephemeral: true);
                    return;
                }
                if (list.Any(x=> x.Name == name))
                {
                    await arg.RespondAsync("오류! 동물 이름이 똑같은거 같은데요?", ephemeral: true);
                    return;
                }
                list.Add(new Animal(name, emoji));
            }
            AnimalRace.MakeBettings(list);
            AnimalRace.RaceName = raceName;
            var ch = await arg.GetChannelAsync() as SocketTextChannel;
            var nf = await Utility.makePublicRoom(ch.Guild, raceName, (ulong)ch.CategoryId);
            AnimalRace.Channel = nf;
            var select = new SelectMenuBuilder()
                .WithCustomId("race_bet")
                .WithMinValues(1).WithMaxValues(1)
                .WithPlaceholder("동물 선택");
            AnimalRace.Animals.ForEach(item => select.AddOption(item.Name, item.Name));
            var comp = new ComponentBuilder()
                .WithSelectMenu(select);
            var msg = await nf.SendMessageAsync("", false, AnimalRace.GetBettingPanel(), components: comp.Build());
            AnimalRace.BettingMsgId = msg.Id;
            await arg.DeferAsync();
        }

        private async Task onWeaponPay(SocketModal arg)
        {
            int count;
            if (!int.TryParse(arg.Data.Components.Single(x=>x.CustomId == "count").Value,out count))
            {
                await arg.RespondAsync("오류! 갯수는 숫자만 입력해주세요.", ephemeral: true);
                return;
            }
            var ch = (arg.Channel as SocketTextChannel).Guild.GetChannel((ulong)WeaponPay.ChannelId) as SocketTextChannel;
            var msg = await ch.GetMessageAsync((ulong)WeaponPay.MessageId);
            var cu = arg.Data.Components.SingleOrDefault(x => x.CustomId == "name").Value;
            var weapons = Weapon.ListFromJson(msg.Content);
            WeaponPay.WeaponPayMap[arg.User.Id].Count = count;
            WeaponPay.WeaponPayMap[arg.User.Id].Weapons = weapons;
            WeaponPay.WeaponPayMap[arg.User.Id].UserName = ch.GetUser(arg.User.Id).Nickname;
            WeaponPay.WeaponPayMap[arg.User.Id].CustomerName = cu ?? ch.GetUser(arg.User.Id).Nickname;
            var sb = new SelectMenuBuilder()
                .WithCustomId("WeaponPay_SelectMenu").WithPlaceholder("무기 선택")
                .WithMinValues(1).WithMaxValues(1);
            foreach (var item in weapons)
            {
                sb.AddOption(item.Name, item.Name);
            }
            var cb = new ComponentBuilder().WithSelectMenu(sb);
            var embed = new EmbedBuilder()
            {
                Color = Color.Blue,
                Title = "무기 선택",
            };
            await arg.RespondAsync("무기를 선택해 주세요.", components: cb.Build(), ephemeral:true);
        }

        private async Task onWeaponAdd(SocketModal arg)
        {
            Weapon weapon;
            try
            {
                weapon = new Weapon()
                {
                    Name = arg.Data.Components.Single(x => x.CustomId == "weapon_name").Value,
                    Price = int.Parse(arg.Data.Components.Single(x => x.CustomId == "weapon_price").Value),
                    SellPrice = int.Parse(arg.Data.Components.Single(x => x.CustomId == "weapon_sellprice").Value),
                    Unit = arg.Data.Components.Single(x => x.CustomId == "weapon_unit").Value,
                };
            }
            catch (Exception)
            {
                await arg.RespondAsync("오류! 가격란에 숫자만 입력해주세요.", ephemeral:true);
                return;
            }
            var msg = await arg.Channel.GetMessageAsync((ulong)WeaponPay.MessageId);
            if (msg == null)
            {
                await arg.RespondAsync("DB를 찾을수 없음", ephemeral: true);
                return;
            }
            List<Weapon> weapons;
            if 
                (msg.Content == null || msg.Content == "empty") weapons = new List<Weapon>();
            else 
                weapons = Weapon.ListFromJson(msg.Content);
            if (weapons.SingleOrDefault(x=> x.Name == weapon.Name) != null)
            {
                await arg.RespondAsync("이미 존재하는 이름의 물건이네요.", ephemeral: true);
                return;
            }
            weapons.Add(weapon);
            await arg.Channel.ModifyMessageAsync((ulong)WeaponPay.MessageId, x => x.Content = Weapon.ListToJson(weapons));
            await arg.RespondAsync("DB 등록 성공!", ephemeral: true);
        }
    }
}
