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
    [RequireChannel("💰ㅣ상납기록")]
    public class RepaymentModule : ModuleBase<SocketCommandContext>
    {
        [Command("상납")]
        public async Task RepaymentCommand([Remainder] int money)
        {
            _ = Context.Message.DeleteAsync();
            var user = Context.User as IGuildUser;
            var name = user.Nickname;
            if (name == string.Empty || name == null)
            {
                await ReplyAsync("닉네임을 IC상의 닉네임으로 변경해주세요.");
                return;

            }
            var context = new Repayment()
            {
                Money = money,
                Name = name,
            };
            var builder = new EmbedBuilder();
            builder.Description = context.ToString();
            builder.Color = Color.Green;

            await ReplyAsync(embed: builder.Build());
        }
    }
}
