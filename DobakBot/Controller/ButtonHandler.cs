using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Controller
{
    class ButtonHandler
    {
        private DBController DB = GambleController.Instance.DB;
        private DiscordSocketClient Client;
        private const ulong roleId = 915546552062312468;
        //private const ulong roleId = 915636004323475487;


        public ButtonHandler(DiscordSocketClient client)
        {
            Client = client;
            client.ButtonExecuted += Client_ButtonExecuted;
        }

        private async Task Client_ButtonExecuted(SocketMessageComponent arg)
        {
            switch (arg.Data.CustomId)
            {
                case "casino_enter": OnEnterButton(arg); return;
                default: return;
            }

        }

        private async void OnEnterButton(SocketMessageComponent arg)
        {
            var channel = arg.Channel as SocketTextChannel;
            var user = channel.Guild.GetUser(arg.User.Id);
            var role = channel.Guild.GetRole(roleId);

            if (user.Nickname == null || user.Nickname == string.Empty)
            {
                SendButtonMessage($"{user.Username} => 디스코드 채널의 별명을 IC상의 이름으로 설정해주세요!", channel);
                await arg.DeferAsync();
                return;
            }

            if (user.Roles.Contains(role))
            {
                SendButtonMessage($"{user.Nickname} => 이미 입장하신분이네요!", channel);
                await arg.DeferAsync();
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

        private async void SendButtonMessage(string msg, SocketTextChannel channel)
        {
            var channelmsg = await channel.SendMessageAsync(msg);
            await Task.Delay(5000);
            await channelmsg.DeleteAsync();
        }
    }
}
