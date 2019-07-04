using System.Collections.Generic;
using System.Linq;
using GUC.Scripts.Menus.AccountCreationGUI;
using GUC.Scripts.Menus.ErrorScreenGUI;

namespace GUC.Scripts.Menus.LoginGUI
{
    internal sealed class LoginMenuWiring
    {
        public LoginMenuWiring(
            LoginMenu loginMenu,
            WaitScreen waitScreen,
            Login.Login login,
            IEnumerable<IClosableMenu> closableMenus,
            CharacterList characterList,
            AccountCreationMenu accountCreationMenu,
            ErrorScreenManager errorScreenManager)
        {
            var menus = closableMenus.ToArray();

            //Login credentials have been entered. Switch to the waiting screen and try to login an the server.
            loginMenu.CredentialsEntered += (sender, args) =>
            {
                //Missing parts of the login information, show an error message.
                if (string.IsNullOrWhiteSpace(args.UserName) || string.IsNullOrWhiteSpace(args.Password))
                {
                    errorScreenManager.ShowError("Accountname und Password eingeben!", loginMenu.Open);
                    return;
                }

                waitScreen.Message = "Login läuft...";
                waitScreen.Open();
                loginMenu.Close();
                login.StartLogin(args.UserName, args.Password);
            };

            //Login got denied for some reason. Show an error message and the go back to the login screen.
            login.LoginDenied += (sender, args) =>
            {
                waitScreen.Close();
                errorScreenManager.ShowError($"Login fehlgeschlagen: {args.ReasonText}", loginMenu.Open);
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
