using System;
using System.Collections.Generic;
using GUC.GameObjects;
using GUC.Network;
using GUC.Utilities;
using GUC.WorldObjects;
using RP_Server_Scripts.ReusedClasses;
using RP_Server_Scripts.VobSystem.Instances;

namespace RP_Server_Scripts.WorldSystem
{
    public class WorldInst : ExtendedObject, World.IScriptWorld
    {
        public World BaseWorld { get; }

        private WorldDef _Definition;

        public WorldDef Definition
        {
            get => _Definition;
            set
            {
                if (IsCreated)
                {
                    throw new ArgumentNullException(
                        "Can't change the definition when the object is already added to the static collection!");
                }

                _Definition = value;
            }
        }

        public bool IsCreated => BaseWorld.IsCreated;
        public ScriptClock Clock => (ScriptClock) BaseWorld.Clock.ScriptObject;
        public ScriptWeatherCtrl Weather => (ScriptWeatherCtrl) BaseWorld.WeatherCtrl.ScriptObject;
        public ScriptBarrierCtrl Barrier => (ScriptBarrierCtrl) BaseWorld.BarrierCtrl.ScriptObject;

        public WorldInst(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(path));
            }

            Path = path;
            BaseWorld = new World(new ScriptClock(this),
                new ScriptWeatherCtrl(this), new ScriptBarrierCtrl(this), this);
        }

        public WorldInst(WorldDef def, string path) : this(path)
        {
            _Definition = def;
        }

        public string Path { get;}

        void GameObject.IScriptGameObject.OnWriteProperties(PacketWriter stream)
        {
            // write definition id
            stream.Write(Path);
        }

        void GameObject.IScriptGameObject.OnReadProperties(PacketReader stream)
        {
        }

        public void Create()
        {
            BaseWorld.Create();
            Weather.StartRainTimer();
            Barrier.StartTimer();
        }

        public void Delete()
        {
            BaseWorld.Delete();
            Weather.StopRainTimer();
            Barrier.StopTimer();
        }

        /// <summary> Gets a vob by ID from this world. </summary>
        public bool TryGetVob(int id, out BaseVobInst vob)
        {
            if (BaseWorld.TryGetVob(id, out var baseVob))
            {
                vob = (BaseVobInst) baseVob.ScriptObject;
                return true;
            }

            vob = null;
            return false;
        }

        /// <summary> Gets a vob of the specific type by ID from this world. </summary>
        public bool TryGetVob<T>(int id, out T vob) where T : BaseVobInst
        {
            if (TryGetVob(id, out var baseVob) && baseVob is T variable)
            {
                vob = variable;
                return true;
            }

            vob = null;
            return false;
        }

        public static readonly List<WorldInst> List = new List<WorldInst>();

        /// <summary> Despawn list for dead npcs </summary>
        public readonly DespawnList<NpcInst> DespawnList_NPC = new DespawnList<NpcInst>(50);

        /// <summary> Despawn list for projectile items (arrows) </summary>
        public readonly DespawnList<ItemInst> DespawnList_PItems = new DespawnList<ItemInst>(3);

     
    }
}