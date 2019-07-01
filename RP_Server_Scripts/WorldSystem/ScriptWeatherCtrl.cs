using System;
using GUC;
using GUC.Network;
using GUC.Scripting;
using GUC.Types;
using GUC.WorldObjects.WorldGlobals;

namespace RP_Server_Scripts.WorldSystem
{
    public class ScriptWeatherCtrl : WeatherController.IScriptWeatherController
    {
        public ScriptWeatherCtrl(WorldInst world)
        {
            World = world ?? throw new ArgumentNullException(nameof(world));
            _RainTimer = new GUCTimer(2 * TimeSpan.TicksPerMinute, OnRainChange);
        }

        public WorldInst World { get; }
        public WeatherController BaseWeather => World.BaseWorld.WeatherCtrl;

        public void SetNextWeight(long time, float weight)
        {
            BaseWeather.SetNextWeight(time, weight);
        }

        public void SetWeatherType(WeatherTypes type)
        {
            BaseWeather.SetWeatherType(type);
        }

        public void OnReadProperties(PacketReader stream)
        {
        }

        public void OnWriteProperties(PacketWriter stream)
        {
        }

        public void OnWriteSetWeight(PacketWriter stream)
        {
        }

        public void OnReadSetWeight(PacketReader stream)
        {
        }

        public void OnWriteSetWeatherType(PacketWriter stream)
        {
        }

        public void OnReadSetWeatherType(PacketReader stream)
        {
        }

        private readonly GUCTimer _RainTimer;


        private void OnRainChange()
        {
            long transition = Randomizer.GetInt(60, 360) * TimeSpan.TicksPerSecond;
            if (Randomizer.GetInt(0, 5) == 0)
            {
                SetNextWeight(GameTime.Ticks + transition, (float) Randomizer.GetDouble()); // rain
            }
            else
            {
                SetNextWeight(GameTime.Ticks + transition, 0.0f); // sun
            }
        }

        public void StartRainTimer()
        {
            _RainTimer.Start();
        }

        public void StopRainTimer()
        {
            _RainTimer.Stop();
        }
    }
}