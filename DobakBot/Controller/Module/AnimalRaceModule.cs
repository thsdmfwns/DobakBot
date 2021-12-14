using Discord.Commands;
using Discord.WebSocket;
using DobakBot.Controller.Attribute;
using DobakBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Controller
{
    [RequireChannel("경마")]
    [Group("경마")]
    public class AnimalRaceModule : ModuleBase<SocketCommandContext>
    {
        private AnimalRaceController controller = BotController.Instance.animalRace;
        private DBController DB = BotController.Instance.DB;

        const string HelpStart = "경마 시작 : !경마 시작 별명#이모티콘 별명#이모티콘 \n" +
                    "(ex : !경마 시작 토끼#:rabbit2: 거북이#:turtle:)\n";
        const string HelpAdd = "경마 말 추가 : !경마 추가 별명#이모티콘\n" +
                    "(ex : !경마 추가 토끼#:rabbit2:)\n";
        const string HelpRemove = "경마 말 제거 : !경마 제거 베팅대상\n" +
                    "(ex : !경마 제거 토끼)\n";
        const string HelpBetting = "경마 베팅 : !경마 베팅 베팅대상 베팅금액\n" +
                    "(ex : !경마 베팅 토끼 100)\n";
        const string HelpCancel = "경마 취소 : !경마 취소\n";

        [Command("help")]
        public async Task HelpCommand()
        {
            var role = (Context.User as SocketGuildUser).Roles.SingleOrDefault(x => x.Name == "CASINO dealer");
            if(role == null)
            {
                await ReplyAsync(HelpBetting);
                return;
            }
            await ReplyAsync(HelpBetting + HelpStart + HelpAdd + HelpRemove + HelpCancel);
        }

        [RequireRole("CASINO dealer")]
        [Command("시작")]
        public async Task AnimalRaceStart()
        {
            await ReplyAsync(HelpStart);
        }

        [RequireRole("CASINO dealer")]
        [Command("시작")]
        public async Task AnimalRaceStart([Remainder] string args)
        {
            if (controller.IsSetting)
            {
                await ReplyAsync("이미 경마 베팅이 시작되어 있습니다. 취소를 원하면 !경마 취소를 입력해주세요.");
                return;
            }

            if (args == string.Empty)
            {
                await ReplyAsync(HelpStart);
                return;
            }

            var arg = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var animals = new List<Animal>();

            foreach (var item in arg)
            {
                var animal = item.Split('#');
                if (animal.Length < 2)
                {
                    await ReplyAsync(HelpStart);
                    return;
                }
                animals.Add(new Animal(animal[0], animal[1]));
            }

            controller.Animals = animals;
            await Context.Channel.SendMessageAsync("", false, controller.GetBettingPanel());
        }

        [RequireRole("CASINO dealer")]
        [Command("추가")]
        public async Task AnimalRaceAdd()
        {
            await ReplyAsync(HelpAdd);
        }

        [RequireRole("CASINO dealer")]
        [Command("추가")]
        public async Task AnimalRaceAdd([Remainder] string args)
        {
            if (!controller.IsSetting)
            {
                await ReplyAsync("시작을 먼저 해주세요!\n" + HelpStart);
                return;
            }
            var arg = args.Split('#', StringSplitOptions.RemoveEmptyEntries);
            if (arg.Length < 2)
            {
                await ReplyAsync(HelpAdd);
                return;
            }

            var animal = new Animal(arg[0], arg[1]);

            if (!controller.TryAddAnimal(animal))
            {
                await ReplyAsync("이미 존재하는 말의 이름입니다.");
                return;
            }
            await Context.Channel.SendMessageAsync("", false, controller.GetBettingPanel());
        }

        [RequireRole("CASINO dealer")]
        [Command("제거")]
        public async Task AnimalRaceRemove()
        {
            await ReplyAsync(HelpRemove);
        }

        [RequireRole("CASINO dealer")]
        [Command("제거")]
        public async Task AnimalRaceRemove([Remainder] string args)
        {
            if (!controller.IsSetting)
            {
                await ReplyAsync("시작을 먼저 해주세요!\n" + HelpStart);
                return;
            }

            if (args == string.Empty)
            {
                await ReplyAsync(HelpRemove);
                return;
            }

            if (!controller.TryRemoveAnimal(args))
            {
                await ReplyAsync("존재하지 않는 말의 이름입니다.");
                return;
            }
            await Context.Channel.SendMessageAsync("", false, controller.GetBettingPanel());
        }

        [Command("베팅")]
        public async Task AnimalRaceBetting()
        {
            await ReplyAsync(HelpBetting);
        }

        [Command("베팅")]
        public async Task AnimalRaceBetting([Remainder] string args)
        {
            if (!controller.IsSetting)
            {
                await ReplyAsync("시작을 먼저 해주세요!\n" + HelpStart);
                return;
            }

            var arg = args.Split(' ');
            if (args == string.Empty || arg.Length < 2)
            {
                await ReplyAsync(HelpBetting);
                return;
            }

            int money;
            if (!int.TryParse(arg[1], out money))
            {
                await ReplyAsync($"베팅 : {arg[1]} 일치 하지 않는 값입니다.\n" + HelpBetting);
                return;
            }

            var user = DB.GetUserByDiscordId(Context.User.Id);
            if (user == null)
            {
                await ReplyAsync($"등록되지 않은 사용자입니다.");
                return;
            }
            if (user.coin < money)
            {
                await ReplyAsync($"{Context.Guild.GetUser(Context.User.Id).Nickname}님의 :coin:이 부족합니다 ({user.coin - money}:coin:).");
                return;
            }
            if (!controller.TryAddBetting(
                Context.User.Id, Context.Guild.GetUser(Context.User.Id).Nickname, arg[0], money))
            {
                await ReplyAsync($"베팅 : {arg[0]} 일치 하지 않는 값입니다.\n" +  HelpBetting);
                return;
            }
            if (!DB.TrySubtractUserCoin(Context.User.Id, money))
            {
                await ReplyAsync("TrySubtractUserCoin => DB에러");
                return;
            }
            await Context.Channel.SendMessageAsync("", false, controller.GetBettingPanel());
        }

        [RequireRole("CASINO dealer")]
        [Command("경기")]
        public async Task AnimalRaceRun()
        {
            if (!controller.IsSetting)
            {
                await ReplyAsync("시작을 먼저 해주세요!\n"+HelpStart);
                return;
            }
            if(controller.TotalMoney == 0)
            {
                await ReplyAsync("베팅한사람이 아무도 없습니다.\n" + HelpBetting);
                return;
            }
            var winners = await RunAnimalRace();
            if (!DB.TryAddUsersCoin(winners)) await ReplyAsync("TryAddUsersCoin => DB에러");
            controller.Clear();
        }

        private async Task<BettingMembers> RunAnimalRace()
        {
            BettingMembers WinnerMembers;
            var race = controller.AnimalRace;
            var msg = await Context.Channel.SendMessageAsync("", false, race.GetEmbed(isStart: true));

            while (!race.isRaceDone)
            {
                await Task.Delay(1250);
                var embed = race.GetEmbed();
                if (embed == null) break;
                await msg.ModifyAsync(msg => msg.Embed = embed);
            }

            WinnerMembers = race.WinnerMembers;

            if (race.WinnerMembers == null)
            {
                await Task.Delay(5000);
                WinnerMembers = await RunAnimalRace();
            }
            return WinnerMembers;
        }

        [RequireRole("CASINO dealer")]
        [Command("취소")]
        public async Task AnimalRaceCancel()
        {
            controller.Clear();
            await ReplyAsync("경마 취소 완료.");
        }
    }
}
