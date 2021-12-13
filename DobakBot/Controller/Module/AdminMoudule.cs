using Discord;
using Discord.Commands;
using DobakBot.Controller.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Controller
{
    [RequireRole("CASINO dealer")]
    public class AdminMoudule : ModuleBase<SocketCommandContext>
    {
        [RequireChannel("사용안함")]
        [Command("입장버튼")]
        public async Task ButtonSpawn()
        {
            var button = new ComponentBuilder().WithButton("카지노 입장하기", "casino_enter", style: ButtonStyle.Secondary, emote: new Emoji("\uD83C\uDFB4"));
            var builder = new EmbedBuilder();
            builder.Title = ":shinto_shrine: 카지노 입장";
            builder.Color = Color.Green;
            builder.Description = "카지노에 입장하시기 전, \n 디스코드 채널의 별명을 IC상의 이름으로 설정해주세요.\n" +
                "IC상에서의 코인 충전과 코인 환전을 위해 필요합니다.";
            await ReplyAsync("", embed: builder.Build(), component: button.Build());
        }

        [Command("상납기록버튼")]
        public async Task SangnapButtonSpawn()
        {
            var button = new ComponentBuilder().WithButton("카지노 입장하기", "casino_enter", style: ButtonStyle.Secondary, emote: new Emoji("\uD83C\uDFB4"));
            var builder = new EmbedBuilder();
            builder.Title = ":shinto_shrine: 카지노 입장";
            builder.Color = Color.Green;
            builder.Description = "카지노에 입장하시기 전, \n 디스코드 채널의 별명을 IC상의 이름으로 설정해주세요.\n" +
                "IC상에서의 코인 충전과 코인 환전을 위해 필요합니다.";
            await ReplyAsync("", embed: builder.Build(), component: button.Build());
        }

        [Command("clear")]
        public async Task ClearMessages()
        {
            var msgs = await Context.Channel.GetMessagesAsync().SingleAsync();
            foreach (var item in msgs)
            {
                await item.DeleteAsync();
                await Task.Delay(1050);
            }
        }
    }
}
