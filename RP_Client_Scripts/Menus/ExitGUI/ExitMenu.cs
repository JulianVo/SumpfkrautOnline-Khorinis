using System;
using GUC.Scripts.Sumpfkraut.Menus.MainMenus;
using RP_Shared_Script;

namespace GUC.Scripts.Arena.Menus
{
    internal class ExitMenu : GUCMainMenu
    {
        public event GenericEventHandler<ExitMenu> ExitGameSelected;
        public event GenericEventHandler<ExitMenu> BackToMainMenu;




        protected override void OnCreate()
        {
            preferredCursorItem = 1;
            Back.CreateTextCenterX("SumpfkrautOnline verlassen?", 100);
            AddButton("Ja", "Ja, ich möchte SumpfkrautOnline verlassen.", 200, () => ExitGameSelected?.Invoke(this));
            AddButton("Nein", "Zurück zum Hauptmenü", 250, () => BackToMainMenu?.Invoke(this));
        }
    }
}
