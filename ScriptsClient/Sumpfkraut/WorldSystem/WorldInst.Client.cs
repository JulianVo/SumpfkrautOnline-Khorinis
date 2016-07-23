﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gothic;
using GUC.Scripts.Sumpfkraut.Menus;

namespace GUC.Scripts.Sumpfkraut.WorldSystem
{
    public partial class WorldInst
    {
        public static WorldInst Current { get { return (WorldInst)WorldObjects.World.Current.ScriptObject; } }

        public void Load()
        {
            GUCMenu.CloseActiveMenus();
            //if (oCGame.GetWorld().LevelName != worldName)
            oCGame.LoadGame(true, "OLDWORLD\\OLDWORLD.ZEN");
        }
    }
}
