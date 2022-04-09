using Newtonsoft.Json;
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

        public static List<Weapon> ListFromJson(string json) => JsonConvert.DeserializeObject<List<Weapon>>(json);
        public static string ListToJson(List<Weapon> weapons) => JsonConvert.SerializeObject(weapons);
    }
}