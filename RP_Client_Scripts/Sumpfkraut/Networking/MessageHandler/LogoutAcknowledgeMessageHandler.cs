using GUC.Network;
using RP_Shared_Script;

namespace GUC.Scripts.Sumpfkraut.Networking
{
    internal sealed class LogoutAcknowledgeMessageHandler:IScriptMessageHandler
    {
        public event GenericEventHandler<LogoutAcknowledgeMessageHandler> LogoutAcknowledgmentReceived;


        public void HandleMessage(ScriptClient sender, PacketReader stream)
        {
            LogoutAcknowledgmentReceived?.Invoke(this);
        }

        public ScriptMessages SupportedMessage => ScriptMessages.LogoutAcknowledged;
    }
}
