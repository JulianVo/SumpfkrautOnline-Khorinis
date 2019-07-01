using GUC.WorldObjects.ItemContainers;
using RP_Server_Scripts.VobSystem.Instances.ItemContainers;

namespace RP_Server_Scripts
{
    public interface IContainer
    {
        ItemInventory BaseInventory { get; }
        ScriptInventory Inventory { get; }
    }
}
