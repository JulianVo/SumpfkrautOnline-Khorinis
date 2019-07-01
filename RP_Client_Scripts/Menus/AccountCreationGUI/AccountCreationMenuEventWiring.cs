using GUC.Scripts.Account;
using GUC.Scripts.Menus;
using GUC.Scripts.Menus.AccountCreationMenu;

namespace GUC.Scripts.GuiEventWiring
{
    internal sealed class AccountCreationMenuEventWiring
    {
        public AccountCreationMenuEventWiring(AccountCreationMenu accountCreationMenu, LoginMenu loginMenu, AccountCreation accountCreation,WaitScreen waitScreen)
        {
            //Escape was pressed
            accountCreationMenu.OnEscape += sender =>
            {
                loginMenu.Open();
                accountCreationMenu.Close();
            };

            //Account creation canceled.
            accountCreationMenu.ChancelSelected += sender =>
            {
                loginMenu.Open();
                accountCreationMenu.Close();
            };

            //Account creation started.
            accountCreationMenu.CreationInformationEntered += sender =>
            {
                waitScreen.Open();
                waitScreen.Message = "Accounterstellung läuft...";
                sender.Close();
                accountCreation.StartAccountCreation(accountCreationMenu.UserName, accountCreationMenu.Password);
            };

            //Account creation successful return to login menu
            accountCreation.AccountCreationSuccessful += sender =>
            {
                loginMenu.UserName = accountCreationMenu.UserName;
                loginMenu.Password = accountCreationMenu.Password;
                loginMenu.Open();

                accountCreationMenu.Close();
                waitScreen.Close();
            };

            //Account creation failed. Display error.
            accountCreation.AccountCreationFailed += (sender, args) =>
            {
                accountCreationMenu.SetErrorText(args.ReasonText);
                accountCreationMenu.Open();
                waitScreen.Close();
            };

        }
    }
}
