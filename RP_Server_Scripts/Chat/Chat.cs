using System;
using GUC;
using GUC.Network;
using RP_Server_Scripts.Client;
using RP_Server_Scripts.ReusedClasses;
using RP_Shared_Script;

namespace RP_Server_Scripts.Chat
{
    internal sealed class Chat : IChatMessageReceiver
    {
        private readonly ClientList _ClientList;

        public Chat(ClientList clientList)
        {
            _ClientList = clientList ?? throw new ArgumentNullException(nameof(clientList));
        }


        public void SendMessageToAll(string message)
        {
            var stream = GameClient.GetScriptMessageStream();
            stream.Write((byte)ScriptMessages.ChatMessage);
            stream.Write(message);
            _ClientList.ForEach(c => c.BaseClient.SendScriptMessage(stream, NetPriority.Low, NetReliability.Reliable));
        }

        public event GenericEventHandler<Chat, ChatMessageEventArgs> ChatMessageReceived;
        public event GenericEventHandler<Chat, ChatCommandReceivedEventArgs> ChatCommandReceived;


        void IChatMessageReceiver.ReceiveChatMessage(Client.Client sender, string message, ChatMode mode)
        {
            if (message.StartsWith("/"))
            {
                ChatCommandReceived?.Invoke(this, new ChatCommandReceivedEventArgs(sender, message, mode));
                return;
            }

            ChatMessageReceived?.Invoke(this, new ChatMessageEventArgs(sender, message, mode));

            if (mode == ChatMode.All)
            {
                SendMessageToAll($"{sender.ControlledNpc.CustomName ?? throw new NullReferenceException("Character custom name must not be null")}: {message}");
            }
        }
    }
}
