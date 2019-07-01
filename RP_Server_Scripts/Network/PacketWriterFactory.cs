using GUC.Network;

namespace RP_Server_Scripts.Network
{
    internal class PacketWriterFactory : IPacketWriterFactory
    {
        public PacketWriter GetScriptMessageStream()
        {
            return GameClient.GetScriptMessageStream();
        }

        public PacketWriter GetScriptMessageStream(RP_Shared_Script.ScriptMessages id)
        {
            var s = GameClient.GetScriptMessageStream();
            s.Write((byte)id);
            return s;
        }
    }
}