using GUC.Network;

namespace GUC.Scripts.Sumpfkraut.Networking
{
    public interface IScriptMessageHandler
    {
        void HandleMessage(ScriptClient sender, PacketReader stream);

        RP_Shared_Script.ScriptMessages SupportedMessage { get; }
    }
}
