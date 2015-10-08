﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUC.Enumeration;
using GUC.Server.Network;
using RakNet;
using GUC.Network;
using GUC.Server.Network.Messages;

namespace GUC.Server.WorldObjects
{
    public class NPC : AbstractCtrlVob, ItemContainer
    {
        #region Instance
        protected NPCInstance instance;
        public NPCInstance Instance { get { return instance; } }
        #endregion

        #region Appearance

        protected string customName = "";
        /// <summary>Set this for a different name from the instance-name.</summary>
        public string CustomName
        {
            get { return customName; }
            set 
            {
                if (value == null || value == instance.Name)
                {
                    customName = "";
                }
                else
                {
                    customName = value;
                }
            }
        }

        /// <summary>This field will be only used with the "_Male"- or "_Female"-Instance.</summary>
        public HumBodyTex HumanBodyTex = HumBodyTex.G1Hero;
        /// <summary>This field will be only used with the "_Male"- or "_Female"-Instance.</summary>
        public HumHeadMesh HumanHeadMesh = HumHeadMesh.HUM_HEAD_PONY;
        /// <summary>This field will be only used with the "_Male"- or "_Female"-Instance.</summary>
        public HumHeadTex HumanHeadTex = HumHeadTex.Face_N_Player;
        /// <summary>This field will be only used with the "_Male"- or "_Female"-Instance.</summary>
        public HumVoice HumanVoice = HumVoice.Hero;

        /// <summary>Body height in percent. (0% ... 255%)</summary>
        public byte BodyHeight;
        /// <summary>Body width in percent. (0% ... 255%)</summary>
        public byte BodyWidth;
        /// <summary>Fatness in percent. (-32768% ... +32767%)</summary>
        public short BodyFatness;

        #endregion

        //Things only the playing Client should know
        #region Player stats

        public ushort AttrHealth;
        public ushort AttrHealthMax;

        public ushort AttrMana;
        public ushort AttrManaMax;
        public ushort AttrStamina;
        public ushort AttrStaminaMax;

        public ushort AttrStrength;
        public ushort AttrDexterity;

        public ushort AttrCapacity;

        #region Health

        public void AttrHealthUpdate()
        {
            BitStream stream = Program.server.SetupStream(NetworkID.NPCHitMessage);
            stream.mWrite(ID);
            stream.mWrite(AttrHealthMax);
            stream.mWrite(AttrHealth);

            foreach (Client cl in cell.SurroundingClients())
                Program.server.ServerInterface.Send(stream, PacketPriority.HIGH_PRIORITY, PacketReliability.RELIABLE_ORDERED, 'W', cl.guid, false);
        }

        public void AttrManaStaminaUpdate()
        {
            if (isPlayer)
            {
                BitStream stream = Program.server.SetupStream(NetworkID.PlayerAttributeMSMessage);
                stream.mWrite(AttrManaMax);
                stream.mWrite(AttrMana);
                stream.mWrite(AttrStaminaMax);
                stream.mWrite(AttrStamina);
                Program.server.ServerInterface.Send(stream, PacketPriority.MEDIUM_PRIORITY, PacketReliability.RELIABLE_ORDERED, 'W', client.guid, false);
            }
        }

        public void AttrUpdate()
        {
            BitStream stream = Program.server.SetupStream(NetworkID.NPCHitMessage);
            stream.mWrite(ID);
            stream.mWrite(AttrHealthMax);
            stream.mWrite(AttrHealth);

            foreach (Client cl in cell.SurroundingClients(client))
                Program.server.ServerInterface.Send(stream, PacketPriority.LOW_PRIORITY, PacketReliability.RELIABLE_ORDERED, 'W', cl.guid, false);

            if (isPlayer)
            {
                // Update all the stats!
                stream = Program.server.SetupStream(NetworkID.PlayerAttributeMessage);
                stream.mWrite(AttrHealthMax);
                stream.mWrite(AttrHealth);
                stream.mWrite(AttrManaMax);
                stream.mWrite(AttrMana);
                stream.mWrite(AttrStaminaMax);
                stream.mWrite(AttrStamina);
                stream.mWrite(AttrCapacity);
                Program.server.ServerInterface.Send(stream, PacketPriority.LOW_PRIORITY, PacketReliability.RELIABLE_ORDERED, 'W', client.guid, false);
            }
        }

        #endregion

        #endregion

        #region States

        protected NPCState state = NPCState.Stand;
        public NPCState State { get { return state; } internal set { state = value; } }

        protected NPCWeaponState wpState = NPCWeaponState.None;
        public NPCWeaponState WeaponState { get { return wpState; } internal set { wpState = value; } }

        #endregion

        #region Constructors

        public NPC(NPCInstance inst)
        {
            instance = inst;
            BodyHeight = instance.BodyHeight;
            BodyWidth = instance.BodyWidth;
            BodyFatness = instance.Fatness;

            AttrHealthMax = 100;
            AttrHealth = 100;

            AttrManaMax = 10;
            AttrMana = 10;
            AttrStaminaMax = 100;
            AttrStamina = 100;

            AttrStrength = 10;
            AttrDexterity = 10;

            AttrCapacity = 1000;
        }

        #endregion

        #region Networking

