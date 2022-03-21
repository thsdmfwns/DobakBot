using Discord;
using Discord.Commands;
using DobakBot.Controller.Attribute;
using DobakBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Controller.Module
{
    [RequireChannel("파칭코")]
    [Group("파칭코")]
    public class SlotMachineModule : ModuleBase<SocketCommandContext>
    {
        const string OddText =
            ":shinto_shrine:　 :shinto_shrine:　 :shinto_shrine:  =　 BET X 8\n\n" +
            ":grapes:　 :grapes:　 :grapes:  =　 BET X 13\n\n" +
            ":tangerine:　 :tangerine:　 :tangerine:  =　 BET X 16\n\n" +
            ":bell:　 :bell:　 :bell:  =　 BET X 10\n\n" +
            ":flower_playing_cards:　 :flower_playing_cards:　 :flower_playing_cards:  =　 BET X 20\n\n" +
            ":cherries:　 　 　 　      =　  BET X 2\n\n" +
            ":cherries:　 :cherries:　 　     =　  BET X 5\n\n" +
            ":cherries:　 :cherries:　 :cherries:  =　 BET X 10";
        const string RunHelp = "파칭코 돌리기 : !파칭코 돌리기 베팅금액 (!파칭코 1000)\n";
        const string OddHelp = "파칭코 배율 보기 : !파칭코 배율\n";

        private DBController DB = BotController.Instance.DB;

        [Command("배율")]
        public async Task GetHelpCommand()
        {
            var builder = new EmbedBuilder();
            builder.Title = "파칭코 배율";
            builder.Description = OddText;
            builder.Color = Color.Orange;
            await ReplyAsync(embed: builder.Build());
        }

        [Command("help")]
        public async Task HelpCommand()
        {
            await ReplyAsync(RunHelp+ OddHelp);
        }
        [Command("돌리기")]
        public async Task SlotMachineCommand([Remainder] string arg)
        {

            int money;
            if (!int.TryParse(arg, out money))
            {
                await ReplyAsync($"{arg} 일치 하지 않는 값입니다.\n" + "!파칭코 배팅금액 (ex : !파칭코 1000)");
                return;
            }
            if (money > 500)
            {
                await ReplyAsync($"베팅 최대 금액을 초과 했습니다. (베팅최대금액 : 500:coin:)");
                return;
            }

            var user = DB.GetUserByDiscordId(Context.User.Id);
            if (user == null)
            {
                await ReplyAsync($"등록되지 않은 사용자입니다.");
                return;
            }
            if (user.coin < money)
            {
                await ReplyAsync($"{Context.Guild.GetUser(Context.User.Id).Nickname}님의 :coin:이 부족합니다 ({user.coin - money}:coin:).");
                return;
            }

            DB.TrySubtractUserCoin(Context.User.Id, money);
            _ = RunSlotMachine(money);
        }

        private async Task RunSlotMachine(int money)
        {
            var slot = new SlotMachine(money);
            await slot.SetResult();

            var embeds = slot.getEmbeds(Context.Guild.GetUser(Context.User.Id).Nickname);
            var msg = await Context.Channel.SendMessageAsync("", false, embeds[0]);
            await Task.Delay(2000);
            await msg.ModifyAsync(msg => msg.Embed = embeds[1]);
            await Task.Delay(1010);
            await msg.ModifyAsync(msg => msg.Embed = embeds[2]);
            await Task.Delay(1010);
            await msg.ModifyAsync(msg => msg.Embed = embeds[3]);
            DB.TryAddUserCoin(Context.User.Id, slot.ResultCoin);
        }
    }
}
