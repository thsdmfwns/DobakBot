using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DobakBot
{
    class Program
    {
        DiscordSocketClient client; 
        CommandService commands;

        static void Main(string[] args)
        {
            new Program().BotMain().GetAwaiter().GetResult();
        }

        public async Task BotMain()
        {
            client = new DiscordSocketClient(new DiscordSocketConfig()
            {   
                LogLevel = LogSeverity.Verbose                              
            });
            commands = new CommandService(new CommandServiceConfig()        
            {
                LogLevel = LogSeverity.Verbose                              
            });

            client.Log += OnClientLogReceived;
            commands.Log += OnClientLogReceived;

            await client.LoginAsync(TokenType.Bot, "OTEyMzg3MDEzNzE2NjA2OTc4.YZvMnw.tSUKo_CXfjCM4wnYdTciy08NtC4"); 
            await client.StartAsync();                         

            client.MessageReceived += OnClientMessage;
            await commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
            await Task.Delay(-1);   
        }

        private async Task OnClientMessage(SocketMessage arg)
        {
            SocketUserMessage message = arg as SocketUserMessage;
            if (message == null) return;

            int pos = 0;

            if (!(message.HasCharPrefix('!', ref pos) ||
                message.HasMentionPrefix(client.CurrentUser, ref pos)) ||
                message.Author.IsBot)
                return;

            var context = new SocketCommandContext(client, message);

            await commands.ExecuteAsync(context: context, argPos: pos, services: null);
        }

        private Task OnClientLogReceived(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());  //로그 출력
            return Task.CompletedTask;
        }
    }
}