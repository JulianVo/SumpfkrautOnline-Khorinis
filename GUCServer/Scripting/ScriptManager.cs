using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using GUC.Network;
using GUC.WorldObjects;
using GUC.Animations;
using GUC.Models;
using GUC.WorldObjects.Instances;
using GUC.WorldObjects.VobGuiding;
using GUC.Types;

namespace GUC.Scripting
{
    public partial interface ScriptInterface
    {
        GameClient CreateClient();
        Overlay CreateOverlay();
        AniJob CreateAniJob();
        Animation CreateAnimation();
        ModelInstance CreateModelInstance();
        BaseVob CreateVob(byte type);
        BaseVobInstance CreateInstance(byte type);
        World CreateWorld();
        GuideCmd CreateGuideCommand(byte type);
    }

    static class ScriptManager
    {


        static ScriptManager()
        {
            //If assemblies can not be found, search inside the scripts folder
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string assemblyDir =
                    Path.GetDirectoryName(new Uri(typeof(ScriptManager).Assembly.CodeBase).AbsolutePath) ??
                    throw new InvalidOperationException();
                string possiblePath = Path.Combine(assemblyDir, "Scripts", args.Name.Split(',').First() + ".dll");
                if (File.Exists(possiblePath))
                {
                    return Assembly.LoadFrom(possiblePath);
                }

                return null;
            };
        }

        static Assembly asm = null;
        public static ScriptInterface Interface { get; private set; }

        public static void StartScripts(string path)
        {
            if (asm != null)
                return;

            try
            {
                asm = Assembly.LoadFile(Path.GetFullPath(path));
                Interface = (ScriptInterface)asm.CreateInstance("RP_Server_Scripts.GUCScripts");
            }
            catch (ReflectionTypeLoadException refException)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in refException.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();
                //Display or log the error based on your application.
                Log.Logger.LogError(Environment.CurrentDirectory + "\n" + errorMessage);
            }
            catch (Exception e)
            {
                Log.Logger.LogError(Environment.CurrentDirectory + "\n" + e);
            }
        }
    }
}
