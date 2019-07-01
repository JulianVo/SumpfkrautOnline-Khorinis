using GUC.Network;
using GUC.WorldObjects;
using RP_Server_Scripts.Visuals;
using RP_Server_Scripts.VobSystem.Definitions;
using RP_Server_Scripts.VobSystem.Definitions.Item;
using RP_Server_Scripts.VobSystem.Instances.ItemContainers;

namespace RP_Server_Scripts.VobSystem.Instances
{
    public class ItemInst : NamedVobInst, Item.IScriptItem
    {

        public ItemInst(ItemDef def) : base(def)
        {
            Definition = def;
        }

        protected override BaseVob CreateVob()
        {
            return new Item(new ModelInst(this), this);
        }


        public override VobType VobType => VobType.Item;


        public new Item BaseInst => (Item) base.BaseInst;

        public new ItemDef Definition
        {
            get => (ItemDef) base.Definition;
            set => base.Definition = value;
        }

        public int Amount => BaseInst.Amount;

        public bool IsEquipped => BaseInst.IsEquipped;

        public ItemMaterials Material => Definition.Material;

        public ItemTypes ItemType => Definition.ItemType;

        public bool IsAmmo => Definition.IsAmmo;
        public bool IsWeapon => Definition.IsWeapon;
        public bool IsWepRanged => Definition.IsWepRanged;
        public bool IsWepMelee => Definition.IsWepMelee;


        public IContainer Container =>
            ((ScriptInventory) BaseInst.Container.Inventory.ScriptObject).Owner;

        public delegate void SetAmountHandler(ItemInst item);

        public event SetAmountHandler OnSetAmount;

        public void SetAmount(int amount)
        {
            BaseInst.SetAmount(amount);
            OnSetAmount?.Invoke(this);
        }

        // Nur das Wichtigste was von aussen als Ausrüstung zu sehen ist!
        void Item.IScriptItem.ReadEquipProperties(PacketReader stream)
        {
        }

        void Item.IScriptItem.WriteEquipProperties(PacketWriter stream)
        {
        }

        // Alles schreiben was der Besitzer über dieses Item wissen muss
        void Item.IScriptItem.WriteInventoryProperties(PacketWriter stream)
        {
        }

        void Item.IScriptItem.ReadInventoryProperties(PacketReader stream)
        {
        }


        /// <summary>
        ///     Removes the item from the world or a container.
        /// </summary>
        public void Remove()
        {
            if (IsEquipped && Container is NpcInst npc)
            {
                npc.UnequipItem(this);
            }

            BaseInst.Remove();
        }

        public int Damage => Definition.Damage;
        public float Range => Definition.Range;
        public int Protection => Definition.Protection;

        /// <summary>
        ///     Creates a new item from the old item with the given amount and reduces the old item's amount.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public ItemInst Split(int amount)
        {
            if (amount <= 0 || Amount <= 0)
            {
                return null;
            }

            int newAmount = Amount - amount;

            if (newAmount > 0)
            {
                SetAmount(newAmount);
                // split item
                ItemInst newItem = new ItemInst(Definition);
                newItem.SetAmount(amount);
                return newItem;
            }

            Remove();
            return this;
        }
    }
}