using Discord;
using Discord.Commands;
using DobakBot.Controller.Controller;
using DobakBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Controller.Module
{
    public class WeaponPayModule : ModuleBase<SocketCommandContext>
    {
        WeaponPayController weaponPay = BotController.Instance.WeaponPay;
        [Command("장부")]
        public async Task jangbuCommand([Remainder] int count)
        {
            var user = Context.User as IGuildUser;
            weaponPay.WeaponPayMap.TryAdd(Context.User.Id, new WeaponPay()
            {
                Count = count
            });

            var com = new ComponentBuilder();
            com.WithButton(label: "보급", customId: "Weapon_Suply");
            com.WithButton(label: "판매", customId: "Weapon_Sell");
            com.WithButton(label: "취소", customId: "Weapon_Cancel");

            await ReplyAsync($"{user.Nickname}님의 장부도우미 \n 요청 갯수 : {count}", component: com.Build());
        }
    }
}
