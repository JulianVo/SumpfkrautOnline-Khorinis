using GUC.Network;
using GUC.WorldObjects.WorldGlobals;

namespace RP_Server_Scripts.WorldSystem
{
    internal class DoNothingBarrierControl : BarrierController.IScriptBarrierController
    {
        public void OnReadProperties(PacketReader stream)
        {
        }

        public void OnWriteProperties(PacketWriter stream)
        {
        }

        public void SetNextWeight(long time, float weight)
        {
        }

        public void OnWriteSetWeight(PacketWriter stream)
        {
        }

        public void OnReadSetWeight(PacketReader stream)
        {
        }
    }
}