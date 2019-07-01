using System;
using RP_Server_Scripts.ReusedClasses;

namespace RP_Server_Scripts.Chat
{
    internal sealed class ChatCommandReceivedEventArgs : EventArgs
    {
        public ChatCommandReceivedEventArgs(Client.Client sender, string command, ChatMode mode)
        {
            Sender = sender ?? throw new ArgumentNullException(nameof(sender));
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Mode = mode;
        }

        public Client.Client Sender { get; }
        public string Command { get; }
        public ChatMode Mode { get; }
    }
}
