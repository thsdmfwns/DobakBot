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
                default: return;
            }

        }

        private async Task OnCustomerReturnButton(SocketMessageComponent arg)
        {
            var comp = new ComponentBuilder().WithSelectMenu(GetMoneySelectMenu("return"));
            await arg.RespondAsync($"충전할 금액을 선택해주세요.", component: comp.Build(), ephemeral: true);
        }

        private async Task OnCustomerPayButton(SocketMessageComponent arg)
        {
            var comp = new ComponentBuilder().WithSelectMenu(GetMoneySelectMenu("pay"));
            await arg.RespondAsync($"충전할 금액을 선택해주세요.", component: comp.Build(), ephemeral: true);
        }

        private SelectMenuBuilder GetMoneySelectMenu(string id)
        {
            var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("금액 선택")
            .WithCustomId($"customer{id}_select")
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
            await arg.RespondAsync($"무기 또는 탄창을 하나만 선택해주세요. \n 선택후, 이메세지를 닫는것을 추천합니다.",component:comp.Build() ,ephemeral: true);

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
            //var notifiyChannel = guild.Channels.Single(x => x.Name == "자유게시판-ic") as SocketTextChannel;
            var user = guild.GetUser(arg.User.Id);
            var role = guild.Roles.Single(x => x.Name == "Customer");

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
            //await notifiyChannel.SendMessageAsync($"{user.Nickname}님이 카지노에 입장하셨습니다.");
            await arg.DeferAsync();
        }
    }
}
