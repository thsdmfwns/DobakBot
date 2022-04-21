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
        public AnimalRace(AnimalRaceBettings bettings, string raceName)
        {
            Bettings = bettings;
            RaceName = raceName;
            embedBuilder.Color = Color.Red;
            animals.ForEach(animal => animal.reset());
        }
        public string RaceName { get; set; }
        public int RaceDistance { get; set; } = 30;
        public AnimalRaceBettings Bettings { get; set; }
        private List<Animal> animals => Bettings.Keys.ToList();
        public Queue<Animal> ArrivedAnimals { get; private set; } = new Queue<Animal>();
        public BettingMembers WinnerMembers { get; set; }

        private EmbedBuilder embedBuilder = new EmbedBuilder();
        public bool CheckRace => ArrivedAnimals.Count == animals.Count || ArrivedAnimals.Count > 2;
        private int LastRank = 1;
        public Embed GetEmbed(bool isStart = false)
        {
            if (CheckRace) return null;
            if (!isStart) RunRace();
            embedBuilder.Title = RaceName;
            var ctx = string.Empty;
            animals.ForEach(animal => ctx += animal.GetRaceContext(RaceDistance) + "\n\n");
            embedBuilder.Description = ctx;
            if (CheckRace)
            {
                embedBuilder.Color = Color.Blue;
                embedBuilder.Fields.Clear();
                embedBuilder.AddField("레이스 결과", GetWinner(), inline: true);
                embedBuilder.AddField("축하합니다.", GetDividen(), inline: true);
            }
            return embedBuilder.Build();
        }

        private void RunRace()
        {
            bool isWin = false;
            if (CheckRace) return;
            foreach (var animal in animals)
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
            if (WinnerMembers != null) return WinnerMembers.Count > 0 ? WinnerMembers.ToString() : "뭐야 아무도 베팅을 안했잖아?";
            var winner = Bettings.Keys.Where(x => x.RaceRank == 1);
            if (winner.Count() > 1)
            {
                return "아쉽게도 무승부네요! \n 5초후 재경기가 펼쳐집니다!";
            }
            WinnerMembers = Bettings[winner.First()];
            var odd = Bettings.GetBettingOdds(winner.First().Name);
            WinnerMembers.SetOdd(odd);
            return WinnerMembers.Count > 0 ? WinnerMembers.ToString() : "뭐야 아무도 베팅을 안했잖아?";
        }
    }
}
