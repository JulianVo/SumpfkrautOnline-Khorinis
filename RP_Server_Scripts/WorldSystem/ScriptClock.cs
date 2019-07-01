using System;
using GUC.Network;
using GUC.Types;
using GUC.WorldObjects.WorldGlobals;

namespace RP_Server_Scripts.WorldSystem
{
    public class ScriptClock : WorldClock.IScriptWorldClock
    {
        public ScriptClock(WorldInst world)
        {
            World = world ?? throw new ArgumentNullException(nameof(world));
        }

        public WorldInst World { get; }

        public WorldClock BaseClock => World.BaseWorld.Clock;

        public WorldTime Time => BaseClock.Time;
        public float Rate => BaseClock.Rate;

        public long GetDurationInTicks(WorldTime time)
        {
            return (long) (time.GetTotalSeconds() * TimeSpan.TicksPerSecond / Rate);
        }

        public void SetTime(WorldTime time, float rate)
        {
            BaseClock.SetTime(time, rate);
        }

        public void Start()
        {
            BaseClock.Start();
        }

        public void Stop()
        {
            BaseClock.Stop();
        }

        public void OnReadProperties(PacketReader stream)
        {
        }

        public void OnWriteProperties(PacketWriter stream)
        {
        }
    }
}