using System.Collections.Generic;
using System.Linq;
using GUC.Scripts.Menus;
using GUC.Scripts.Menus.AccountCreationMenu;

namespace GUC.Scripts.GuiEventWiring
{
    internal sealed class LoginMenuWiring
    {
        public LoginMenuWiring(LoginMenu loginMenu,WaitScreen waitScreen, Login.Login login,MainMenu mainMenu,IEnumerable<IClosableMenu> closableMenus, CharacterList characterList, AccountCreationMenu accountCreationMenu)
        {
            var menus = closableMenus.ToArray();

            //Login credentials have been entered. Switch to the waiting screen and try to login an the server.
            loginMenu.CredentialsEntered += (sender, args) =>
            {
                waitScreen.Message = "Login läuft...";
                waitScreen.Open();
                loginMenu.Close();
                login.StartLogin(args.UserName, args.Password);
            };

            //Login got denied for some reason. Lets go back to the login menu.
            login.LoginDenied += (sender, args) =>
            {
                loginMenu.Open();
                waitScreen.Close();
                loginMenu.SetErrorText($"Login fehlgeschlagen: {args.ReasonText}");
            };

            //Login successful. Go to the main menu.
            login.LoginAcknowledged += (sender, args) =>
            {
                waitScreen.Message = "Lade Charakterliste...";
                waitScreen.Open();
                characterList.RefreshFromServer();
            };

            //Open the account creation menu.
            loginMenu.CreateAccountSelected += sender =>
            {
                accountCreationMenu.Open();
                loginMenu.Close();
            };


            login.LoggedOut += sender =>
            {
                //CGameManager.ExitSession();
                //Close all menus on logout(its somewhat of a cleanup).
                foreach (var closableMenu in menus)
                {
                    closableMenu.Close();
                }
                loginMenu.Open();
            };
        }
    }
}
