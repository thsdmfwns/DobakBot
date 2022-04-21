using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Model
{
    class AnimalRaceBettings : Dictionary<Animal, BettingMembers>
    {
        public int TotalMoney => GetTotalMoney();
        public Animal GetAnimalByName(string name) => Keys.SingleOrDefault(x => x.Name == name);
        public bool CheckAnimalByName(string name) => Keys.Any(x => x.Name == name);
        public bool CheckMemberById(ulong id) => Values.Any(x => x.Any(item => item.ID == id));
        public int GetBettingMoney(string animalName)
        {
            int money = 0;
            this[Keys.Single(x=> x.Name == animalName)].ForEach(x => money += x.Money);
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
            Keys.ToList().ForEach(x => money += GetBettingMoney(x.Name));
            return money;
        }
    }
}
