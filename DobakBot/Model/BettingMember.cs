using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Model
{
    class BettingMember
    {
        public BettingMember(ulong id, string nickname, int money)
        {
            ID = id;
            Nickname = nickname;
            Money = money;
        }

        public ulong ID { get; set; }
        public string Nickname { get; set; }
        public int Money { get; set; }
    }
}
