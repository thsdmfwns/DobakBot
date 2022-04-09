using Discord.WebSocket;
using DobakBot.Controller.Controller;
using DobakBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Controller.Handler
{
    internal class ModalHandler
    {
        private WeaponPayController WeaponPay = BotController.Instance.WeaponPay;
        public ModalHandler(DiscordSocketClient client)
        {
            client.ModalSubmitted += Client_ModalSubmitted;
        }

        private async Task Client_ModalSubmitted(SocketModal arg)
        {
            switch (arg.Data.CustomId)
            {
                case "weapon_add": await onWeaponAdd(arg); return;
                default:
                    break;
            }
            return;
        }

        private async Task onWeaponAdd(SocketModal arg)
        {
            Weapon weapon;
            try
            {
                weapon = new Weapon()
                {
                    Name = arg.Data.Components.Single(x => x.CustomId == "weapon_name").Value,
                    Price = int.Parse(arg.Data.Components.Single(x => x.CustomId == "weapon_price").Value),
                    SellPrice = int.Parse(arg.Data.Components.Single(x => x.CustomId == "weapon_sellprice").Value),
                    Unit = arg.Data.Components.Single(x => x.CustomId == "weapon_unit").Value,
                };
            }
            catch (Exception ex)
            {
                await arg.RespondAsync("오류! 가격란에 숫자만 입력해주세요.", ephemeral:true);
                return;
            }
            var msg = await arg.Channel.GetMessageAsync((ulong)WeaponPay.messageId);
            if (msg == null)
            {
                await arg.RespondAsync("DB를 찾을수 없음", ephemeral: true);
                return;
            }
            List<Weapon> weapons;
            if 
                (msg.Content == null || msg.Content == "empty") weapons = new List<Weapon>();
            else 
                weapons = Weapon.ListFromJson(msg.Content);
            if (weapons.SingleOrDefault(x=> x.Name == weapon.Name) != null)
            {
                await arg.RespondAsync("이미 존재하는 이름의 물건이네요.", ephemeral: true);
                return;
            }
            weapons.Add(weapon);
            await arg.Channel.ModifyMessageAsync((ulong)WeaponPay.messageId, x => x.Content = Weapon.ListToJson(weapons));
            await arg.RespondAsync("DB 등록 성공!", ephemeral: true);
        }
    }
}
