using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Model
{
    class Repayment
    {
        public int Money { get; set; }
        public string Name { get; set; }
        public DateTime TimeStamp => DateTime.Now;

        public override string ToString()
        {
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            return $"{TimeStamp.Month:00}/{TimeStamp.Day:00} 　 {Name} 　 {Money.ToString("C0", nfi)}";
        }
    }
}
