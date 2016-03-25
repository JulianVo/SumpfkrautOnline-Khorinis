﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUC.Network;
using GUC.Scripting;
using GUC.WorldObjects;
using GUC.Enumeration;
using RakNet;

namespace GUC.Client.Network.Messages
{
    static class WorldMessage
    {
        #region WorldClock

        public static void ReadTimeMessage(PacketReader stream)
        {
            var clock = World.Current.Clock;
            clock.ReadProperties(stream);
            World.Current.ScriptObject.SetDayTime(clock.Day, clock.Hour, clock.Minute, clock.Rate);
        }

        public static void ReadTimeStartMessage(PacketReader stream)
        {
            if (stream.ReadBit())
            {
                World.Current.ScriptObject.StartDayClock();
            }
            else
            {
                World.Current.ScriptObject.StopDayClock();
            }
        }

        #endregion

        #region World Loading

        public static void ReadLoadWorldMessage(PacketReader stream)
        {
            var world = ScriptManager.Interface.CreateWorld();
            world.ID = 0;
            world.ReadStream(stream);
            bool startClock = stream.ReadBit();
            world.Create();
            World.current = world;
            world.ScriptObject.Load();
            if (startClock)
            {
                world.ScriptObject.StartDayClock();
            }

            SendConfirmation();
        }

        static void SendConfirmation()
        {
            PacketWriter stream = GameClient.SetupStream(NetworkIDs.LoadWorldMessage);
            GameClient.Send(stream, PacketPriority.IMMEDIATE_PRIORITY, PacketReliability.RELIABLE);
        }

        #endregion

        #region Spawns & Cells

        public static void ReadVobSpawnMessage(PacketReader stream)
        {
            BaseVob vob = ScriptManager.Interface.CreateVob((VobTypes)stream.ReadByte());
            vob.ReadStream(stream);
            vob.ScriptObject.Spawn(World.current);
        }

        public static void ReadVobDespawnMessage(PacketReader stream)
        {
            BaseVob vob;
            if (World.current.TryGetVob(stream.ReadUShort(), out vob))
            {
                vob.ScriptObject.Despawn();
            }
        }

        public static void ReadCellMessage(PacketReader stream)
        {
            for (int t = 0; t < (int)VobTypes.Maximum; t++)
            {
                int vobCount = stream.ReadUShort();
                for (int i = 0; i < vobCount; i++)
                {
                    BaseVob vob = ScriptManager.Interface.CreateVob((VobTypes)t);
                    vob.ReadStream(stream);
                    vob.ScriptObject.Spawn(World.current);
                }
            }
            int delCount = stream.ReadUShort();
            for (int i = 0; i < delCount; i++)
            {
                BaseVob vob;
                if (World.Current.TryGetVob(stream.ReadUShort(), out vob))
                {
                    vob.ScriptObject.Despawn();
                }
            }
        }

        #endregion
    }
}
