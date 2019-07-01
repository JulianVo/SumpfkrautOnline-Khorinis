using System.Collections.Generic;

namespace RP_Server_Scripts.VobSystem.Definitions
{
    public interface IItemDefList
    {
        ItemDef GetByCode(string codeName);
        IEnumerable<ItemDef> AllDefinitions { get; }
    }
}