using System;
using GUC.Network;
using RP_Server_Scripts.Chat;
using RP_Server_Scripts.ReusedClasses;

namespace RP_Server_Scripts.Network.ScriptMessages
{
    internal class ChatScriptMessageHandler : IScriptMessageHandler
    {
        private readonly IChatMessageReceiver _MessageReceiver;

        public ChatScriptMessageHandler(IChatMessageReceiver messageReceiver)
        {
            _MessageReceiver = messageReceiver ?? throw new ArgumentNullException(nameof(messageReceiver));
        }

        public void HandleMessage(Client.Client sender, PacketReader stream)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            string message = stream.ReadString();
            _MessageReceiver.ReceiveChatMessage(sender, message, ChatMode.All);
        }

        public RP_Shared_Script.ScriptMessages SupportedMessage => RP_Shared_Script.ScriptMessages.ChatMessage;
    }
}