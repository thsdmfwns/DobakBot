using Discord;
using DobakBot.Controller;
using DobakBot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Model
{
    class AnimalRace
    {
        public AnimalRace(List<Animal> animals, AnimalRaceBettings bettings, string raceName)
        {
            embedBuilder.Color = Color.Red;
            Animals = animals;
            Animals.ForEach(animal => animal.reset());
            Bettings = bettings;
            RaceName = raceName;
        }
        public string RaceName { get; set; }
        public int RaceDistance { get; set; } = 30;
        public List<Animal> Animals { get; set; }
        public AnimalRaceBettings Bettings { get; set; }
        public Queue<Animal> ArrivedAnimals { get; private set; } = new Queue<Animal>();
        public bool isRaceDone { get; private set; } = false;
        public BettingMembers WinnerMembers { get; set; }

        private EmbedBuilder embedBuilder = new EmbedBuilder();
        private void CheckRace() =>
            isRaceDone = (ArrivedAnimals.Count == Animals.Count || ArrivedAnimals.Count > 2);
        private int LastRank = 1;

        public Embed GetEmbed(bool isStart = false)
        {
            if (isRaceDone) return null;
            if (!isStart) RunRace();
            embedBuilder.Title = RaceName;
            var ctx = string.Empty;
            foreach (var animal in Animals)
            {
                ctx += animal.GetRaceContext(RaceDistance);
                ctx += "\n\n";
            }
            embedBuilder.Description = ctx;
            if (isRaceDone)
            {
                embedBuilder.Color = Color.Blue;
                embedBuilder.Fields.Clear();
                embedBuilder.AddField("결과", GetWinner(), inline: true);
                embedBuilder.AddField("승리자들", GetDividen(), inline: true);
            }
            return embedBuilder.Build();
        }

        private void RunRace()
        {
            bool isWin = false;
            if (isRaceDone) return;
            foreach (var animal in Animals)
            {
                if (ArrivedAnimals.Contains(animal))
                {
                    continue;
                }
                if (animal.CurrentDistance >= RaceDistance)
                {
                    isWin = true;
                    animal.RaceRank = LastRank;
                    ArrivedAnimals.Enqueue(animal);
                    continue;
                }
                animal.Move(RaceDistance);
            }
            if (isWin) LastRank++;
            CheckRace();
        }


        private string GetWinner()
        {
            var WinnerCtx = string.Empty;
            for (int i = 0; i < 3; i++)
            {
                if (ArrivedAnimals.Count == 0) break;
                var animal = ArrivedAnimals.Dequeue();
                WinnerCtx += $"{Utility.RaceRankImoticon[animal.RaceRank]}   {animal.Name}#{animal.Imoticon}\n";
            }
            return WinnerCtx;
        }

        private string GetDividen()
        {
            var winnerCount = 0;
            Animal winner = new Animal("","");
            foreach (var item in Animals)
            {
                if (item.RaceRank == 1)
                {
                    winner = item;
                    winnerCount++;
                }
            }
            if (winnerCount > 1)
            {
                return "아쉽게도 무승부네요! \n 5초후 재경기가 펼쳐집니다!";
            }
            WinnerMembers = Bettings[winner.Name];
            var odd = Bettings.GetBettingOdds(winner.Name);
            WinnerMembers.SetOdd(odd);
            return WinnerMembers.ToString() ?? "뭐야 아무도 베팅을 안했잖아?";
        }
    }
}
