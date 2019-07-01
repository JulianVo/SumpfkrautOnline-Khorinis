using GUC.Network;
using GUC.Types;
using RP_Server_Scripts.WorldSystem;

namespace RP_Server_Scripts.Network.ScriptMessages
{
    internal class SpectateScriptMessageHandler:IScriptMessageHandler
    {
        public void HandleMessage(Client.Client sender, PacketReader stream)
        {
            sender.BaseClient.SetToSpectate(WorldInst.List[0].BaseWorld, new Vec3f(-6489, -480, 3828), new Angles(0.1151917f, -2.104867f, 0f));
        }

        public RP_Shared_Script.ScriptMessages SupportedMessage => RP_Shared_Script.ScriptMessages.Spectate;
    }
}
