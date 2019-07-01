using System;
using GUC.Network;
using RP_Shared_Script;

namespace GUC.Scripts.Sumpfkraut.Networking
{
    internal class LoginAcknowledgedMessageHandler : IScriptMessageHandler
    {
        public event GenericEventHandler<LoginAcknowledgedMessageHandler> LoginAcknowledgementReceived;

        public void HandleMessage(ScriptClient sender, PacketReader stream)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            LoginAcknowledgementReceived?.Invoke(this);
        }

        public ScriptMessages SupportedMessage => ScriptMessages.LoginAcknowledge;
    }
}
