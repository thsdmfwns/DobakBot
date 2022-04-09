using DobakBot.Utils;
using System.Collections.Generic;
using System.Globalization;

namespace DobakBot.Model
{
    enum WeaponPayKind
    {
        None,
        supply,
        Sell,
    }

    class WeaponPay
    {
        public Weapon Weapon { get; set; }
        public WeaponPayKind Kind { get; set; }
        public int Count { get; set; }
        public string UserName { get; set; }
        public List<Weapon> Weapons { get; set; }

        public int Price => (Kind == WeaponPayKind.supply ? Weapon.Price : Weapon.SellPrice)* Count;

        public static string WeaponPayKindToString(WeaponPayKind kind)
        {
            switch (kind)
            {
                case WeaponPayKind.None:
                    break;
                case WeaponPayKind.supply:
                    return "보급";
                case WeaponPayKind.Sell:
                    return "판매";
            }
            return null;
        }

        public override string ToString()
        {
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            return $"{UserName}　/　{Weapon.Name}　/　{Count+Weapon.Unit}　/　{Price.ToString("C0", nfi)}　/　{WeaponPayKindToString(Kind)}";
        }
    }
}
