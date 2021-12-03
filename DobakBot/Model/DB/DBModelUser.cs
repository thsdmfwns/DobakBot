using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Model
{
    class DBModelUser
    {
        public DBModelUser(ulong id, int coin)
        {
            this.id = id;
            this.coin = coin;
        }

        public DBModelUser(ulong idx, ulong id, int coin)
        {
            this.idx = idx;
            this.id = id;
            this.coin = coin;
        }

        public ulong idx { get; set; }
        public ulong id { get; set; }
        public int coin { get; set; }
    }
}
