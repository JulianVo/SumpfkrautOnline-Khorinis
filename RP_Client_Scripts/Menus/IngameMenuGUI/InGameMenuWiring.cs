using GUC.Scripts.Arena.Menus;
using GUC.Scripts.Menus;
using GUC.Scripts.Menus.IngameMenu;
using GUC.Scripts.Sumpfkraut.Networking;

namespace GUC.Scripts.GuiEventWiring
{
    internal sealed class InGameMenuWiring
    {
        public InGameMenuWiring(ScriptClient client,InGameMenu inGameMenu,ExitMenu exitMenu,WaitScreen waitScreen,Login.Login login,CharacterList characterList, LeaveGameSender leaveMsgSender)
        {
            inGameMenu.BackToMainMenu += sender =>
            {
                leaveMsgSender.SendLeaveGameMessage();
                waitScreen.Message = "Lade Charakterliste...";
                waitScreen.Open();
                sender.Close();
                characterList.RefreshFromServer();
            };


            inGameMenu.BackToLoginSelected += sender =>
            {
                sender.Close();
                waitScreen.Message = "Logout läuft...";
                waitScreen.Open();
                login.StartLogout();
            };


            inGameMenu.ExitGameSelected += sender =>
            {
                sender.Close();
                exitMenu.Open();
            };

            inGameMenu.OnEscape += sender => sender.Close();
        }
    }
}
