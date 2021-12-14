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
        private DBController DB = BotController.Instance.DB;
        private WeaponPayController WeaponPay = BotController.Instance.WeaponPay;

        public SelectMenuHandler(DiscordSocketClient client)
        {
            client.SelectMenuExecuted += Client_SelectMenuExecuted;
        }

        private async Task Client_SelectMenuExecuted(SocketMessageComponent arg)
        {
            switch (arg.Data.CustomId)
            {
                case "WeaponPay_SelectMenu": await OnWeaponPaySelectMenu(arg); return;
                default: return;
            }
        }

        private async Task OnWeaponPaySelectMenu(SocketMessageComponent arg)
        {
            var data = arg.Data.Values.First();
            var wp = Utility.StringtoWeapon(data);
            if (wp == null)
            {
                await arg.RespondAsync($"장부 도우미를 한번더 불려와 주세요!\n장부도우미 부르기 : !장부 무기갯수 (!장부 1)", ephemeral: true);
                return;
            }

            WeaponPay ctx;

            if (!WeaponPay.WeaponPayMap.TryRemove(arg.User.Id, out ctx))
            {
                await arg.RespondAsync($"장부 도우미를 한번더 불려와 주세요!\n장부도우미 부르기 : !장부 무기갯수 (!장부 1)", ephemeral: true);
                return;
            }


            ctx.Weapon = (Weapon)wp;
            ctx.UserName = (arg.User as IGuildUser).Nickname;

            var builder = new EmbedBuilder();
            builder.Description = ctx.ToString();
            builder.Color = Color.Green;

            await arg.Channel.SendMessageAsync(embed: builder.Build());
            await arg.DeferAsync();
        }
    }
}
