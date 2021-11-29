﻿using Discord;
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
        public AnimalRace(List<Animal> animals, Dictionary<string, List<BettingMember>> bettings)
        {
            embedBuilder.Title = "경마?";
            embedBuilder.Color = Color.Red;
            Animals = animals;
            Bettings = bettings;
        }

        public int RaceDistance { get; set; } = 30;
        public List<Animal> Animals { get; set; }
        public Dictionary<string, List<BettingMember>> Bettings { get; set; }
        public Queue<Animal> ArrivedAnimals { get; private set; } = new Queue<Animal>();
        public bool isRaceDone { get; private set; } = false;

        private EmbedBuilder embedBuilder = new EmbedBuilder();
        private void CheckRace() =>
            isRaceDone = (ArrivedAnimals.Count == Animals.Count || ArrivedAnimals.Count > 2);
        private int LastRank = 1;

        public Embed GetEmbed(bool isStart = false)
        {
            if (isRaceDone) return null;
            if (!isStart) RunRace();
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
            var text = string.Empty;
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
                return "아쉽게도 무승부네요!";
            }
            var bettings = Bettings[winner.Name];
            var odd = GambleController.Instance.animalRace.GetBettingOdds(winner.Name);
            foreach (var item in bettings)
            {
                text += $"{item.Nickname} : {(int)(item.Money * odd)}\n";
            }
            return text;
        }
    }
}
