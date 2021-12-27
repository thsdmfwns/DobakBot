using DobakBot.Utils;
using System.Globalization;

namespace DobakBot.Model
{
    enum WeaponPayKind
    {
        None,
        supply,
        Sell,
        DCSell,
    }

    class WeaponPay
    {
        public Weapon Weapon { get; set; }
        public WeaponPayKind Kind { get; set; }
        public int Count { get; set; }
        public string UserName { get; set; }
        public int Price => 
            Kind == WeaponPayKind.DCSell && Weapon.Kind == WeaponKind.Guns ? 
            (Weapon.Price - 200) * Count : 
            Weapon.Price * Count;

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
                case WeaponPayKind.DCSell:
                    return "할인 판매";
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
