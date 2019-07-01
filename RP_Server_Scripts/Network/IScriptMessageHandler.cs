using GUC.Network;

namespace RP_Server_Scripts.Network
{
    public interface IScriptMessageHandler
    {
        void HandleMessage(Client.Client sender, PacketReader stream);

        RP_Shared_Script.ScriptMessages SupportedMessage { get; }
    }
}
