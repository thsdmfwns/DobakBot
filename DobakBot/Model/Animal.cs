﻿using DobakBot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Model
{
    enum AnimalKind
    {
        None,
        Rabbit,
        Turtle,
    }
    class Animal
    {
        public string Name { get; set; }
        public string Imoticon { get; set; }
        public int RaceRank { get; set; }
        public int CurrentDistance { get; private set; } = 0;
        public int MinSpeed { get; set; } = 1;
        public int MaxSpeed { get; set; } = 3;

        private Random rand = new Random();

        public Animal(string name, string imoticon)
        {
            Name = name;
            Imoticon = imoticon;
            MaxSpeed = rand.Next(3, 5);
        }

        public void Move(int raceDistance) 
        {
            CurrentDistance += rand.Next(MinSpeed, MaxSpeed);
            if (CurrentDistance > raceDistance) CurrentDistance = raceDistance;
        }

        public string GetRaceContext(int raceDistance)
        {
            string ctx = "";
            ctx += raceDistance - CurrentDistance <= 0 ? ":flag_white:" : ":checkered_flag:";
            for (int i = 0; i < raceDistance - CurrentDistance; i++)
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
