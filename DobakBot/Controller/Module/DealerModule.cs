using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Controller.Module
{
    public class DealerModule : ModuleBase<SocketCommandContext>
    {
        private DBController DB = GambleController.Instance.DB;

        const string AddHelp = "코인 충전 : !충전 닉네임 금액 (ex : !충전 Dalsu_Son 10000";

        [Command("충전")]
        public async Task AddUserCoinCommand([Remainder] string arg)
        {
            var list = arg.Split('-', StringSplitOptions.RemoveEmptyEntries);
            await Context.Guild.DownloadUsersAsync();
            var user = Context.Guild.Users.FirstOrDefault(x => x.Nickname == list[0]);
            if (user == null)
            {
                await ReplyAsync($"{list[0]} 등록되지 않은 사용자입니다.");
                return;
            }

            int money;
            if (!int.TryParse(list[1], out money))
            {
                await ReplyAsync($"{list[1]} 일치 하지 않는 값입니다.\n");
                return;
            }

            if(!DB.TryAddUserCoin(user.Id, money))
            {
                await ReplyAsync($"TryAddUserCoin Error");
                return;
            }

            await ReplyAsync($"{list[0]} => {money} 충전성공.");
        }

        [Command("환전")]
        public async Task SubtractUserCoinCommand([Remainder] string arg)
        {
            var list = arg.Split('-', StringSplitOptions.RemoveEmptyEntries);
            await Context.Guild.DownloadUsersAsync();
            var user = Context.Guild.Users.FirstOrDefault(x => x.Nickname == list[0]);
            if (user == null)
            {
                await ReplyAsync($"{list[0]} 등록되지 않은 사용자입니다.");
                return;
            }

            int money;
            if (!int.TryParse(list[1], out money))
            {
                await ReplyAsync($"{list[1]} 일치 하지 않는 값입니다.\n");
                return;
            }

            var dbuser = DB.GetUserByDiscordId(user.Id);
            if(dbuser == null)
            {
                await ReplyAsync($"{list[0]} 등록되지 않은 사용자입니다.");
                return;
            }

            if (dbuser.coin < money)
            {
                await ReplyAsync($"{list[0]}님의 :coin:이 부족합니다 ({dbuser.coin - money}:coin:).");
                return;
            }

            if (!DB.TrySubtractUserCoin(user.Id, money))
            {
                await ReplyAsync($"TrySubtractUserCoin Error");
                return;
            }

            await ReplyAsync($"{list[0]} => {money} 환전성공.");
        }
    }
}
