using Discord;
using DobakBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Controller
{
    class AnimalRaceController
    {
        private List<Animal> animals;
        public List<Animal> Animals {
            get 
            {
                return animals;
            } 
            set 
            {
                animals = value;
                Bettings = new Dictionary<string, List<BettingMember>>();
                foreach (var item in value)
                {
                    Bettings.Add(item.Name, new List<BettingMember>());
                }
            } 
        }
        public Dictionary<string, List<BettingMember>> Bettings { get; private set; }
        public int TotalMoney => GetTotalMoney();
        public bool IsSetting => Animals != null;

        public Embed GetBettingPanel()
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.Color = Color.Green;
            builder.Title = "경마 베팅";
            builder.Description = $"총 베팅금액 : {TotalMoney}";
            builder.AddField("출전마", GetAnimalsInfo(), inline : true);
            builder.AddField("베팅정보", GetAnimalsBetting(), inline: true);
            return builder.Build();
        }

        public int GetBettingMoney(string animalName)
        {
            int money = 0;
            Bettings[animalName].ForEach(x =>
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
            return (float) TotalMoney / betting;
        }


        public bool TryAddBetting(ulong id, string nickname, string animalName, int money)
        {
            if (!Bettings.ContainsKey(animalName)) return false;
            foreach (var item in Bettings[animalName])
            {
                if (item.ID == id)
                {
                    item.Money += money;
                    return true;
                }
            }
            Bettings[animalName].Add(new BettingMember(id, nickname, money));
            return true;
        }

        public void Clear()
        {
            Bettings = null;
            animals = null;
        }

        private string GetAnimalsInfo()
        {
            string text = string.Empty;
            for (int i = 0; i < Animals.Count; i++)
            {
                text += $"{i + 1}번마 : {Animals[i].Imoticon}{Animals[i].Name}\n";
            }
            return text;
        }

        private string GetAnimalsBetting()
        {
            string text = string.Empty;
            foreach (var item in Animals)
            {
                text += $"{item.Name}\n"+
                    $"　총베팅금액 : {GetBettingMoney(item.Name)}\n" +
                    $"　배당률 : {GetBettingOdds(item.Name):0.00}\n";
            }
            return text;
        }

        private int GetTotalMoney()
        {
            int money = 0;
            foreach (var item in Bettings)
            {
                money += GetBettingMoney(item.Key);
            }
            return money;
        }
    }
}
