using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DobakBot.Model
{
    internal class CoinReceipt
    {
        public CoinReceipt(string nickname, int money, ulong id, bool isPay)
        {
            Kind = isPay ? "환전" : "충전";
            Nickname = nickname;
            Money = money;
            Id = id;
            IsPay = isPay;
        }

        public string Kind { get; private set; }
        public string Nickname { get; private set; }
        public int Money { get; private set; }
        public ulong Id { get; private set; }
        public bool IsPay { get; private set; }


        static public string toJson(CoinReceipt receipt) => JsonConvert.SerializeObject(receipt);
        static public CoinReceipt fromJson(string receipt) => JsonConvert.DeserializeObject<CoinReceipt>(receipt);
    }
}
