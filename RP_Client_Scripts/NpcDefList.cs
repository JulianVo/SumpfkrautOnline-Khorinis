using GUC.Scripts.Sumpfkraut.VobSystem.Definitions;

namespace GUC.Scripts
{
  public sealed  class NpcDefList
    {
        public bool TryGetByName(int id, out NPCDef npcDef)
        {
            return BaseVobDef.TryGetDef(id, out npcDef);
        }
    }
}
