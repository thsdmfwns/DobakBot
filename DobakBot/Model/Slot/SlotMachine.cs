using Discord;
using Discord.Commands;
using Discord.Rest;
using DobakBot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DobakBot.Model
{
    class SlotMachine
    {
        public List<SlotCard> resultCards { get; private set; } = new List<SlotCard>();
        public SlotResult SlotResult { get; private set; }
        public int Coin { get; set; }
        public int Odd { get; set; } = 0;

        public int ResultCoin => Coin * Odd;

        private Dictionary<SlotCard, int> ResultMap = new Dictionary<SlotCard, int>();
        private List<SlotCard> Cards = new List<SlotCard>();

        public SlotMachine(int coin)
        {
            Coin = coin;
            for (int i = 1; i < (int)SlotCard.Max; i++)
            {
                ResultMap.Add((SlotCard)i, 0);
                Cards.Add((SlotCard)i);
            }
        }

        public async Task SetResult()
        {
            Random random = new Random();

            for (int i = 0; i < 3; i++)
            {
                var num = random.Next(0, Cards.Count);
                var card = Cards.ElementAt(num);
                ResultMap[card]++;
                resultCards.Add(card);
                await Task.Delay(1);
            }
            SlotResult = getSlotResult();
        }

        public int SlotCardToOdd(SlotCard card)
        {
            switch (card)
            {
                case SlotCard.None:
                    return 0;
                case SlotCard.Orange:
                    return 8;
                case SlotCard.Grape:
                    return 6;
                case SlotCard.Cherry:
                    return 3;
                case SlotCard.Card:
                    return 12;
                case SlotCard.Gate:
                    return 4;
                case SlotCard.Bell:
                    return 10;
                default:
                    break;
            }
            return 0;
        }

        private SlotResult getSlotResult()
        {
            if (ResultMap.ContainsValue(3))
            {
                var result = ResultMap.Single(x => x.Value == 3).Key;
                Odd = SlotCardToOdd(result);
                return SlotResult.JackPot;
            }
            if (ResultMap[SlotCard.Cherry] > 1)
            {
                Odd = 2;
                return SlotResult.Win;
            }
            return SlotResult.Lose;
        }

        public List<Embed> getEmbeds(string userNickname)
        {

            EmbedBuilder eb = new EmbedBuilder();
            var list = new List<Embed>();
            var ctxs = getContexts();

            eb.Title = $@"{userNickname}의 슬롯머신 <a:9885peepogamble:912598782343020556>";
            eb.Color = Color.Red;
            eb.Description = ctxs[0];
            list.Add(eb.Build());

            eb.Description = ctxs[1];
            list.Add(eb.Build());

            eb.Description = ctxs[2];
            list.Add(eb.Build());

            eb.Color = Color.Blue;
            eb.Description = ctxs[3];
            var ctx = Odd > 0 ? $"+{ResultCoin}" : $"-{Coin}";
            eb.AddField("결과", $"{userNickname} : {ctx}:coin:");
            list.Add(eb.Build());

            return list;
        }



        private List<string> getContexts()
        {
            var list = new List<string>();

            var emoticons = new List<string>();
            foreach (var item in resultCards)
            {
                emoticons.Add(Utils.Utility.SlotCardToEmoticon(item));
            }

            string slotEmoticon = Utils.Utility.SlotCardToEmoticon(SlotCard.None);

            string[] ctxs =
            {
                " ​  ​  ​ 🆂🅻🅾🆃 ​ 🅼🅰🅲🅷🅸🅽🅴 \n",
                "╔══════════╗\n",
                "\n",
                "",
                "\n",
                "╚══════════╝\n",
                "\n",
                "",
            };

            ctxs[7] = "　　:question:　　:question:　　:question:";

            ctxs[3] = $"　　{slotEmoticon}　|　{slotEmoticon}　|　{slotEmoticon} \n";
            list.Add(string.Concat(ctxs));
            ctxs[3] = $"　　{emoticons[0]}　|　{slotEmoticon}　|　{slotEmoticon} \n";
            list.Add(string.Concat(ctxs));
            ctxs[3] = $"　　{emoticons[0]}　|　{emoticons[1]}　|　{slotEmoticon} \n";
            list.Add(string.Concat(ctxs));
            ctxs[3] = $"　　{emoticons[0]}　|　{emoticons[1]}　|　{emoticons[2]} \n";
            ctxs[7] = Utility.SlotResultToString(SlotResult);
            list.Add(string.Concat(ctxs));
            return list;
        }

    }
}