        internal Client client;
        public bool isPlayer { get { return client != null; } }

        internal override void WriteSpawn(IEnumerable<Client> list)
        {
            BitStream stream = Program.server.SetupStream(NetworkID.WorldNPCSpawnMessage);
            stream.mWrite(ID);
            stream.mWrite(pos);
            stream.mWrite(dir);
            stream.mWrite(instance.ID);
            if (instance.ID <= 1)
            {
                stream.mWrite((byte)HumanBodyTex);
                stream.mWrite((byte)HumanHeadMesh);
                stream.mWrite((byte)HumanHeadTex);
                stream.mWrite((byte)HumanVoice);
            }
            stream.mWrite(BodyHeight);
            stream.mWrite(BodyWidth);
            stream.mWrite(BodyFatness);

            stream.mWrite(CustomName);

            stream.mWrite(AttrHealthMax);
            stream.mWrite(AttrHealth);

            stream.mWrite(equippedMeleeWeapon != null);
            if (equippedMeleeWeapon != null)
            {
                stream.mWrite(equippedMeleeWeapon.ID);
            }

            stream.mWrite(equippedRangedWeapon != null);
            if (equippedRangedWeapon != null)
            {
                stream.mWrite(equippedRangedWeapon.ID);
            }

            stream.mWrite(equippedArmor != null);
            if (equippedArmor != null)
            {
                stream.mWrite(equippedArmor.ID);
            }

            stream.mWrite((byte)state);
            stream.mWrite((byte)wpState);

            foreach (Client cl in list)
                Program.server.ServerInterface.Send(stream, PacketPriority.HIGH_PRIORITY, PacketReliability.RELIABLE_ORDERED, 'W', cl.guid, false);
        }

        #endregion

        #region Equipment

        protected ItemInstance equippedMeleeWeapon;
        public ItemInstance EquippedMeleeWeapon { get { return equippedMeleeWeapon; } }

        protected ItemInstance equippedRangedWeapon;
        public ItemInstance EquippedRangedWeapon { get { return equippedRangedWeapon; } }

        protected ItemInstance equippedArmor;
        public ItemInstance EquippedArmor { get { return equippedArmor; } }

        public void Equip(ItemInstance inst)
        {
            if (inst.Type >= ItemType.Sword_1H && inst.Type <= ItemType.Blunt_2H)
                equippedMeleeWeapon = inst;
            else if (inst.Type == ItemType.Bow || inst.Type == ItemType.XBow)
                equippedRangedWeapon = inst;
            else if (inst.Type == ItemType.Armor)
                equippedArmor = inst;
            else return;

            NPCMessage.WriteEquipMessage(this.cell.SurroundingClients(), this, inst);
        }

        public void Unequip(ItemInstance inst)
        {
            if (inst == equippedMeleeWeapon)
                equippedMeleeWeapon = null;
            else if (inst == equippedRangedWeapon)
                equippedRangedWeapon = null;
            else if (inst == equippedArmor)
                equippedArmor = null;
            else return;

            NPCMessage.WriteEquipMessage(this.cell.SurroundingClients(), this, inst);
        }

        #endregion

        #region Itemcontainer

        protected Dictionary<ItemInstance, int> inventory = new Dictionary<ItemInstance, int>();
        public Dictionary<ItemInstance, int> Inventory { get { return inventory; } }

        protected int weight = 0;
        public int Weight { get { return weight; } }

        public bool HasItem(ItemInstance instance)
        {
            return HasItem(instance, 1);
        }

        public bool HasItem(ItemInstance instance, int amount)
        {
            int has;
            inventory.TryGetValue(instance, out has);
            return has >= amount;
        }

        public bool AddItem(ItemInstance instance)
        {
            return AddItem(instance, 1);
        }

        public bool AddItem(ItemInstance instance, int amount)
        {
            int newWeight = weight + instance.Weight * amount;
            if (AttrCapacity >= newWeight)
            {
                weight = newWeight;
                if (inventory.ContainsKey(instance))
                {
                    inventory[instance] += amount;
                }
                else
                {
                    inventory.Add(instance, amount);
                }

                if (this.isPlayer)
                {
                    Network.Messages.InventoryMessage.WriteAddItem(client, instance, amount);
                }
                return true;
            }
            return false;
        }

        public void RemoveItem(ItemInstance instance)
        {
            RemoveItem(instance, 1);
        }

        public void RemoveItem(ItemInstance instance, int amount)
        {
            int current;
            if (inventory.TryGetValue(instance, out current))
            {
                if (current - amount <= 0)
                    inventory.Remove(instance);
                else
                    inventory[instance] -= amount;

                weight -= instance.Weight * amount;

                if (this.isPlayer)
                {
                    Network.Messages.InventoryMessage.WriteRemoveItem(client, instance, amount);
                }
            }
        }
        #endregion

        #region Events
        public delegate void OnHitHandler(NPC attacker, NPC target);
        public OnHitHandler OnHit;
        public static OnHitHandler sOnHit;

        internal void DoHit(NPC attacker)
        {
            if (sOnHit != null)
            {
                sOnHit(attacker, this);
            }
            if (OnHit != null)
            {
                OnHit(attacker, this);
            }
        }
        #endregion
    }
}
