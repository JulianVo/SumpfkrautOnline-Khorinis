﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using GUC.Network;
using Gothic;
using System.IO;
using Gothic.Types;
using GUC.Log;
using WinApi;
using GUC.Hooks;
using System.Reflection;
using System.Management;
using System.Runtime.InteropServices;

namespace GUC
{
    //zCMesh::Load(class zSTRING const &, int)
    //( zCMesh::MergeMesh(class zCMesh *, class zMAT4 const &) ? )
    //zCWorld::CompileWorld(zTBspTreeMode const &,float,int,int,zCArray<zCPolygon *> *) ???

    public static class Program
    {
        static string _GothicPath;
        static string _ProjectPath;
        static string _ServerIP;
        static ushort _ServerPort;
        static string _Password;
        static string _GothicRootPath;

        /// <summary> Gothic 2 folder. No backslash at the end. </summary>
        public static string GothicPath => _GothicPath;

        /// <summary> Gothic 2 folder. No backslash at the end. (Gothic2/System at start of program) </summary>
        public static string GothicRootPath => _GothicRootPath;

        /// <summary> project (ip) folder. No backslash at the end. </summary>
        public static string ProjectPath => _ProjectPath;

        public static string ServerIP => _ServerIP;
        public static ushort ServerPort => _ServerPort;
        public static string Password => _Password;

        public static string ProjectPathCombine(string path) { return Path.Combine(_ProjectPath, path); }
        public static string GothicPathCombine(string path) { return Path.Combine(_GothicPath, path); }
        public static string GothicRootPathCombine(string path) { return Path.Combine(_GothicRootPath, path); }

        static void SetRootPathHook(Hook hook, RegisterMemory rmem)
        {
            _GothicRootPath = Gothic.System.zFile.s_rootPathString.ToString().ToUpperInvariant();
            Logger.Log("Set root to: " + _GothicRootPath);
        }

        static void SetupProject()
        {
            _GothicPath = Environment.GetEnvironmentVariable("GUCGothicPath").ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(_GothicPath) || !Directory.Exists(_GothicPath))
                throw new Exception("Gothic folder environment variable is null or not found!");

            Process.AddHook(SetRootPathHook, 0x44235E, 7);
            Process.AddHook(SetRootPathHook, 0x44237A, 7);
            _GothicRootPath = Path.Combine(_GothicPath, "SYSTEM").ToUpperInvariant();

            _ProjectPath = Environment.GetEnvironmentVariable("GUCProjectPath").ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(_ProjectPath) || !Directory.Exists(_ProjectPath))
                throw new Exception("Project folder environment variable is null or not found!");

            _ServerIP = Environment.GetEnvironmentVariable("GUCServerIP");
            if (string.IsNullOrWhiteSpace(_ServerIP))
                throw new Exception("Server IP environment variable is null or white space!");

            if (!ushort.TryParse(Environment.GetEnvironmentVariable("GUCServerPort"), out _ServerPort))
                throw new Exception("Could not parse server port environment variable to ushort!");

            _Password = Environment.GetEnvironmentVariable("GUCServerPassword");
            if (string.IsNullOrWhiteSpace(_Password))
                _Password = null;

            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

