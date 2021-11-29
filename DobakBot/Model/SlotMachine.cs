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

        private Dictionary<SlotCard, int> ResultMap = new Dictionary<SlotCard, int>();

        public SlotMachine()
        {
            for (int i = 1; i < (int)SlotCard.Max; i++)
            {
                ResultMap.Add((SlotCard)i, 0);
            }

        }

        public async Task setValue()
        {
            Random random = new Random();

            for (int i = 0; i < 3; i++)
            {
                var num = random.Next((int)SlotCard.Orange, (int)SlotCard.Max);
                ResultMap[(SlotCard)num]++;
                resultCards.Add((SlotCard)num);
                await Task.Delay(10);
            }

            SlotResult = getSlotResult();
        }

/*        private async Task setValue(SlotCard card)
        {
            for (int i = 0; i < 3; i++)
            {
                ResultMap[card]++;
                resultCards.Add(card);
                await Task.Delay(10);
            }

            SlotResult = getSlotResult();
        }*/

        private SlotResult getSlotResult()
        {
            if (ResultMap.ContainsValue(3))
                return SlotResult.JackPot;
            if (ResultMap[SlotCard.Cherry] > 0)
                return SlotResult.Win;
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

