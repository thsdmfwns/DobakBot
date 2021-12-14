using Discord;
using Discord.Commands;
using Discord.Rest;
using DobakBot.Controller;
using DobakBot.Controller.Attribute;
using DobakBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DobakBot
{
    public class GambleModule : ModuleBase<SocketCommandContext>
    {
        private DBController DB = BotController.Instance.DB;
        [Command("help")]
        public async Task HelpCommand()
        {
            string text = string.Empty;
            text += "파칭코 : 파칭코 채널을 입장후 !파칭코 help\n";
            text += "경마 : 경마 채널을 입장후, !경마 help\n";
            text += "지갑보기 : 카지노의 아무 채널에서 !지갑\n";
            text += "(직원전용)코인 : !코인 help\n";
            await ReplyAsync(text);
        }

/*        [Command("test")]
        public async Task ChannelIdCommand()
        {
            var guildid = Context.Guild.Id;
            Console.WriteLine(guildid);
            foreach (var item in Context.Guild.Channels)
            {
                Console.WriteLine($"{item.Id} => {item.Name}");
            }
            foreach (var item in Context.Guild.Roles)
            {
                Console.WriteLine($"{item.Id} => {item.Name}");
            }
            //await ReplyAsync($"{Context.Channel.Id}");
        }*/

        [Command("지갑")]
        public async Task GetuserCommand()
        {
            var user = DB.GetUserByDiscordId(Context.User.Id);
            if (user == null)
            {
                await ReplyAsync($"등록되지 않은 사용자입니다.");
                return;
            }
            var nick = Context.Guild.GetUser(user.id).Nickname;
            await ReplyAsync($"{nick}님의 현재 남은:coin:은 {user.coin}:coin: 입니다.");
        }

    }
}
