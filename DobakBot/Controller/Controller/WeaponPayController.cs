using DobakBot.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Controller.Controller
{
    class WeaponPayController
    {
        public ulong? messageId { get; set; }
        public List<Weapon> Weapons { get; set; } = new List<Weapon>();
        public ConcurrentDictionary<ulong, WeaponPay> WeaponPayMap { get; private set; } = new ConcurrentDictionary<ulong, WeaponPay>();
    }
}
