using System;
using GUC.GUI;
using GUC.Scripts.Sumpfkraut.GUI.MainMenu;
using GUC.Types;
using RP_Shared_Script;

namespace GUC.Scripts.Menus.AccountCreationMenu
{
    internal sealed class AccountCreationMenu : MenuWithViewBlocker
    {
        public event GenericEventHandler<AccountCreationMenu> ChancelSelected;
        public event GenericEventHandler<AccountCreationMenu> CreationInformationEntered; 

        private MainMenuTextBox _TbName;
        private MainMenuTextBox _TbPw;
        private MainMenuTextBox _TbPw2;
        private GUCVisualText _LbErrorText;

        protected override void OnCreate()
        {
            base.OnCreate();
            var screenSize = GUCView.GetScreenSize();
            Back.SetPos((screenSize.X - 900) / 2, (screenSize.Y - 480) / 2);
            Back.SetSize(900,480);

            Back.CreateTextCenterX("Login", 100);

            _TbName = AddTextBox("Accountname:", "Name deines Accounts eingeben.", 200, 200, OnActivate);


            _TbPw = AddTextBox("Passwort:", "Passwort deines Accounts eingeben.", 250, 200, OnActivate, true);

            _TbPw2 = AddTextBox("Passwort wdh.:", "Passwort deines Accounts wiederholen.", 300, 200, OnActivate, true);

            AddButton("erstellen", "Die Erstellung des Accounts ausführen", 350,  OnActivate);

            AddButton("Zurück", "Die Erstellung des Accounts abbrechen", 400, () => ChancelSelected?.Invoke(this));

            _LbErrorText = Back.CreateTextCenterX("", 400);
            _LbErrorText.SetColor(ColorRGBA.Red);
        }

        private void OnActivate()
        {
            if (string.IsNullOrWhiteSpace(_TbName.Input) || string.IsNullOrWhiteSpace(_TbPw.Input)|| string.IsNullOrWhiteSpace(_TbPw2.Input))
            {
                SetErrorText("Accountname und Password und Passwordwiederholung eingegeben!");
            }
            else if (!_TbPw.Input.Equals(_TbPw2.Input))
            {
                SetErrorText("Password und Wiederholung stimmen nicht überein!");
            }
            else
            {
                CreationInformationEntered?.Invoke(this);
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
