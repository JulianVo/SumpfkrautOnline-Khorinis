using System;
using GUC.Network;
using GUC.Scripts.Sumpfkraut.Networking;
using RP_Shared_Script;

namespace GUC.Scripts.Arena
{
    public class Chat
    {
        private readonly IPacketWriterFactory _WriterFactory;
        private readonly ScriptMessageSender _MessageSender;

        public event GenericEventHandler<Chat, ChatMessageReceivedArgs> ChatMessageReceived;

        public Chat(IPacketWriterFactory writerFactory, ScriptMessageSender messageSender)
        {
            _WriterFactory = writerFactory ?? throw new ArgumentNullException(nameof(writerFactory));
            _MessageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        }

        public void SendMessage(string message)
        {
            var stream = _WriterFactory.Create();
            stream.Write((byte)ScriptMessages.ChatMessage);
            stream.Write(message);
            _MessageSender.SendScriptMessage(stream, NetPriority.Low, NetReliability.Reliable);
        }

        public void SendTeamMessage(string message)
        {
            var stream = _WriterFactory.Create();
            stream.Write((byte)ScriptMessages.ChatTeamMessage);
            stream.Write(message);
            _MessageSender.SendScriptMessage(stream, NetPriority.Low, NetReliability.Reliable);
        }

        public void ReadMessage(PacketReader stream)
        {
            ChatMessageReceived?.Invoke(this, new ChatMessageReceivedArgs(ChatMode.All, stream.ReadString()));
        }

        public void ReadTeamMessage(PacketReader stream)
        {
            ChatMessageReceived?.Invoke(this, new ChatMessageReceivedArgs(ChatMode.Team, stream.ReadString()));
        }
    }
}
