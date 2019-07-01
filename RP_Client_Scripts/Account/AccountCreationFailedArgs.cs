using System;

namespace GUC.Scripts.Account
{
    internal sealed class AccountCreationFailedArgs
    {
        public AccountCreationFailedArgs(string reasonText)
        {
            ReasonText = reasonText ?? throw new ArgumentNullException(nameof(reasonText));
        }

        public string ReasonText { get; }
    }
}
