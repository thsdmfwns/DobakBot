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
    [RequireRole("Admin-")]
    public class AdminMoudule : ModuleBase<SocketCommandContext>
    {
        [Command("입장버튼")]
        public async Task EnterButtonSpawn()
        {
            var button = new ComponentBuilder().WithButton("카지노 입장하기", "casino_enter", style: ButtonStyle.Secondary, emote: new Emoji("\uD83C\uDFB4"));
            var builder = new EmbedBuilder();
            builder.Title = ":shinto_shrine: 카지노 입장";
            builder.Color = Color.Green;
            builder.Description = 
                "카지노에 입장하시기 전, \n 디스코드 채널의 별명을 IC상의 이름으로 설정해주세요.\n" +
                "IC상에서의 코인 충전과 코인 환전을 위해 필요합니다.";
            await ReplyAsync("", embed: builder.Build(), components: button.Build());
        }

        [Command("지갑버튼")]
        public async Task WalletButtonSpawn()
        {
            var buttons = new ComponentBuilder()
                .WithButton("지갑보기", "customer_Wallet", style: ButtonStyle.Primary)
                .WithButton("충전하기", "customer_pay", style: ButtonStyle.Success)
                .WithButton("환전하기", "customer_return", style: ButtonStyle.Danger);
            var builder = new EmbedBuilder();
            builder.Title = "환전소";
            builder.Color = Color.Blue;
            await ReplyAsync("", embed: builder.Build(), components: buttons.Build());
        }

        [Command("test")]
        public async Task test()
        {
            var buttons = new ComponentBuilder()
                .WithButton("방생성", "slot_roomCreate", style: ButtonStyle.Primary);
            var builder = new EmbedBuilder();
            builder.Title = "test";
            builder.Color = Color.Blue;
            await ReplyAsync("", embed: builder.Build(), components: buttons.Build());
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

        [Command("clear")]
        public async Task ClearMessages([Remainder] int count)
        {
            var msgs = await Context.Channel.GetMessagesAsync(limit:count).SingleAsync();
            foreach (var item in msgs)
            {
                await item.DeleteAsync();
                await Task.Delay(1050);
            }
        }
    }
}
