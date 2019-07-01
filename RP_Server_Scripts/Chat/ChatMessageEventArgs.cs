using System;
using RP_Server_Scripts.ReusedClasses;

namespace RP_Server_Scripts.Chat
{
    public sealed class ChatMessageEventArgs:EventArgs
    {
        public ChatMessageEventArgs(Client.Client client, string message, ChatMode mode)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Mode = mode;
        }

        public string Message { get; }
        public Client.Client Client { get; }
        public ChatMode Mode { get; }
    }
}
