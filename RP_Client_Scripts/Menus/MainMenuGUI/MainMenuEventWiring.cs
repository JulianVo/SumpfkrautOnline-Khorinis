using GUC.Scripts.Arena.Menus;
using GUC.Scripts.Menus;
using GUC.Scripts.Menus.CharacterCreationGUI;
using GUC.Scripts.Menus.CharacterSelectionMenu;
using GUC.Scripts.Sumpfkraut.Networking;

namespace GUC.Scripts
{
    internal sealed class MainMenuEventWiring
    {
        public MainMenuEventWiring(MainMenu mainMenu, CharCreationMenu charCreationMenu, ExitMenu exitMenu, ScriptClient client,LoginMenu loginMenu,Login.Login login,WaitScreen waitScreen, CharacterSelectionMenu selectionMenu)
        {
            mainMenu.CharacterCreationSelected += sender =>
            {
                sender.Close();
                charCreationMenu.Open();
            };

            mainMenu.FreeForAllSelected += sender =>
            {
                //sender.Close();
                //client.SendSpectateMessage();      
            };

            mainMenu.BackToLoginSelected += sender =>
            {
                sender.Close();
                waitScreen.Message = "Logout läuft...";
                waitScreen.Open();
                login.StartLogout();
            };

            mainMenu.CharacterSelectionSelected += sender =>
            {
                selectionMenu.Open();
                sender.Close();
            };

            mainMenu.ExitGameSelected += sender =>
            {
                sender.Close();
                exitMenu.Open();         
            };
        }
    }
}
