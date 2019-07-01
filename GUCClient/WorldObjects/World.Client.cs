using System;
using System.Collections.Generic;
using Gothic;
using Gothic.Music;
using Gothic.Objects;
using Gothic.Session;
using Gothic.Sound;
using GUC.Network;
using GUC.Scripting;
using GUC.Log;

namespace GUC.WorldObjects
{
    public partial class World
    {
        public static event EventHandler WorldUnloaded;

        #region Network Messages

        internal static class Messages
        {
            #region World Loading & Joining

            public static void ReadLoadWorld(PacketReader stream)
            {
                if (current != null)
                {
                    current.ForEachVob(v => v.Despawn());
                    current.Delete();
                }

                current = ScriptManager.Interface.CreateWorld();

                current.ID = 0;
                current.Create();

                current.ReadStream(stream);
                if (stream.ReadBit())
                {
                    current.Clock.ScriptObject.Start();
                }
                current.WeatherCtrl.ScriptObject.SetWeatherType(current.WeatherCtrl.WeatherType);
                current.WeatherCtrl.ScriptObject.SetNextWeight(current.WeatherCtrl.EndTime, current.WeatherCtrl.EndWeight);
                current.BarrierCtrl.ScriptObject.SetNextWeight(current.BarrierCtrl.EndTime, current.BarrierCtrl.EndWeight);

                Hooks.hGame.FirstRenderDone = false;
                current.ScriptObject.Load();

                var hero = oCNpc.GetPlayer();
                if (hero.Address != 0)
                {
                    hero.Disable();
                    GothicGlobals.Game.GetWorld().RemoveVob(hero);
                }

                PacketWriter confirmation = GameClient.SetupStream(ClientMessages.WorldLoadedMessage);
                GameClient.Send(confirmation, NetPriority.Immediate, NetReliability.Reliable);

                CGameManager.ApplySomeSettings();
            }

            public static void ReadJoinWorld(PacketReader stream)
            {
                for (int i = stream.ReadUShort(); i > 0; i--)
                {
                    ReadVobSpawn(stream);
                }
            }

            public static void ReadLeaveWorldMessage(PacketReader stream)
            {
                if (current != null)
                {
                    var npc = new List<oCNpc>();
                    World.Current.ForEachVob(v =>
                    {
                        if (v.gVob.VTBL == zCObject.gVobTypes.oCNpc)
                        {
                            npc.Add(new oCNpc(v.gVob.Address));
                        }
                        v.Despawn();
                    });

                    oCGame.GetGame().ClearGameState();
                    oCGame.GetGame().GetSpawnManager().ClearList();
                    new oCWorld(Current.gWorld.Address).DisposeVobs();
                    new oCWorld(Current.gWorld.Address).DisposeWorld();

                    foreach (var oCNpc in npc)
                    {
                        Logger.Log(Log.Logger.LOG_INFO, $"vobtype is ");
                        Logger.Log(Log.Logger.LOG_INFO, $"Vob ''{oCNpc.VTBL.ToString()} has refCtr of {oCNpc.refCtr}");
                    }

                    //Stop all sounds.
                    zCSndSys_MSS.StopAllSounds();
                    //Stop the currently playing music

                    zCMusicSys_DirectMusic.Stop();
                    //ToDo The call to CheckObjectConsistency does currently still fail. Find out why
                    //oCGame.GetGame().CheckObjectConsistency();

                    current.Delete();
                    current = null;

                    WorldUnloaded?.Invoke(null, new EventArgs());
                }

            }

            #endregion

            #region Spawns

            public static void ReadCellMessage(PacketReader stream)
            {
                // remove vobs
                for (int i = stream.ReadUShort(); i > 0; i--)
                {
                    ReadVobDespawnMessage(stream);
                }

                // add vobs
                for (int i = stream.ReadUShort(); i > 0; i--)
                {
                    ReadVobSpawn(stream);
                }
            }

            public static void ReadVobSpawn(PacketReader stream)
            {
                byte type = stream.ReadByte();
                BaseVob vob = ScriptManager.Interface.CreateVob(type);
                vob.ReadStream(stream);
                vob.ScriptObject.Spawn(current);
            }

            public static void ReadVobDespawnMessage(PacketReader stream)
            {
                int id = stream.ReadUShort();

                if (current.TryGetVob(id, out BaseVob vob))
                {
                    // despawn also removes guided id
                    vob.ScriptObject.Despawn();
                }
                else
                {
                    GameClient.Client.guidedIDs.Remove(id);
                }
            }

            #endregion
        }

        #endregion

        static World current;
        public static World Current { get { return current; } }

        #region ScriptObject

        public partial interface IScriptWorld : IScriptGameObject
        {
            void Load();
        }

        #endregion

        #region Properties

        /// <summary> The correlating gothic-object of this world. </summary>
        public zCWorld gWorld { get { return GothicGlobals.Game.GetWorld(); } }

        #endregion

        #region Gothic-Object Address Dictionary

        // Dictionary with all addresses of gothic-objects in this world.
        Dictionary<int, BaseVob> vobAddr = new Dictionary<int, BaseVob>();

        public bool TryGetVobByAddress(int address, out BaseVob vob)
        {
            return vobAddr.TryGetValue(address, out vob);
        }

        public bool TryGetVobByAddress<T>(int address, out T vob) where T : BaseVob
        {
            BaseVob v;
            if (vobAddr.TryGetValue(address, out v))
            {
                if (v is T)
                {
                    vob = (T)v;
                    return true;
                }
            }
            vob = null;
            return false;
        }

        #endregion

        #region Add & Remove

        partial void pAfterAddVob(BaseVob vob)
        {
            // add the vob to the gothic world
            gWorld.AddVob(vob.gVob);

            // add the gothic-object's address to the dictionary
            vobAddr.Add(vob.gVob.Address, vob);
        }

        partial void pBeforeRemoveVob(BaseVob vob)
        {
            var gVob = vob.gVob;

            // update position & direction one last time
            //vob.UpdateOrientation();
            //vob.UpdateEnvironment();

            // remove gothic-object from the gothic-world
            gWorld.RemoveVob(gVob);

            // remove the gothic-object's address from the dictionary
            vobAddr.Remove(gVob.Address);
        }

        #endregion
    }
}
