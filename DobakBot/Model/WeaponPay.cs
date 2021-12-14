using DobakBot.Utils;
using System.Globalization;

namespace DobakBot.Model
{
    class WeaponPay
    {
        public Weapon Weapon { get; set; }
        public WeaponPayKind Kind { get; set; }
        public int Count { get; set; }
        public string UserName { get; set; }
        public int Price => (int)(Utility.WeaponToMoney(Weapon) * Count);

        public override string ToString()
        {
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            return $"{UserName}　/　{Weapon}　/　{Count}　/　{Price.ToString("C0", nfi)}　/　{Utility.WeaponPayKindToString(Kind)}";
        }
    }
}
