using GUC.Network;

namespace RP_Server_Scripts.Network
{
    public interface IPacketWriterFactory
    {
        PacketWriter GetScriptMessageStream();

        PacketWriter GetScriptMessageStream(RP_Shared_Script.ScriptMessages id);

    }
}
