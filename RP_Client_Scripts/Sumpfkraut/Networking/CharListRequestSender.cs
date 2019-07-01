using System;
using RP_Shared_Script;

namespace GUC.Scripts.Sumpfkraut.Networking
{
    public sealed class CharListRequestSender
    {
        private readonly IPacketWriterFactory _PacketWriterFactory;
        private readonly ScriptMessageSender _Sender;

        public CharListRequestSender(IPacketWriterFactory packetWriterFactory, ScriptMessageSender sender)
        {
            _PacketWriterFactory = packetWriterFactory ?? throw new ArgumentNullException(nameof(packetWriterFactory));
            _Sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public void SendCharacterListRequest()
        {
            _Sender.SendScriptMessage(_PacketWriterFactory.Create(ScriptMessages.RequestCharacterList), NetPriority.High, NetReliability.Reliable);
        }
    }
}
