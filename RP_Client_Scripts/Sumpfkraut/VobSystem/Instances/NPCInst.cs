using GUC.Scripts.Sumpfkraut.VobSystem.Instances.ItemContainers;
using GUC.WorldObjects;
using GUC.Network;
using GUC.Scripts.Sumpfkraut.Visuals;
using GUC.Scripts.Sumpfkraut.VobSystem.Definitions;
using GUC.Types;
using GUC.WorldObjects.ItemContainers;
using GUC.Scripts.Sumpfkraut.WorldSystem;
using RP_Shared_Script;

namespace GUC.Scripts.Sumpfkraut.VobSystem.Instances
{
    public partial class NPCInst : VobInst, NPC.IScriptNPC, ScriptInventory.IContainer
    {
        partial void pConstruct();
        public NPCInst()
        {
            pConstruct();
        }

        protected override BaseVob CreateVob()
        {
            return new NPC(new ScriptInventory(this), new ModelInst(this), this);
        }



        public override VobType VobType { get { return VobType.NPC; } }


        new public NPC BaseInst { get { return (NPC)base.BaseInst; } }
        public ItemInventory BaseInventory { get { return BaseInst.Inventory; } }
        public ScriptInventory Inventory { get { return (ScriptInventory)BaseInventory.ScriptObject; } }

        public new NPCDef Definition { get { return (NPCDef)base.Definition; } set { base.Definition = value; } }

        public NPCMovement Movement { get { return this.BaseInst.Movement; } }
        public VobEnvironment Environment { get { return this.BaseInst.Environment; } }

        public bool IsDead { get { return this.BaseInst.IsDead; } }
        public bool IsInFightMode { get { return this.BaseInst.IsInFightMode; } }

        public bool IsWading
        {
            get
            {
                float waterLevel = Environment.WaterLevel;
                return waterLevel > 0 && waterLevel < 0.4f;
            }
        }

        public bool IsSwimming
        {
            get
            {
                float waterLevel = Environment.WaterLevel;
                return waterLevel > 0 && waterLevel >= 0.4f;
            }
        }

        public int HP { get { return this.BaseInst.HP; } }
        public int HPMax { get { return this.BaseInst.HPMax; } }

        public bool UseCustoms = false;
        public HumBodyTexs CustomBodyTex;
        public HumHeadMeshs CustomHeadMesh;
        public HumHeadTexs CustomHeadTex;
        public HumVoices CustomVoice;
        public float CustomFatness = 0;
        public Vec3f CustomScale = new Vec3f(1, 1, 1);
        public string CustomName;


        public void OnWriteTakeControl(PacketWriter stream)
        {
            // write everything the player needs to know about this npc
            // i.e. abilities, level, guild etc
        }

        public void OnReadTakeControl(PacketReader stream)
        {
            // read everything the player needs to know about this npc
            // i.e. abilities, level, guild etc
        }

        public ItemInst LastUsedWeapon;

        public ItemInst GetEquipmentBySlot(NPCSlots slotNum)
        {
            return this.BaseInst.TryGetEquippedItem((int)slotNum, out Item item) ? (ItemInst)item.ScriptObject : null;
        }

        public ItemInst GetArmor() { return GetEquipmentBySlot(NPCSlots.Armor); }
        public ItemInst GetAmmo() { return GetEquipmentBySlot(NPCSlots.Ammo); }
        public ItemInst GetRightHand() { return GetEquipmentBySlot(NPCSlots.RightHand); }
        public ItemInst GetLeftHand() { return GetEquipmentBySlot(NPCSlots.LeftHand); }
        public bool HasItemInHands() { return GetRightHand() != null || GetLeftHand() != null; }

        public ItemInst GetDrawnWeapon()
        {
            ItemInst item;
            if (((item = GetRightHand()) != null && item.IsWeapon)
             || ((item = GetLeftHand()) != null && item.IsWeapon))
                return item;
            return null;
        }

        public void EquipItem(int slot, Item item)
        {
            this.EquipItem((NPCSlots)slot, (ItemInst)item.ScriptObject);
        }

        public delegate void OnEquipHandler(ItemInst item);
        public event OnEquipHandler OnEquip;

