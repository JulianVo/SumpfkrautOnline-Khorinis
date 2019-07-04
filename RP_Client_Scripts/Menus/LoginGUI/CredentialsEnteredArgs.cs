using System;

namespace GUC.Scripts.Menus.LoginGUI
{
    internal sealed class CredentialsEnteredArgs
    {
        public CredentialsEnteredArgs(string userName, string password)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }

        public string UserName { get; }

        public string Password { get; }
    }
}
