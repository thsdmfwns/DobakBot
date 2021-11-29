using DobakBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Utils
{
    class Utility
    {
        public static string SlotCardToEmoticon(SlotCard card)
        {
            switch (card)
            {
                case SlotCard.None:
                    return "<a:SlotM:912594546163077120>";
                case SlotCard.Orange:
                    return ":tangerine:";
                case SlotCard.Grape:
                    return ":grapes:";
                case SlotCard.Cherry:
                    return ":cherries:";
                case SlotCard.Card:
                    return ":flower_playing_cards:";
                case SlotCard.Gate:
                    return ":shinto_shrine:";
                case SlotCard.Bell:
                    return ":bell:";
                default:
                    break;
            }

            return "";
        }

        public static string SlotResultToString(SlotResult slotResult)
        {
            switch (slotResult)
            {
                case SlotResult.Error:
                    break;
                case SlotResult.Lose:
                    return "　　　<a:7169rainbowwtf:912624770028290058>　𝕷𝖔𝖘𝖊　<a:7169rainbowwtf:912624770028290058>";
                case SlotResult.Win:
                    return "　　　:sunglasses:　𝓦𝓲𝓷　:sunglasses:";
                case SlotResult.JackPot:
                    return " :rocket:　🅹🅰🅲🅺 ​ 🅿🅾🆃　:rocket:";
                default:
                    break;
            }
            return string.Empty;
        }

        public static readonly string[] RaceRankImoticon = 
            {
            "",
            ":first_place:",
            ":second_place:",
            ":third_place:" 
            };
    }
}
