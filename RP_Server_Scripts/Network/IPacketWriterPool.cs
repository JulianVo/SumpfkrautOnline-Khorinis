using GUC.Network;

namespace RP_Server_Scripts.Network
{
    public interface IPacketWriterPool
    {
        ScriptPacketWriter GetScriptMessageStream();

        ScriptPacketWriter GetScriptMessageStream(RP_Shared_Script.ScriptMessages id);

    }
}
