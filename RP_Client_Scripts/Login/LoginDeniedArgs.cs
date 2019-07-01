using System;

namespace GUC.Scripts.Login
{
    internal sealed class LoginDeniedArgs
    {
        public LoginDeniedArgs(string reasonText)
        {
            ReasonText = reasonText ?? throw new ArgumentNullException(nameof(reasonText));
        }

        public string ReasonText { get; }
    }
}
