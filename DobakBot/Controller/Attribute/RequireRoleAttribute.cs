using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Controller.Attribute
{
    public class RequireRoleAttribute : PreconditionAttribute
    {
        // Create a field to store the specified name
        private readonly string _name;

        // Create a constructor so the name can be specified
        public RequireRoleAttribute(string name) => _name = name;

        // Override the CheckPermissions method
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            // Check if this user is a Guild User, which is the only context where roles exist
            if (context.User is SocketGuildUser gUser)
            {
                // If this command was executed by a user with the appropriate role, return a success
                if (gUser.Roles.Any(r => r.Name == _name))
                    // Since no async work is done, the result has to be wrapped with `Task.FromResult` to avoid compiler errors
                    return Task.FromResult(PreconditionResult.FromSuccess());
                // Since it wasn't, fail
                else
                    return Task.FromResult(PreconditionResult.FromError($"이커맨드는 {_name}역활이 필요합니다."));
            }
            else
                return Task.FromResult(PreconditionResult.FromError("디스코드 채널에서만 사용가능합니다."));
        }
    }
}
