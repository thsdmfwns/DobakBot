using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Model
{

    class Weapon
    {
        public Weapon(string name, int price, string unit)
        {
            Name = name;
            Price = price;
            Unit = unit;
        }

        public string Name { get; private set; }
        public string Unit { get; private set; }
        public int Price { get; private set; }

        public static IReadOnlyList<Weapon> GetList()
        {
            var list = new List<Weapon>();
            list.Add(new Weapon("Knife", 1000, "자루"));
            list.Add(new Weapon("GLOCK 17", 2000, "정"));
            list.Add(new Weapon("Desert Eagle", 2200, "정"));
            list.Add(new Weapon("12GAUGE Shotgun", 2500, "정"));
            list.Add(new Weapon("MP5", 3500, "정"));
            list.Add(new Weapon("9mm", 50, "탄창"));
            list.Add(new Weapon("7.62mm", 80, "탄창"));
            list.Add(new Weapon("5.56mm", 70, "탄창"));
            list.Add(new Weapon("Shotgun Shell", 30, "탄창"));
            list.Add(new Weapon(".22LR", 40, "탄창"));
            list.Add(new Weapon(".45acp", 30, "탄창"));
            list.Add(new Weapon(".50BMG", 20, "탄창"));
            return list.AsReadOnly();
        }
    }
}
