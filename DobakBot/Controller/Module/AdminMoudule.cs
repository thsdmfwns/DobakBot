using Discord;
using Discord.Commands;
using Discord.Rest;
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
    [RequireRole("Discord Admin")]
    public class AdminMoudule : ModuleBase<SocketCommandContext>
    {
        private readonly DBController DB = BotController.Instance.DB;
        [Command("입장버튼")]
        public async Task EnterButtonSpawn()
        {
            var button = new ComponentBuilder().WithButton("카지노 입장하기", "casino_enter", style: ButtonStyle.Secondary, emote: new Emoji("\uD83C\uDFB4"));
            var builder = new EmbedBuilder();
            builder.Title = ":shinto_shrine: 카지노 입장";
            builder.Color = Color.Green;
            builder.Description = 
                "카지노에 입장하시기 전, \n 디스코드 채널의 별명을 IC상의 이름으로 설정해주세요.\n" +
                "IC상에서의 코인 충전과 코인 환전을 위해 필요합니다.";
            await ReplyAsync("", embed: builder.Build(), components: button.Build());
        }

        [Command("지갑버튼")]
        public async Task WalletButtonSpawn()
        {
            var buttons = new ComponentBuilder()
                .WithButton("지갑보기", "customer_Wallet", style: ButtonStyle.Primary)
                .WithButton("충전하기", "customer_pay", style: ButtonStyle.Success)
                .WithButton("환전하기", "customer_return", style: ButtonStyle.Danger);
            var builder = new EmbedBuilder();
            builder.Title = "환전소";
            builder.Color = Color.Blue;
            await ReplyAsync("", embed: builder.Build(), components: buttons.Build());
        }

        [Command("슬롯머신버튼")]
        public async Task SlotButtonSpawn()
        {
            var buttons = new ComponentBuilder()
                .WithButton("이용하기", "slot_roomCreate", style: ButtonStyle.Success);
            var builder = new EmbedBuilder();
            builder.Title = "슬롯머신 이용하기";
            builder.Color = Color.Blue;
            await ReplyAsync("", embed: builder.Build(), components: buttons.Build());
        }

        [Command("문의버튼")]
        public async Task SlotInfoButtonSpawn()
        {
            var buttons = new ComponentBuilder()
                .WithButton("이용문의", "info_create", style: ButtonStyle.Success);
            var builder = new EmbedBuilder();
            builder.Title = "이용문의";
            builder.Color = Color.Blue;
            await ReplyAsync("", embed: builder.Build(), components: buttons.Build());
        }

        [Command("레이스도우미버튼")]
        public async Task RaceButtonSpawn()
        {
            var buttons = new ComponentBuilder()
                .WithButton("경기 만들기", "race_make", style: ButtonStyle.Primary)
                .WithButton("경기 시작", "race_start", style: ButtonStyle.Success)
                .WithButton("경기 취소", "race_cancel", style: ButtonStyle.Danger);
            var builder = new EmbedBuilder();
            builder.Title = "레이스 도우미";
            builder.Color = Color.Blue;
            await ReplyAsync("", embed: builder.Build(), components: buttons.Build());
        }

        [Command("DB")]
        public async Task WeaponDBSpawn()
        {
            var buttons = new ComponentBuilder()
                .WithButton("DB 추가", "weapon_add", style: ButtonStyle.Primary)
                .WithButton("DB 제거", "weapon_remove", style: ButtonStyle.Danger)
                .WithButton("DB 갱신", "weapon_apply", style: ButtonStyle.Success);
            await ReplyAsync("empty", components: buttons.Build());
        }

        [Command("장부")]
        public async Task WeaponPaySpawn()
        {
            var buttons = new ComponentBuilder()
                .WithButton("무기 보급", "weaponpay_supply", style: ButtonStyle.Primary)
                .WithButton("무기 판매", "weaponpay_sell", style: ButtonStyle.Success);
            var builder = new EmbedBuilder();
            builder.Title = "장부 도우미";
            builder.Color = Color.Blue;
            await ReplyAsync("", embed: builder.Build(), components: buttons.Build());
        }

        [Command("판매글버튼")]
        public async Task SellingSpawn()
        {
            var buttons = new ComponentBuilder()
                .WithButton("판매글 작성하기", "sell_upload", style: ButtonStyle.Success)
                .WithButton("상시 판매글 작성하기", "sell_alwayupload", style: ButtonStyle.Primary);
            var builder = new EmbedBuilder();
            builder.Title = "판매글 도우미";
            builder.Color = Color.Blue;
            await ReplyAsync("", embed: builder.Build(), components: buttons.Build());
        }

        [Command("우수회원신청버튼")]
        public async Task UpgradSellingSpawn()
        {
            var buttons = new ComponentBuilder()
                .WithButton("신청하기", "sell_upgrade", style: ButtonStyle.Primary);
            var builder = new EmbedBuilder();
            builder.Title = "🦹🏻우수회원 신청";
            builder.Description = "우수 회원이 되는데 필요한 자격조건은 아무것도 필요하지 않습니다.";
            builder.Color = Color.Blue;
            await ReplyAsync("", embed: builder.Build(), components: buttons.Build());
        }

        [Command("보관")]
        public async Task CloseChannel()
        {
            var channel = Context.Channel as SocketTextChannel;
            var guild = channel.Guild;
            var ct = guild.CategoryChannels.Single(x => x.Name == "보관소");
            await ReplyAsync($"채널 이름 : {channel.Id} | 카테고리 이름 : {channel.Category.Name}");
            await channel.ModifyAsync(x => x.CategoryId = ct.Id);
        }

        [Command("지원버튼")]
        public async Task FactionJoin()
        {
            var buttons = new ComponentBuilder()
                .WithButton("📝 입사 지원", "faction_join", style: ButtonStyle.Primary);
            var builder = new EmbedBuilder().WithImageUrl("https://cdn.discordapp.com/attachments/991332432873791488/991354272660852776/daeweol_will_4.jpg");
            builder.Title = "📝 입사 지원";
            builder.Description = "대월그룹에 오신 것을 환영합니다.\n아래의 입사 지원 버튼을 통해 도움을 드리겠습니다.";
            builder.Color = Color.Blue;
            await ReplyAsync("", embed: builder.Build(), components: buttons.Build());
        }

        [Command("신고버튼")]
        public async Task FactionReport()
        {
            var buttons = new ComponentBuilder()
                .WithButton("🚨 사원 신고", "faction_report", style: ButtonStyle.Danger);
            var builder = new EmbedBuilder();
            builder.Title = "🚨 대월그룹 사원 신고";
            builder.Description = "아래의 신고 버튼을 통해 도움을 드리겠습니다.";
            builder.Color = Color.Orange;
            await ReplyAsync("", embed: builder.Build(), components: buttons.Build());
        }

        [Command("권한요청버튼")]
        public async Task FactionPermission()
        {
            var buttons = new ComponentBuilder()
                .WithButton("🔔 권한 요청", "faction_permission", style: ButtonStyle.Success);
            var builder = new EmbedBuilder();
            builder.Title = "🔔 권한 요청";
            builder.Description = "권한 필요시, 아래의 버튼을 눌러 요청 부탁드립니다.";
            builder.Color = Color.Green;
            await ReplyAsync("", embed: builder.Build(), components: buttons.Build());
        }

        [Command("지갑")]
        public async Task GetWallat([Remainder] string nick)
        {
            await Context.Guild.DownloadUsersAsync();
            var discorUser = Context.Guild.Users.FirstOrDefault(x => x.Nickname == nick);
            if (discorUser == null)
            {
                await ReplyAsync($"{nick}를 찾을수 없음.");
                return;
            }
            var user = DB.GetUserByDiscordId(discorUser.Id);
            if (user == null)
            {
                await ReplyAsync($"{discorUser.Nickname}은 등록되지 않은 유저");
                return;
            }
            await ReplyAsync($"{discorUser.Nickname}의 보유 코인 : {user.coin}");
        }

        [Command("clear")]
        public async Task ClearMessages()
        {
            var msgs = await Context.Channel.GetMessagesAsync().SingleAsync();
            foreach (var item in msgs)
            {
                await item.DeleteAsync();
                await Task.Delay(1050);
            }
        }

        [Command("clear")]
        public async Task ClearMessages([Remainder] int count)
        {
            var msgs = await Context.Channel.GetMessagesAsync(limit:count).SingleAsync();
            foreach (var item in msgs)
            {
                await item.DeleteAsync();
                await Task.Delay(1050);
            }
        }
    }
}
