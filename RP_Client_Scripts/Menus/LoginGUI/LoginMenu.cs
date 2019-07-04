using GUC.Scripts.Sumpfkraut.GUI.MainMenu;
using RP_Shared_Script;

namespace GUC.Scripts.Menus.LoginGUI
{
    internal sealed class LoginMenu : MenuWithViewBlocker
    {
        public event GenericEventHandler<LoginMenu, CredentialsEnteredArgs> CredentialsEntered;
        public event GenericEventHandler<LoginMenu> CreateAccountSelected;

        private MainMenuTextBox _TbName;
        private MainMenuTextBox _TbPw;

        protected override void OnCreate()
        {
            base.OnCreate();

            Back.CreateTextCenterX("Login", 100);

            _TbName = AddTextBox("Accountname:", "Name deines Accounts eingeben.", 200, 200, OnActivate);


            _TbPw = AddTextBox("Passwort:", "Passwort deines Accounts eingeben.", 250, 200, OnActivate, true);


            AddButton("Login ausführen", "Mit den eingegebenen Accountdaten einloggen.", 300, OnActivate);


            AddButton("Account erstellen", "Einen neuen Account erstellen", 400, () => CreateAccountSelected?.Invoke(this));

        }

        private void OnActivate()
        {
            CredentialsEntered?.Invoke(this, new CredentialsEnteredArgs(_TbName.Input, _TbPw.Input));
        }

        public string UserName
        {
            get => _TbName.Input;
            set => _TbName.Input = value;
        }

        public string Password
        {
            get => _TbPw.Input;
            set => _TbPw.Input = value;
        }
    }
}
