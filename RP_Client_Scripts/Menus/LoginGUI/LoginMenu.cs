using System;
using GUC.GUI;
using GUC.Scripts.Sumpfkraut.GUI.MainMenu;
using GUC.Types;
using RP_Shared_Script;

namespace GUC.Scripts.Menus
{
    internal sealed class LoginMenu : MenuWithViewBlocker
    {
        public event GenericEventHandler<LoginMenu, CredentialsEnteredArgs> CredentialsEntered;
        public event GenericEventHandler<LoginMenu> CreateAccountSelected;

        private MainMenuTextBox _TbName;
        private MainMenuTextBox _TbPw;
        private GUCVisualText _LbErrorText;

        protected override void OnCreate()
        {
            base.OnCreate();

            Back.CreateTextCenterX("Login", 100);

            _TbName = AddTextBox("Accountname:", "Name deines Accounts eingeben.", 200, 200, OnActivate);


            _TbPw = AddTextBox("Passwort:", "Passwort deines Accounts eingeben.", 250, 200, OnActivate, true);


            AddButton("Login ausführen", "Mit den eingegebenen Accountdaten einloggen.", 300, OnActivate);


            AddButton("Account erstellen", "Einen neuen Account erstellen", 400, () => CreateAccountSelected?.Invoke(this));


            _LbErrorText = Back.CreateTextCenterX("", 350);
            _LbErrorText.SetColor(ColorRGBA.Red);
        }

        private void OnActivate()
        {
            if (string.IsNullOrWhiteSpace(_TbName.Input) || string.IsNullOrWhiteSpace(_TbPw.Input))
            {
                SetErrorText("Accountname und Password eingegeben.");
            }
            else
            {
                CredentialsEntered?.Invoke(this, new CredentialsEnteredArgs(_TbName.Input, _TbPw.Input));
            }
        }

        public void SetErrorText(string errorText)
        {
            _LbErrorText.Text = errorText ?? throw new ArgumentNullException(nameof(errorText));
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
