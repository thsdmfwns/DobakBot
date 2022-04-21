using DobakBot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Model
{
    class Animal
    {
        public string Name { get; set; }
        public string Imoticon { get; set; }
        public int RaceRank { get; set; }
        public int CurrentDistance { get; private set; } = 0;
        public int MinSpeed { get; set; } = 1;
        public int MaxSpeed { get; set; } = 5;
        private Random rand = new Random();
        public void reset() => CurrentDistance = 0;
        public Animal(string name, string imoticon)
        {
            Name = name;
            Imoticon = imoticon;
        }
        public void Move(int raceDistance) 
        {
            CurrentDistance += rand.Next(MinSpeed, MaxSpeed);
            CurrentDistance = CurrentDistance > raceDistance ? raceDistance : CurrentDistance;
        }
        public string GetRaceContext(int raceDistance)
        {
            string ctx = string.Empty;
            var remainingDistance = raceDistance - CurrentDistance;
            ctx += remainingDistance <= 0 ? ":flag_white:" : ":checkered_flag:";
            for (int i = 0; i < remainingDistance; i++)
            {
                ctx += "　";
            }
            ctx += Imoticon;
            for (int i = 0; i < CurrentDistance; i++)
            {
                ctx += "　";
            }
            ctx += ":triangular_flag_on_post:";
            return ctx;
        }
    }
}
