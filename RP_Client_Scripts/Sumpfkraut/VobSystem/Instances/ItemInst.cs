﻿using GUC.Network;
using GUC.Scripts.Sumpfkraut.VobSystem.Definitions;
using GUC.Scripts.Sumpfkraut.WorldSystem;
using GUC.Types;
using GUC.WorldObjects;


namespace GUC.Scripts.Sumpfkraut.VobSystem.Instances
{
    public partial class ItemInst : NamedVobInst, Item.IScriptItem
    {
        partial void pConstruct();
        public ItemInst()
        {
            pConstruct();
        }

        protected override BaseVob CreateVob()
        {
            return new Item(new Visuals.ModelInst(this), this);
        }


        public override VobType VobType { get { return VobType.Item; } }



        public new Item BaseInst { get { return (Item)base.BaseInst; } }
        public new ItemDef Definition { get { return (ItemDef)base.Definition; } set { base.Definition = value; } }

        public int Amount { get { return this.BaseInst.Amount; } }

        public bool IsEquipped { get { return this.BaseInst.IsEquipped; } }

        public ItemMaterials Material { get { return this.Definition.Material; } }

        public ItemTypes ItemType { get { return this.Definition.ItemType; } }

        public bool IsAmmo { get { return this.Definition.IsAmmo; } }
        public bool IsWeapon { get { return this.Definition.IsWeapon; } }
        public bool IsWepRanged { get { return this.Definition.IsWepRanged; } }
        public bool IsWepMelee { get { return this.Definition.IsWepMelee; } }


        public ItemContainers.ScriptInventory.IContainer Container
        {
            get
            {
                return ((ItemContainers.ScriptInventory)this.BaseInst.Container.Inventory.ScriptObject).Owner;
            }
        }

        public delegate void SetAmountHandler(ItemInst item);
        public event SetAmountHandler OnSetAmount;

        public void SetAmount(int amount)
        {
            this.BaseInst.SetAmount(amount);
            OnSetAmount?.Invoke(this);
        }

        // Nur das Wichtigste was von aussen als Ausrüstung zu sehen ist!
        public void ReadEquipProperties(PacketReader stream)
        {
        }

        public void WriteEquipProperties(PacketWriter stream)
        {
        }

        // Alles schreiben was der Besitzer über dieses Item wissen muss
        public void WriteInventoryProperties(PacketWriter stream)
        {
        }

        public void ReadInventoryProperties(PacketReader stream)
        {
        }

        partial void pSpawn();
        public override void Spawn(WorldInst world, Vec3f pos, Angles ang)
        {
            base.Spawn(world, pos, ang);
            pSpawn();
        }

        /// <summary>
        /// Removes the item from the world or a container.
        /// </summary>
        public void Remove()
        {
            if (this.IsEquipped && this.Container is NPCInst npc)
            {
                npc.UnequipItem(this);
            }

            this.BaseInst.Remove();
        }
    }
}
