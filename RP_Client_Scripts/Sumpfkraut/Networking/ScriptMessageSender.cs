using System;
using GUC.Network;

namespace GUC.Scripts.Sumpfkraut.Networking
{
    public sealed class ScriptMessageSender
    {
        public void SendScriptMessage(PacketWriter stream, NetPriority priority, NetReliability reliability)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            GameClient.SendScriptMessage(stream, priority, reliability);
        }
    }
}
