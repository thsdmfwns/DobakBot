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

        [Command("보관")]
        public async Task CloseChannel()
        {
            var channel = Context.Channel as SocketTextChannel;
            var guild = channel.Guild;
            var ct = guild.CategoryChannels.Single(x => x.Name == "보관소");
            await ReplyAsync($"채널 이름 : {channel.Id} | 카테고리 이름 : {channel.Category.Name}");
            await channel.ModifyAsync(x => x.CategoryId = ct.Id);
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
