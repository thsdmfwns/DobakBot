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
        public CoinReceipt(string nickname, int money, ulong id, bool isPay, int tip = 0)
        {
            Kind = isPay ? "충전" : "환전";
            Nickname = nickname;
            Money = money;
            Tip = tip;
            Id = id;
        }

        public string Kind { get; private set; }
        public string Nickname { get; private set; }
        public int TotalMoney => Money - Tip;
        public int Money { get; private set; }
        public int Tip { get; private set; }
        public ulong Id { get; private set; }
        public bool IsPay => Kind == "충전";
        static public string toJson(CoinReceipt receipt) => JsonConvert.SerializeObject(receipt);
        static public CoinReceipt fromJson(string receipt) => JsonConvert.DeserializeObject<CoinReceipt>(receipt);
    }
}
