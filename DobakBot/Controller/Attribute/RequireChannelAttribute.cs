using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Controller.Attribute
{
    class RequireChannelAttribute : PreconditionAttribute
    {
        private readonly string _name;

        public RequireChannelAttribute(string name) => _name = name;

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.Channel is SocketTextChannel textChannel)
            {
                var name = textChannel.Name.Split('-', StringSplitOptions.RemoveEmptyEntries)[0];
                if (name == _name)
                    return Task.FromResult(PreconditionResult.FromSuccess());
                else
                    return Task.FromResult(PreconditionResult.FromError($"이커맨드는 {_name}채널에서만 가능합니다."));
            }
            else
                return Task.FromResult(PreconditionResult.FromError("디스코드 채널에서만 사용가능합니다."));
        }
    }
}
