using System.Threading.Tasks;
using GUC.Network;

namespace RP_Server_Scripts.Network.ScriptMessages
{
    internal class LeaveGameMessageHandler : IScriptMessageHandler
    {
        public void HandleMessage(Client.Client sender, PacketReader stream)
        {
            var character = sender.Character;
            sender?.RemoveControl();
            character?.Despawn();
        }

        public RP_Shared_Script.ScriptMessages SupportedMessage => RP_Shared_Script.ScriptMessages.LeaveGame;
    }
}
