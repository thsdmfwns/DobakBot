using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Controller
{
    [RequireUserPermission(GuildPermission.Administrator)]
    public class AdminMoudule : ModuleBase<SocketCommandContext>
    {
        [Command("입장버튼")]
        public async Task Spawn()
        {
            var button = new ComponentBuilder().WithButton("카지노 입장하기", "casino_enter", style: ButtonStyle.Secondary);
            await ReplyAsync("", component: button.Build());
        }
    }
}
