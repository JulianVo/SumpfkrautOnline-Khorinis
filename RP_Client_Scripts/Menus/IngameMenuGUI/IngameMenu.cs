using GUC.Scripts.Sumpfkraut.Menus.MainMenus;
using RP_Shared_Script;

namespace GUC.Scripts.Menus.IngameMenu
{
    internal class InGameMenu : GUCMainMenu
    {
        public event GenericEventHandler<InGameMenu> BackToMainMenu;
        public event GenericEventHandler<InGameMenu> ExitGameSelected;
        public event GenericEventHandler<InGameMenu> BackToLoginSelected;

        protected override void OnCreate()
        {
            AddButton("Hauptmenü", " Zurück zum Hauptmenü.", 180, () => BackToMainMenu?.Invoke(this));
            AddButton("Logout", "Zurück zum Login Menü", 260, () => BackToLoginSelected?.Invoke(this));
            AddButton("Spiel verlassen", "Das Spiel schließen.", 320, () => ExitGameSelected?.Invoke(this));
        }
    }
}
