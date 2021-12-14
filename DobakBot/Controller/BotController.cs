using Discord.WebSocket;
using DobakBot.Controller.Controller;
using DobakBot.Controller.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Controller
{
    class BotController
    {
        #region 싱글톤
        private BotController() { }
        private static readonly Lazy<BotController> _instance = new Lazy<BotController>(() => new BotController());
        public static BotController Instance { get { return _instance.Value; } }
        #endregion

        public AnimalRaceController animalRace = new AnimalRaceController();
        public DBController DB = new DBController();
        public WeaponPayController WeaponPay = new WeaponPayController();

    }
}
