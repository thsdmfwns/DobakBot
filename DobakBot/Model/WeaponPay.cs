using DobakBot.Utils;
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
        public int Price => Weapon.Price * Count;

        public override string ToString()
        {
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            return $"{UserName}　/　{Weapon.Name}　/　{Count+Weapon.Unit}　/　{Price.ToString("C0", nfi)}　/　{Utility.WeaponPayKindToString(Kind)}";
        }
    }
}
