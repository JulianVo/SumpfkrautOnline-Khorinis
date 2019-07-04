using System;
using GUC.Scripts.Sumpfkraut.Menus.MainMenus;

namespace GUC.Scripts.Menus.ErrorScreenGUI
{
    internal sealed class ErrorScreenManager
    {
        private readonly ErrorScreen _ErrorScreen;
        private Action _LastCallBack;

        public ErrorScreenManager(ErrorScreen errorScreen)
        {
            _ErrorScreen = errorScreen ?? throw new ArgumentNullException(nameof(errorScreen));
            _ErrorScreen.OnEscape+=ErrorScreenOnOnEscape;
        }

        private void ErrorScreenOnOnEscape(GUCMainMenu sender)
        {
            _ErrorScreen.Close();
            _LastCallBack?.Invoke();
            _LastCallBack = null;
        }

        public void ShowError(string message, Action closingCallback)
        {
            if (_LastCallBack != null)
            {
                throw new InvalidOperationException("The error screen has already been opened.");
            }

            _LastCallBack = closingCallback ?? throw new ArgumentNullException(nameof(closingCallback));
            _ErrorScreen.Message = message ?? throw new ArgumentNullException(nameof(message));
            _ErrorScreen.Open();
        }
    }
}
