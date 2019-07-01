﻿using GUC.GUI;

namespace GUC.Scripts.Sumpfkraut.Menus.MainMenus
{
    class HelpMenu : GUCMainMenu
    {
        public readonly static HelpMenu Menu = new HelpMenu();

        GUCVisual keyHelp;

        protected override void OnCreate()
        {
            Back.CreateTextCenterX("Kurzhilfe", 100);
            
            AddButton("Chatbefehle", "Eine Liste aller nutzbaren Chatbefehle.", 180, HelpChatMenu.Menu.Open);
            AddButton("RP-Guide", "Eine kurze Einführung ins Rollenspiel.", 220, HelpRPMenu.Menu.Open);

            const int offsetY = 280;
            const int dist = 24;
            keyHelp = (GUCVisual)Back.AddChild(new GUCVisual());
            keyHelp.CreateTextCenterX("ENTER - Chat öffnen", pos[1] + offsetY + dist * 0);
            keyHelp.CreateTextCenterX("F2 - Wechsel zw. OOC-/RP-Chat", pos[1] + offsetY + dist * 1);
            keyHelp.CreateTextCenterX("T - Handelsanfrage", pos[1] + offsetY + dist * 2);
            keyHelp.CreateTextCenterX("X - Animationsmenü", pos[1] + offsetY + dist * 3);

            AddButton("Zurück", "Zurück zum Hauptmenü.",400, MainMenu.Menu.Open);
            //OnEscape = MainMenu.Menu.Open;
        }
    }
}
