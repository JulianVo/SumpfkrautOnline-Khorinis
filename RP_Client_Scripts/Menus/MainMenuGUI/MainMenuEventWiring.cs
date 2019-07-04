using GUC.Scripts.Arena.Menus;
using GUC.Scripts.Character;
using GUC.Scripts.Menus;
using GUC.Scripts.Menus.CharacterCreationGUI;
using GUC.Scripts.Menus.CharacterSelectionMenu;
using GUC.Scripts.Menus.ErrorScreenGUI;
using GUC.Scripts.Menus.LoginGUI;
using GUC.Scripts.Sumpfkraut.Networking;

namespace GUC.Scripts
{
    internal sealed class MainMenuEventWiring
    {
        public MainMenuEventWiring(
            MainMenu mainMenu,
            CharCreationMenu charCreationMenu,
            ExitMenu exitMenu,
            ScriptClient client,
            LoginMenu loginMenu,
            Login.Login login,
            WaitScreen waitScreen,
            CharacterSelectionMenu selectionMenu,
            JoinGameSender joinGameSender,
            CharacterList characterList,
            ErrorScreenManager errorScreenManager)
        {
            mainMenu.CharacterCreationSelected += sender =>
            {
                sender.Close();
                charCreationMenu.Open();
            };

            mainMenu.JoinGameSelected += sender =>
            {
                if (!characterList.TryGetActiveCharacter(out Character.Character character))
                {
                    mainMenu.Close();
                    errorScreenManager.ShowError("Kein Character gewählt!", mainMenu.Open);
                    return;
                }

                joinGameSender.StartJoinGame(character);
                waitScreen.Message = "Trete Spiel bei...";
                waitScreen.Open();
                mainMenu.Close();

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

            joinGameSender.JoinGameRequestSuccessful += sender =>
            {
                //The server accepted the join request, close the ui, from here on the server does most of the controlling.
                waitScreen.Close();
            };

            joinGameSender.JoinGameFailed += (sender, args) =>
            {
                //Something went wrong show an error message to the player and then return to the main menu.
                waitScreen.Close();
                errorScreenManager.ShowError(args.ReasonText, mainMenu.Open);
            };
        }
    }
}