            const string iniString = "SYSTEM\\GOTHIC.INI";
            string destIni = Path.Combine(ProjectPath, iniString);
            if (!File.Exists(destIni))
            {
                string srcIni = Path.Combine(GothicPath, iniString);
                if (File.Exists(srcIni))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destIni));
                    File.Copy(srcIni, destIni);
                }
            }
        }

        static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            string name = args.Name.Substring(0, args.Name.IndexOf(','));

            if (name.ToUpper() == "GUC.RESOURCES")
            {
                //load from resource
                var resxStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
                byte[] buffer = new byte[resxStream.Length];
                resxStream.Read(buffer, 0, (int)resxStream.Length);
                return Assembly.Load(buffer);
            }

       
            //Only load assemblies form this location if the actually exist. Otherwise we would block later AppDomain.CurrentDomain.AssemblyResolve  handler(e.g. Costura.Fody)
            if (File.Exists(Path.Combine(_ProjectPath, name + ".dll")))
            {
                return Assembly.LoadFrom(Path.Combine(_ProjectPath, name + ".dll"));
            }
            if (File.Exists(Path.Combine(_ProjectPath, "Scripts", name + ".dll")))
            {
                return Assembly.LoadFrom(Path.Combine(_ProjectPath, "Scripts", name + ".dll"));
            }

            return null;
        }

        static bool mained = false;
        public static int Main(string message)
        {
            try
            {
                if (mained) return 0;
                mained = true;
                
                SetupProject();

                SplashScreen.SetUpHooks();
                SplashScreen.Create();

                // add hooks
                //hFileSystem.AddHooks();
                Hooks.VDFS.hFileSystem.AddHooks();
                hParser.AddHooks();
                hGame.AddHooks();
                hPlayerVob.AddHooks();
                hWeather.AddHooks();
                VobRenderArgs.AddHooks();
                hNpc.AddHooks();
                hModel.AddHooks();
                hMob.AddHooks();

                #region Some more editing

                Process.Write(0x5D50CE, (byte)0xEB); // portal lighting, CollectLights_StatLights, handle sectors as outdoor
                                                     // portal fade ShouldActivatePortal

                //Process.Write(0x52F30F, 0xE9, 0xC0, 0x00, 0x00, 0x00); // RenderNodeOutdoor always render sectors
                //Process.Write(0x534954, 0xEB, 0x57); // ActivateSectorRec always render sectors

                //int wald = Process.AllocString("WALD", Encoding.Default);
                //Process.Write(0x535885 + 1, wald);
                //Process.Write(0x535A91 + 1, wald);


                Process.Write(0x00726831, 0xEB, 0x38); // don't fill load MobContainers in the Zen with Items

                Process.Write(0x42687F, 0xE9, 0xA3, 0x00, 0x00, 0x00); // skip intro videos

                Process.Write(0x00424EE2, 0xEB, 0x35); // don't init savegame manager
                Process.Write(0x5DEA4B, (byte)0xC3); // don't init output units

                Process.Write(0x006B5A44, 0xEB, 0x15); // don't start falling animation

                Process.Write(0x00735EB0, 0xC2, 0x08, 0x00); // don't drop unconscious
                Process.Nop(0x00736898, 7); // don't drop weapons on death

                // remove all gothic controls
                Process.Write(0x004D3DF6, 0xE9, 0x3E, 0x04, 0x00, 0x00);
                Process.Write(0x004D5700, 0xC3, 0xE8, 0x8B, 0xE6, 0xFF, 0xFF, 0xC3);
                Process.Nop(0x006C8A71, 5); // remove freeLook controls

                //Process.Write(0x006C873D, 0xD8, 0x1D, 0xB4, 0x04, 0x83, 0x00); // reduce time gothic waits after the loading screen from 2500ms to 1000ms
                //Process.Write(0x006C8720, 0xEB, 0x2C); // skip precache time completely


                Process.Write(0x008BACD0, 18000.0f); // spawnManager : insertrange
                Process.Write(0x008BACD4, 20000.0f); // spawnManager : removerange

                // Make rain drops being blocked by vobs!
                Process.Write(0x5E227A, (byte)0xE0);

                // Blocking Call Init Scripts!
                Process.Write(0x006C1F60, (byte)0xC3);
                // Blocking Call Startup Scripts!
                Process.Write(0x006C1C70, (byte)0xC3);


                Process.Write(0x0069C2DA, 0xDC, 0x0D, 0x30, 0xEB, 0x82, 0x00); // bleed with < 25% health

                Process.Write(0x0069C08B, 0xE9, 0xB0, 0x01, 0x00, 0x00); // disable player AI
                Process.Write(0x0069BFCB, 0xE9, 0x89, 0, 0, 0, 0x90); // disable focus highlighting

                Process.Nop(0x006B0896, 12); // don't let oCAniCtrl_Human::CreateHit check whether the target is an enemy

                Process.Write(0x0073E480, (byte)0xC3);//Blocking oCNpc::ProcessNpc (Dive Damage etc)
                Process.Write(0x0066CAC9, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90);//Block Damage!

                // Blocking time!
                Process.Write(0x00780D80, (byte)0xC3);

                //Process.VirtualProtect(0x007792E0, 40);
                Process.Write(0x007792E0, 0x33, 0xC0, 0xC2, 0x04, 0x00);//Block deleting of dead characters!

                //Process.Write(new byte[] { 0xB8, 0x01, 0x00, 0x00, 0x00, 0xC3 }, 0x7425A0); // oCNpc::IsAPlayer always true, DON'T DO THIS or bullshit will happen (like getting the hero's focus mode resetted because another npc sets his weaponmode)
                Process.Write(0x76D8A0, 0x31, 0xC0, 0xC3); // oCNpc_States::IsInRoutine always false

                Process.Write(0x69C247, 0xEB, 0x21); // oCAIHuman::DoAI -> skip perception check
                Process.Write(0x76D1A0, 0x31, 0xC0, 0xC3); // oCNpc_States::DoAIState -> skip and return false

                Process.Nop(0x4A059C, 7); // don't load dialogcams.zen

                Process.Write(0x76D9E0, (byte)0xC3); // remove oCNPCStates.CloseCutscenes(); // made problems with npcs as menu3dvisuals

                Process.Write(0x4A45C0, (byte)0xC3); // remove zCAICamera::CheckKeys

                Process.Nop(0x712E2C, 0x18); // don't let gothic make all items have collision

                // double zFar limit
                Process.Write(0x005E07C2, 0x830830); // indoor
                Process.Write(0x005E07D3, 0x42c60000);
                Process.Write(0x005E9EA2, 0x830830); // outdoor
                Process.Write(0x005E9EB3, 0x42c60000);
                Process.Write(0x004283AC, 10000); // applysomesettings
                Process.Write(0x004283B6, 10000);


                Logger.Log("Hooking & editing of gothic process completed. (for now...)");
                #endregion

                // Load Scripts
                Logger.Log("Loading client scripts...");
                Scripting.ScriptManager.StartScripts(Path.Combine(_ProjectPath, "Scripts", "RP_Client_Scripts.dll"));

                Logger.Log("Waiting...");
                SplashScreen.WaitHandle.WaitOne(3000);
                SplashScreen.WaitHandle = null;
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
            return 0;
        }


        public static void Exit()
        {
            GameClient.Disconnect();
            Logger.Log("Exiting...");
            Thread.Sleep(200);
            CGameManager.ExitGameVar = 1;
            zCOption.GetSectionByName("internal").GetEntryByName("gameAbnormalExit").VarValue.Set("0");
            zCOption.Save(zString.Create("Gothic.ini")); // don't dispose this zString or crashes will happen
            //Process.CDECLCALL<NullReturnCall>(0x00425F30); // ExitGameFunc
        }

        public static string GetSignature(uint y)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            foreach (ManagementObject obj in searcher.Get())
            {
                if ((uint)obj["Index"] == y)
                {
                    object sign = obj["Signature"];
                    if (sign != null && !string.IsNullOrWhiteSpace(sign.ToString()))
                        return sign.ToString();
                }
            }
            return "";
        }

        public static string GetMacAddress()
        {
            ManagementClass adapter = new ManagementClass("Win32_NetworkAdapter");
            ManagementObjectCollection collection = adapter.GetInstances();
            foreach (ManagementObject MO in collection)
            {
                if (MO != null)
                {
                    object obj = MO["MacAddress"];
                    if (obj != null && !string.IsNullOrWhiteSpace(obj.ToString()))
                    {
                        return obj.ToString();
                    }
                }
            }
            return "";
        }
    }
}
