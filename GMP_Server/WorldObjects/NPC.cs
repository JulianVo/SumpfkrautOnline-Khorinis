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
        //Networking
        internal Client client;
        public bool isPlayer { get { return client != null; } }

        protected string name = "Spieler";
        public string Name { get { return name; } }

        protected NPCState state = NPCState.Stand;
        public NPCState State { get { return state; } internal set { state = value; } }

        protected NPCWeaponState wpState = NPCWeaponState.None;
        public NPCWeaponState WeaponState { get { return wpState; } internal set { wpState = value; } }

        //Things only the playing Client should know
        #region Player stats

        public ushort[] Attributes = new ushort[NPCAttributes.MAX_ATTRIBUTES];
        public ushort[] Talents = new ushort[NPCTalents.MAX_TALENTS];

        #endregion

        #region Visual
        protected string bodyMesh = "hum_body_Naked0";
        public string BodyMesh
        {
            get
            {
                return bodyMesh;
            }
            set
            {
                bodyMesh = value;
                //update Networking
            }
        }

        protected int bodyTex = 9;
        public int BodyTex
        {
            get
            {
                return bodyTex;
            }
            set
            {
                bodyTex = value;
                //update Networking
            }
        }

        protected string headMesh = "Hum_Head_Pony";
        public string HeadMesh
        {
            get
            {
                return headMesh;
            }
            set
            {
                headMesh = value;
                //update Networking
            }
        }

        protected int headTex = 18;
        public int HeadTex
        {
            get
            {
                return headTex;
            }
            set
            {
                headTex = value;
                //update Networking
            }
        }

        protected float fatness = 0;
        public float Fatness
        {
            get
            {
                return fatness;
            }
            set
            {
                fatness = value;
                //update Networking
            }
        }

        protected float bodyHeight = 1.0f;
        public float BodyHeight
        {
            get
            {
                return bodyHeight;
            }
            set
            {
                bodyHeight = value;
                //update Networking
            }
        }
        
        protected float bodyWidth = 1.0f; //x & z together
        public float BodyWidth
        {
            get
            {
                return bodyWidth;
            }
            set
            {
                bodyWidth = value;
                //update Networking
            }
        }
        #endregion

        #region Animation
        public List<String> Overlays = new List<string>();
        public short Animation = short.MaxValue;
        #endregion

        public NPC() : base()
        {
            visual = "HUMANS.MDS";

            Attributes[NPCAttributes.Health_Max] = 1;
            Attributes[NPCAttributes.Health] = 1;
            Attributes[NPCAttributes.Capacity] = 1000;
        }

        internal override void WriteSpawn(IEnumerable<Client> list)
        {
            BitStream stream = Program.server.SetupStream(NetworkID.WorldNPCSpawnMessage);
            stream.mWrite(ID);
            stream.mWrite(pos);
            stream.mWrite(dir);
            stream.mWrite(name);

            foreach (Client client in list)
                Program.server.ServerInterface.Send(stream, PacketPriority.HIGH_PRIORITY, PacketReliability.RELIABLE_ORDERED, (char)0, client.guid, false);
        }

        #region Equipment

        protected ItemInstance equippedMeleeWeapon;
        public ItemInstance EquippedMeleeWeapon { get { return equippedMeleeWeapon; } }

        protected ItemInstance equippedRangedWeapon;
        public ItemInstance EquippedRangedWeapon { get { return equippedRangedWeapon; } }

        protected ItemInstance equippedArmor;
        public ItemInstance EquippedArmor { get { return equippedArmor; } }

        public void Equip(ItemInstance inst)
        {
            if (inst.MainFlags == MainFlags.ITEM_KAT_NF)
            {
                equippedMeleeWeapon = inst;
            }
            else if (inst.MainFlags == MainFlags.ITEM_KAT_FF)
            {
                equippedRangedWeapon = inst;
            }
            else if (inst.MainFlags == MainFlags.ITEM_KAT_ARMOR)
            {
                equippedArmor = inst;
            }
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
            if (Attributes[NPCAttributes.Capacity] >= newWeight)
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
    }
}