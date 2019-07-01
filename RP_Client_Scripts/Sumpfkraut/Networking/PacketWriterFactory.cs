using GUC.Network;
using RP_Shared_Script;

namespace GUC.Scripts.Sumpfkraut.Networking
{
    public class PacketWriterFactory : IPacketWriterFactory
    {
        public PacketWriter Create()
        {
            return GameClient.GetScriptMessageStream();
        }

        public PacketWriter Create(ScriptMessages messageId)
        {
            PacketWriter writer = Create();
            writer.Write((byte) messageId);
            return writer;
        }
    }
}