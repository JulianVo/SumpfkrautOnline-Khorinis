﻿using GUC.Utilities;
using WinApi.User.Enumeration;
using System.IO;

namespace GUC.Scripts.Sumpfkraut.Options
{
    public static class ClientOptions
    {
        const string FilePath = "options.xml";
        

        public struct Options
        {
            public VirtualKeys key1;
        }

        static Options options = new Options();

        public static void Save()
        {
            XMLHelper.SaveObject<Options>(options, Path.Combine(Program.ProjectPath, FilePath));
        }

        public static void Load()
        {
            options = XMLHelper.LoadObject<Options>(FilePath);
        }
    }
}
