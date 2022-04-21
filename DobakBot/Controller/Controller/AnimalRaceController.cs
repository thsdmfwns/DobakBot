using Discord;
using Discord.Rest;
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
        public RestTextChannel Channel { get; set; }
        public ulong? BettingMsgId { get; set; }
        public string RaceName { get; set; }
        public AnimalRaceBettings Bettings { get; private set; }
        public List<Animal> Animals => Bettings.Keys.ToList();
        public int TotalMoney => Bettings.TotalMoney;
        public bool IsSetting => Bettings != null;
        public bool IsRunning = false;
        public AnimalRace MakeAnimalRace()
        {
            IsRunning = true;
            return new AnimalRace(Bettings, RaceName);
        }
        public void MakeBettings(List<Animal> animals)
        {
            Bettings = new AnimalRaceBettings();
            animals.ForEach(item => Bettings.Add(item, new BettingMembers()));
        }
        public bool TryAddAnimal(Animal animal)
        {
            if (Bettings.CheckAnimalByName(animal.Name))
                return false;
            Bettings.Add(animal, new BettingMembers());
            return true;
        }
        public bool TryRemoveAnimal(string animalName)
        {
            Animal animal = Bettings.GetAnimalByName(animalName);
            if (animal == null) return false;
            Bettings.Remove(animal);
            return true;
        }
        public Embed GetBettingPanel()
        {
            EmbedBuilder builder = new EmbedBuilder()
            {
                Color = Color.Green,
                Title = "경마 베팅",
                Description = $"총 베팅금액 : {TotalMoney}",
            };
            builder.AddField("출전마", GetAnimalsInfo(), inline : true);
            builder.AddField("베팅정보", GetAnimalsBetting(), inline: true);
            return builder.Build();
        }
        public bool TryAddBetting(BettingMember member, string animalName)
        {
            var animal = Bettings.GetAnimalByName(animalName);
            if (Bettings.CheckMemberById(member.ID)) return false;
            Bettings[animal].Add(member);
            return true;
        }

        public void Clear()
        {
            Bettings = null;
            Channel = null;
            BettingMsgId = null;
            IsRunning = false;
        }

        private string GetAnimalsInfo()
        {
            string text = string.Empty;
            for (int i = 0; i < Animals.Count; i++)
                text += $"{i + 1}번마 : {Animals[i].Imoticon}{Animals[i].Name}\n";
            return text;
        }

        private string GetAnimalsBetting()
        {
            string text = string.Empty;
            Animals.ForEach(item => text += 
            $"{item.Name}\n" + 
            $"　총베팅금액 : {Bettings.GetBettingMoney(item.Name)}\n" +
            $"　배당률 : {Bettings.GetBettingOdds(item.Name):0.00}\n");
            return text;
        }
    }
}
