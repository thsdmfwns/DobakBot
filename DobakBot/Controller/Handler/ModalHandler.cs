using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Controller.Handler
{
    internal class ModalHandler
    {
        public ModalHandler(DiscordSocketClient client)
        {
            client.ModalSubmitted += Client_ModalSubmitted;
        }

        private async Task Client_ModalSubmitted(SocketModal arg)
        {
            return;
        }
    }
}