        partial void pBeforeEquip(NPCSlots slot, ItemInst item);
        partial void pAfterEquip(NPCSlots slot, ItemInst item);
        public void EquipItem(NPCSlots slot, ItemInst item)
        {
            if (item.BaseInst.Slot == (int)slot)
                return;

            pBeforeEquip(slot, item);
            this.BaseInst.EquipItem((int)slot, item.BaseInst);
            pAfterEquip(slot, item);

            OnEquip?.Invoke(item);
        }

        public void UnequipItem(Item item)
        {
            this.UnequipItem((ItemInst)item.ScriptObject);
        }
        public event OnEquipHandler OnUnequip;

        partial void pBeforeUnequip(ItemInst item);
        partial void pAfterUnequip(ItemInst item);
        public void UnequipItem(ItemInst item)
        {
            pBeforeUnequip(item);
            this.BaseInst.UnequipItem(item.BaseInst);
            pAfterUnequip(item);

            OnUnequip?.Invoke(item);
        }

        public void UnequipSlot(NPCSlots slot)
        {
            ItemInst item = GetEquipmentBySlot(slot);
            if (item != null)
                UnequipItem(item);
        }

        public delegate void OnDeathHandler(NPCInst npc);
        public static event OnDeathHandler sOnDeath;
        public event OnDeathHandler OnDeath;

        public void SetHealth(int hp)
        {
            this.SetHealth(hp, BaseInst.HPMax);
        }

        public int GetHealth()
        {
            return this.BaseInst.HP;
        }

        public event NPCEvent OnRevive;

        partial void pSetHealth(int hp, int hpmax);
        public void SetHealth(int hp, int hpmax)
        {
            if (this.HP <= 0 && hp > 0)
                OnRevive?.Invoke(this);

            this.BaseInst.SetHealth(hp, hpmax);
            pSetHealth(hp, hpmax);

            if (hp <= 0)
            {
                this.uncon = Unconsciousness.None;
                OnDeath?.Invoke(this);
                sOnDeath?.Invoke(this);
            }
        }

        public override void OnReadProperties(PacketReader stream)
        {
            base.OnReadProperties(stream);
            UseCustoms = stream.ReadBit();
            if (UseCustoms)
            {
                CustomBodyTex = (HumBodyTexs)stream.ReadByte();
                CustomHeadMesh = (HumHeadMeshs)stream.ReadByte();
                CustomHeadTex = (HumHeadTexs)stream.ReadByte();
                CustomVoice = (HumVoices)stream.ReadByte();
                CustomFatness = stream.ReadFloat();
                CustomScale = stream.ReadVec3f();
                CustomName = stream.ReadString();
            }

            this.uncon = (Unconsciousness)stream.ReadByte();
            this.TeamID = stream.ReadSByte();
        }

        // ARENA
        public int TeamID = -1;

        public override void OnWriteProperties(PacketWriter stream)
        {
            base.OnWriteProperties(stream);
            if (UseCustoms)
            {
                stream.Write(true);
                stream.Write((byte)CustomBodyTex);
                stream.Write((byte)CustomHeadMesh);
                stream.Write((byte)CustomHeadTex);
                stream.Write((byte)CustomVoice);
                stream.Write(CustomFatness);
                stream.Write(CustomScale);
                stream.Write(CustomName == null ? "" : CustomName);
            }
            else
            {
                stream.Write(false);
            }

            stream.Write((byte)this.uncon);
            stream.Write((sbyte)TeamID);
        }

        partial void pSetFightMode(bool fightMode);
        public void SetFightMode(bool fightMode)
        {
            this.BaseInst.SetFightMode(fightMode);
            pSetFightMode(fightMode);
        }

        public delegate void NPCEvent(NPCInst npc);
        public event NPCEvent OnDespawn;
        public event NPCEvent OnSpawn;

        partial void pDespawn();
        public override void Despawn()
        {
            pDespawn();
            base.Despawn();
            OnDespawn?.Invoke(this);
        }

        partial void pBeforeSpawn();
        partial void pAfterSpawn();
        public override void Spawn(WorldInst world, Vec3f pos, Angles ang)
        {
            pBeforeSpawn();
            base.Spawn(world, pos, ang);
            pAfterSpawn();

            OnSpawn?.Invoke(this);
        }

        public NPCEvent OnUnconChange;

        Unconsciousness uncon = Unconsciousness.None;
        public bool IsUnconscious { get { return uncon != Unconsciousness.None; } }

        public Allegiance Guild;
    }
}
