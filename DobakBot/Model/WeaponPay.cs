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
        DCSell,
    }

    class WeaponPay
    {
        public Weapon Weapon { get; set; }
        public WeaponPayKind Kind { get; set; }
        public int Count { get; set; }
        public string UserName { get; set; }
        public int Price => Weapon.Price * Count;
        public List<Weapon> Weapons => GetList();

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

        private List<Weapon> GetList()
        {
            switch (Kind)
            {
                case WeaponPayKind.None:
                    break;
                case WeaponPayKind.supply:
                    return Weapon.GetSupplyList();
                case WeaponPayKind.Sell:
                    return Weapon.GetSellList();
                case WeaponPayKind.DCSell:
                    return Weapon.GetDcSellList();
                default:
                    break;
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
