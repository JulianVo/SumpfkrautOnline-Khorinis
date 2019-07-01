using System;
using RP_Shared_Script;

namespace GUC.Scripts.Sumpfkraut.Networking
{
    internal sealed class LeaveGameSender
    {
        private readonly IPacketWriterFactory _PacketWriterFactory;
        private readonly ScriptMessageSender _Sender;

        public LeaveGameSender(IPacketWriterFactory packetWriterFactory,ScriptMessageSender sender)
        {
            _PacketWriterFactory = packetWriterFactory ?? throw new ArgumentNullException(nameof(packetWriterFactory));
            _Sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public void SendLeaveGameMessage()
        {
            _Sender.SendScriptMessage(_PacketWriterFactory.Create(ScriptMessages.LeaveGame),NetPriority.High,NetReliability.Reliable);
        }
    }
}
