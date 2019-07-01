using System;
using GUC;
using GUC.Network;
using GUC.Scripting;
using GUC.WorldObjects.WorldGlobals;

namespace RP_Server_Scripts.WorldSystem
{
    public class ScriptBarrierCtrl : BarrierController.IScriptBarrierController
    {
        public ScriptBarrierCtrl(WorldInst world)
        {
            this.World = world ?? throw new ArgumentNullException(nameof(world));
        }

        public WorldInst World { get; }

        public BarrierController BaseBarrier => this.World.BaseWorld.BarrierCtrl;

        public void SetNextWeight(long time, float weight)
        {
         
        }

        public void OnReadProperties(PacketReader stream)
        {
        }

        public void OnReadSetWeight(PacketReader stream)
        {
        }

        public void OnWriteProperties(PacketWriter stream)
        {
        }

        public void OnWriteSetWeight(PacketWriter stream)
        {
        }

        const long InvisibleTimeMin = 30 * TimeSpan.TicksPerSecond;
        const long InvisibleTimeMax = 3 * TimeSpan.TicksPerMinute;

        const long VisibleTimeMin = 1 * TimeSpan.TicksPerSecond;
        const long VisibleTimeMax = 20 * TimeSpan.TicksPerSecond;

        const int TransitionSecondsMin = 2;
        const int TransitionSecondsMax = 10;

        readonly GUCTimer _Timer = new GUCTimer();

        void ShowBarrier()
        {
            long transition = Randomizer.GetInt(TransitionSecondsMin, TransitionSecondsMax) * TimeSpan.TicksPerSecond;
            SetNextWeight(GameTime.Ticks + transition, 1.0f); // barrier visible

            _Timer.SetInterval(VisibleTimeMin + (long)(Randomizer.GetDouble() * VisibleTimeMax));
            _Timer.SetCallback(HideBarrier);
        }

        void HideBarrier()
        {
            long transition = Randomizer.GetInt(TransitionSecondsMin, TransitionSecondsMax) * TimeSpan.TicksPerSecond;
            SetNextWeight(GameTime.Ticks + transition, 0.0f); // barrier invisible

            _Timer.SetInterval(InvisibleTimeMin + (long)(Randomizer.GetDouble() * InvisibleTimeMax));
            _Timer.SetCallback(ShowBarrier);
        }

        public void StartTimer()
        {
            HideBarrier();
            _Timer.Start();
        }

        public void StopTimer()
        {
            _Timer.Stop();
            HideBarrier();
        }
    }
}
