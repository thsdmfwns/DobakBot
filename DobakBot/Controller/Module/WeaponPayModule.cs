using Discord;
using Discord.Commands;
using DobakBot.Controller.Attribute;
using DobakBot.Controller.Controller;
using DobakBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Controller.Module
{
    [RequireChannel("장부")]
    public class WeaponPayModule : ModuleBase<SocketCommandContext>
    {
        WeaponPayController weaponPay = BotController.Instance.WeaponPay;

        [Command("장부")]
        public async Task jangbuCommand([Remainder] int count)
        {
            _ = Context.Message.DeleteAsync();
            var user = Context.User as IGuildUser;
        }
    }
}
