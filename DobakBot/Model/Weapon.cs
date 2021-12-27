using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Model
{
    enum WeaponKind
    {
        Etc,
        Guns,
        Ammo,
        Armor,
    }
    class Weapon
    {
        public Weapon(string name, int price, WeaponKind kind)
        {
            Name = name;
            Price = price;
            Kind = kind;
        }

        public string Name { get; private set; }
        public string Unit => WeaponKindToUnit(Kind);
        public WeaponKind Kind { get; private set; }
        public int Price { get; private set; }

        private string WeaponKindToUnit(WeaponKind kind)
        {
            switch (kind)
            {
                case WeaponKind.Etc:
                    return "개";
                case WeaponKind.Guns:
                    return "자루";
                case WeaponKind.Ammo:
                    return "탄창";
                case WeaponKind.Armor:
                    return "복";
                default:
                    break;
            }
            return null;
        }

        public static string WeaponKindToString(WeaponKind kind)
        {
            switch (kind)
            {
                case WeaponKind.Etc:
                    return "장비";
                case WeaponKind.Guns:
                    return "총기";
                case WeaponKind.Ammo:
                    return "총알";
                case WeaponKind.Armor:
                    return "방탄복";
                default:
                    break;
            }
            return null;
        }

        public static List<Weapon> GetList()
        {
            var list = new List<Weapon>();
            list.Add(new Weapon("Knife", 1000, WeaponKind.Etc));
            list.Add(new Weapon("GLOCK 17", 2000, WeaponKind.Guns));
            list.Add(new Weapon("Desert Eagle", 2200, WeaponKind.Guns));
            list.Add(new Weapon("Shotgun(12GAUGE)", 2500, WeaponKind.Guns));
            list.Add(new Weapon("MP5", 3500, WeaponKind.Guns));
            list.Add(new Weapon("9mm", 50, WeaponKind.Ammo));
            list.Add(new Weapon("7.62mm", 80, WeaponKind.Ammo));
            list.Add(new Weapon("5.56mm", 70, WeaponKind.Ammo));
            list.Add(new Weapon("Shotgun Shell", 30, WeaponKind.Ammo));
            list.Add(new Weapon(".22LR", 40, WeaponKind.Ammo));
            list.Add(new Weapon(".45acp", 30, WeaponKind.Ammo));
            list.Add(new Weapon(".50BMG", 20, WeaponKind.Ammo));
            return list;
        }

        public static List<Weapon> GetSellList()
        {
            var list = new List<Weapon>();
            list.Add(new Weapon("Knife", 1000, WeaponKind.Etc));
            list.Add(new Weapon("GLOCK 17", 3000, WeaponKind.Guns));
            list.Add(new Weapon("Desert Eagle", 3000, WeaponKind.Guns));
            list.Add(new Weapon("Shotgun(12GAUGE)", 3500, WeaponKind.Guns));
            list.Add(new Weapon("MP5", 4500, WeaponKind.Guns));
            list.Add(new Weapon("9mm", 50, WeaponKind.Ammo));
            list.Add(new Weapon("7.62mm", 80, WeaponKind.Ammo));
            list.Add(new Weapon("5.56mm", 70, WeaponKind.Ammo));
            list.Add(new Weapon("Shotgun Shell", 30, WeaponKind.Ammo));
            list.Add(new Weapon(".22LR", 40, WeaponKind.Ammo));
            list.Add(new Weapon(".45acp", 30, WeaponKind.Ammo));
            list.Add(new Weapon(".50BMG", 20, WeaponKind.Ammo));
            list.Add(new Weapon("경량 방탄복", 400, WeaponKind.Armor));
            list.Add(new Weapon("중량 방탄복", 1200, WeaponKind.Armor));
            return list;
        }
    }
}
