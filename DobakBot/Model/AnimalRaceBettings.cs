using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Model
{
    class AnimalRaceBettings : Dictionary<string, BettingMembers>
    {
        public int TotalMoney => GetTotalMoney();
        public int GetBettingMoney(string animalName)
        {
            int money = 0;
            this[animalName].ForEach(x =>
            {
                money += x.Money;
            });
            return money;
        }

        public float GetBettingOdds(string animalName)
        {
            var betting = GetBettingMoney(animalName);
            if (TotalMoney == 0 || betting == 0)
                return 0;
            return (float)TotalMoney / betting;
        }

        private int GetTotalMoney()
        {
            int money = 0;
            foreach (var item in this)
            {
                money += GetBettingMoney(item.Key);
            }
            return money;
        }
    }
}
