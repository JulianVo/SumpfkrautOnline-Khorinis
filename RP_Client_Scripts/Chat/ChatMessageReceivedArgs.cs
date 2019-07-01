using System;

namespace GUC.Scripts.Arena
{
    public sealed class ChatMessageReceivedArgs
    {
        public ChatMessageReceivedArgs(ChatMode mode, string message)
        {
            Mode = mode;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public string Message { get; }
        public ChatMode Mode { get; }
    }
}
