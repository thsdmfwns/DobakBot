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

        public Weapon(string name, int supplyPrice, int sellPrice, int dcSellPrice, WeaponKind kind)
        {
            Name = name;
            Kind = kind;
            SupplyPrice = supplyPrice;
            SellPrice = sellPrice;
            DcSellPrice = dcSellPrice;
        }

        public Weapon(string name, int Price , WeaponKind kind)
        {
            Name = name;
            Kind = kind;
            SupplyPrice = Price;
            SellPrice = Price;
            DcSellPrice = Price;
        }

        public string Name { get; private set; }
        public string Unit => WeaponKindToUnit(Kind);
        public WeaponKind Kind { get; private set; }

        private int SupplyPrice;
        private int SellPrice;
        private int DcSellPrice;

        public int GetPrice(WeaponPayKind payKind)
        {
            switch (payKind)
            {
                case WeaponPayKind.None:
                    break;
                case WeaponPayKind.supply:
                    return SupplyPrice;
                case WeaponPayKind.Sell:
                    return SellPrice;
                case WeaponPayKind.DCSell:
                    return DcSellPrice;
                default:
                    break;
            }
            return 0;
        }


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
                    return "벌";
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
            list.Add(new Weapon("Knife", 1000, 1500, 1300, WeaponKind.Etc));
            list.Add(new Weapon("GLOCK 17", 2000, 3000, 2800, WeaponKind.Guns));
            list.Add(new Weapon("Desert Eagle", 2200, 3300, 3100, WeaponKind.Guns));
            list.Add(new Weapon("Shotgun(12GAUGE)", 2500, 3800, 3600, WeaponKind.Guns));
            list.Add(new Weapon("MP5", 3500, 5300, 5100, WeaponKind.Guns));
            list.Add(new Weapon("K2", 4600, 10000, 9000, WeaponKind.Guns));
            list.Add(new Weapon("M4", 4800, 10200, 10000, WeaponKind.Guns));
            list.Add(new Weapon("9mm", 50, WeaponKind.Ammo));
            list.Add(new Weapon("7.62mm", 80, WeaponKind.Ammo));
            list.Add(new Weapon("5.56mm", 70, WeaponKind.Ammo));
            list.Add(new Weapon("Shotgun Shell", 30, WeaponKind.Ammo));
            list.Add(new Weapon(".22LR", 40, WeaponKind.Ammo));
            list.Add(new Weapon(".45acp", 30, WeaponKind.Ammo));
            list.Add(new Weapon(".50BMG", 20, WeaponKind.Ammo));
            list.Add(new Weapon("경량 케블라 조끼", 400, 800, 600, WeaponKind.Armor));
            list.Add(new Weapon("중량 케블라 조끼", 1200, 1500, 1200, WeaponKind.Armor));
            return list;
        }
    }
}