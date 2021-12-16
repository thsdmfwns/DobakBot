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
        private const ulong roleId = 915546552062312468;


        public ButtonHandler(DiscordSocketClient client)
        {
            client.ButtonExecuted += Client_ButtonExecuted;
        }

        private async Task Client_ButtonExecuted(SocketMessageComponent arg)
        {
            switch (arg.Data.CustomId)
            {
                case "casino_enter": await OnEnterButton(arg); return;
                case "Weapon_Suply": await OnWeaponSuplyButton(arg); return;
                case "Weapon_Sell": await OnWeaponSellButton(arg); return;
                case "Weapon_Cancel": await OnWeaponCancelButton(arg); return;
                case "test": await testButton(arg); return;
                default: return;
            }

        }

        private async Task OnWeaponCancelButton(SocketMessageComponent arg)
        {
            WeaponPay temp;
            WeaponPay.WeaponPayMap.TryRemove(arg.User.Id, out temp);
            await arg.Message.DeleteAsync();
        }

        private async Task OnWeaponSellButton(SocketMessageComponent arg)
        {
            await arg.Message.DeleteAsync();
            var id = arg.User.Id;
            if (!WeaponPay.WeaponPayMap.ContainsKey(id))
            {
                await arg.RespondAsync($"장부 도우미를 한번더 불려와 주세요!\n장부도우미 부르기 : !장부 무기갯수 (!장부 1)", ephemeral: true);
                return;
            }
            WeaponPay.WeaponPayMap[id].Kind = WeaponPayKind.Sell;
            var comp = new ComponentBuilder().WithSelectMenu(GetWeponSelectMenu());
            await arg.RespondAsync($"무기 또는 탄창을 선택해주세요. \n 선택후, 이메세지를 닫는것을 추천합니다.",component:comp.Build() ,ephemeral: true);

        }

        private async Task OnWeaponSuplyButton(SocketMessageComponent arg)
        {
            await arg.Message.DeleteAsync();
            var id = arg.User.Id;
            if (!WeaponPay.WeaponPayMap.ContainsKey(id))
            {
                await arg.RespondAsync($"장부 도우미를 한번더 불려와 주세요!\n장부도우미 부르기 : !장부 무기갯수 (!장부 1)", ephemeral: true);
                return;
            }
            WeaponPay.WeaponPayMap[id].Kind = WeaponPayKind.supply;

            var comp = new ComponentBuilder().WithSelectMenu(GetWeponSelectMenu());

            await arg.RespondAsync($"무기 또는 탄창을 선택해주세요. \n 선택후, 이메세지를 닫는것을 추천합니다.", component: comp.Build(), ephemeral: true);
        }

        private SelectMenuBuilder GetWeponSelectMenu()
        {
            var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("총기 선택")
            .WithCustomId("WeaponPay_SelectMenu")
            .WithMinValues(1)
            .WithMaxValues(1);
            var options = Weapon.GetList();
            foreach (var item in options)
            {
                menuBuilder.AddOption(item.Name, item.Name);
            }
            return menuBuilder;
        }

        private async Task testButton(SocketMessageComponent arg)
        {
            await arg.RespondAsync("이건 테스트로 만들어진 당신만이 볼수 있는 메세지입니다.", ephemeral: true);
        }

        private async Task OnEnterButton(SocketMessageComponent arg)
        {
            var channel = arg.Channel as SocketTextChannel;
            var guild = channel.Guild;
            var notifiyChannel = guild.Channels.Single(x => x.Name == "자유게시판-ic") as SocketTextChannel;
            var user = guild.GetUser(arg.User.Id);
            var role = guild.GetRole(roleId);

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
            await notifiyChannel.SendMessageAsync($"{user.Nickname}님이 카지노에 입장하셨습니다.");
            await arg.DeferAsync();
        }
    }
}
