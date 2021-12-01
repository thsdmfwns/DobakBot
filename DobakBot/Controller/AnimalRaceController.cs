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
                Bettings = new AnimalRaceBettings();
                foreach (var item in value)
                {
                    Bettings.Add(item.Name, new List<BettingMember>());
                }
            } 
        }
        public AnimalRaceBettings Bettings { get; private set; }
        public int TotalMoney => Bettings.TotalMoney;
        public bool IsSetting => Animals != null && Bettings != null;
        public AnimalRace GetAnimalRace => new AnimalRace(Animals, Bettings);

        public bool TryAddAnimal(Animal animal)
        {
            if (GetAnimalByName(animal.Name) != null || Bettings.ContainsKey(animal.Name))
                return false;
            Animals.Add(animal);
            Bettings.Add(animal.Name, new List<BettingMember>());
            return true;
        }

        public bool TryRemoveAnimal(string animalName)
        {
            Animal animal = GetAnimalByName(animalName);
            if (animal == null || !Bettings.ContainsKey(animal.Name)) 
                return false;
            Animals.Remove(animal);
            Bettings.Remove(animalName);
            return true;
        }

        public Animal GetAnimalByName(string animal)
        {
            foreach (var item in Animals)
            {
                if (item.Name == animal)
                {
                    return item;
                }
            }
            return null;
        }

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

        public int GetBettingMoney(string animalName) => Bettings.GetBettingMoney(animalName);

        public float GetBettingOdds(string animalName) => Bettings.GetBettingOdds(animalName);


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
    }
}
