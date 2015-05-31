﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUC.Sumpfkraut.GUI;

namespace GUC.Sumpfkraut.Menus
{
    partial class MainMenus
    {
        class RPGuideMenu : AbstractMenu
        {
            //massive text wall
            //back

            protected override void CreateMenu()
            {
                thisMenu = new GUCMainMenu(0);
                thisMenu.AddText("RP-Guide", 100);
                thisMenu.AddMenuButton("Zurück", "Zurück zum Hauptmenü.", Main.Open, 350);
                thisMenu.OnEscape = Main.Open;
            }
        }
    }
}