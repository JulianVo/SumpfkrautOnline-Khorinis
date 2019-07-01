using System;

namespace RP_Server_Scripts.Authentication
{
    public sealed class AccountCreatedEventArgs
    {
        public AccountCreatedEventArgs(Account newAccount)
        {
            NewAccount = newAccount ?? throw new ArgumentNullException(nameof(newAccount));
        }

        public Account NewAccount { get; }

    }
}
