﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RakNet;
using GUC.Network;
using GUC.WorldObjects;
using GUC.Enumeration;

namespace GUC.Server.Network.Messages
{
    static class InventoryMessage
    {
        //Add an item to the client's inventory
        public static void WriteAddItem(GameClient client, Item item)
        {
            PacketWriter stream = GameServer.SetupStream(NetworkIDs.InventoryAddMessage);
            stream.Write((byte)item.ID);
            item.WriteInventoryProperties(stream);
            client.Send(stream, PacketPriority.LOW_PRIORITY, PacketReliability.RELIABLE_ORDERED, 'I');
        }

        public static void WriteRemoveItem(GameClient client, Item item)
        {
            PacketWriter stream = GameServer.SetupStream(NetworkIDs.InventoryAddMessage);
            stream.Write((byte)item.ID);
            client.Send(stream, PacketPriority.LOW_PRIORITY, PacketReliability.RELIABLE_ORDERED, 'I');
        }

        public static void WriteAmountUpdate(GameClient client, Item item)
        {
            WriteAmountUpdate(client, item, item.Amount);
        }

        public static void WriteAmountUpdate(GameClient client, Item item, int amount)
        {
            PacketWriter stream = Network.GameServer.SetupStream(NetworkIDs.InventoryAmountMessage);
            stream.Write(item.ID);
            stream.Write((ushort)amount);
            client.Send(stream, PacketPriority.LOW_PRIORITY, PacketReliability.RELIABLE_ORDERED, 'I');
        }



        #region Equipment

        public static void WriteEquipMessage(NPC npc, Item item)
        {
            PacketWriter stream = GameServer.SetupStream(NetworkIDs.NPCEquipMessage);

            stream.Write((byte)item.ID);
            stream.Write((byte)item.Slot);

            npc.client.Send(stream, PacketPriority.LOW_PRIORITY, PacketReliability.RELIABLE_ORDERED, 'W');
        }

        public static void WriteUnequipMessage(NPC npc, int slot)
        {
            PacketWriter stream = GameServer.SetupStream(NetworkIDs.NPCUnequipMessage);
            
            stream.Write((byte)slot);

            npc.client.Send(stream, PacketPriority.LOW_PRIORITY, PacketReliability.RELIABLE_ORDERED, 'W');
        }

        public static void ReadEquipMessage(PacketReader stream, NPC character)
        {
            Item item;
            if (character.TryGetEquippedItem(stream.ReadByte(), out item))
            {
                character.ScriptObject.OnCmdEquipItem(stream.ReadByte(), item);
            }
        }

        public static void ReadUnequipMessage(PacketReader stream, NPC character)
        {
            Item item;
            if (character.TryGetEquippedItem(stream.ReadByte(), out item))
            {
                character.ScriptObject.OnCmdUnequipItem(item);
            }
        }
        #endregion
    }
}