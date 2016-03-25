﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinApi;
using GUC.Log;

namespace GUC.Client.Hooks
{
    static class hWeather
    {
        const float DropsPerMillisecond = 1;
        const int MaxDropsPerFrame = 16;

        static HookInfos rainHook;
        public static void AddHooks()
        {
            Process.Hook(Program.GUCDll, typeof(hWeather).GetMethod("hook_ResetTime"), 0x005EB02A, 6, 0);
            rainHook = Process.Hook(Program.GUCDll, typeof(hWeather).GetMethod("hook_ParticleMaximum"), 0x005E1F0F, 6, 0);

            addr1 = rainHook.oldFuncInNewFunc.ToInt32();
            addr2 = 0x005E1F78;
            Process.Write(new byte[] { 0xB8, 0x00, 0x00, 0x00, 0x00, 0x90 }, addr1);
            Process.Write(new byte[] { 0xB8, 0x00, 0x00, 0x00, 0x00, 0x90 }, addr2);
            addr1++;
            addr2++;

            Process.Write(new byte[] { 0xE9, 0x8D, 0x00, 0x00, 0x00 }, 0x005EAF54); // jmp over rain weight calculation
        }

        static int addr1;
        static int addr2;

        public static Int32 hook_ResetTime(String message)
        {
            try
            {
                lastTime = 0;
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
            return 0;
        }

        static long lastTime = 0;
        public static Int32 hook_ParticleMaximum(String message)
        {
            try
            {
                long now = DateTime.UtcNow.Ticks;

                int num;
                if (lastTime > 0)
                {
                    num = (int)(DropsPerMillisecond * (double)(now - lastTime) / TimeSpan.TicksPerMillisecond);
                    if (num > MaxDropsPerFrame)
                        num = MaxDropsPerFrame;
                }
                else
                {
                    num = MaxDropsPerFrame;
                }

                Process.Write(num, addr1);
                Process.Write(num, addr2);

                lastTime = now;
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
            return 0;
        }
    }
}
