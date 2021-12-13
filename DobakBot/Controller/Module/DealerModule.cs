using Discord.Commands;
using Discord.WebSocket;
using DobakBot.Controller.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Controller.Module
{
    [RequireChannel("자유게시판")]
    [RequireRole("CASINO dealer")]
    [Group("코인")]
    public class DealerModule : ModuleBase<SocketCommandContext>
    {
        private readonly DBController DB = GambleController.Instance.DB;

        const string AddHelp = "코인 충전 : !코인 충전 닉네임-금액 (ex : !코인 충전 Dalsu_Son-10000)\n";
        const string SubtracktHelp = "코인 환전 : !코인 환전 닉네임-금액 (ex : !코인 환전 Dalsu_Son-10000)\n";

        [Command("help")]
        public async Task HelpCommand()
        {
            await ReplyAsync(AddHelp + SubtracktHelp);
        }

        [Command("충전")]
        public async Task AddUserCoinCommand([Remainder] string arg)
        {
            _ = Context.Message.DeleteAsync();
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

            await ReplyAsync($"{list[0]}님이 {money}:coin: 충전하셨습니다.");
        }

        [Command("환전")]
        public async Task SubtractUserCoinCommand([Remainder] string arg)
        {
            _ = Context.Message.DeleteAsync();
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
                await ReplyAsync($"{list[0]}님의 :coin:이 부족하여 환전이 불가능합니다.\n" +
                    $"(현재 잔액 : {dbuser.coin}:coin:) (요청금액 : {money}:coin:).");
                return;
            }

            if (!DB.TrySubtractUserCoin(user.Id, money))
            {
                await ReplyAsync($"TrySubtractUserCoin Error");
                return;
            }

            await ReplyAsync($"{list[0]}님이 {money - money*0.03}$ 환전하셨습니다.");
        }
    }
}
