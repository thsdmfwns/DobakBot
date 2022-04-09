using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Model
{
    class Weapon
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public int Price { get; set; }
        public int SellPrice { get; set; }

        public static List<Weapon> ListFromJson(string json) => JsonConvert.DeserializeObject<List<Weapon>>(json);
        public static string ListToJson(List<Weapon> weapons) => JsonConvert.SerializeObject(weapons);
    }
}