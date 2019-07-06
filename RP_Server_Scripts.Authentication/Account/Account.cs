using System;
using System.Diagnostics;
using RP_Shared_Script;

namespace RP_Server_Scripts.Authentication
{
    [DebuggerDisplay("Account: {" + nameof(UserName) + "}")]
    public sealed class Account
    {
        public string UserName { get; }

        public int AccountId { get; }

        private readonly string _PasswordHash;

        public event GenericEventHandler<Account, LogoutEventArgs> LoggedOut;

        public bool IsLoggedIn { get; private set; }

        internal Account(string userName, int accountId, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(userName));
            }

            if (accountId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(accountId));
            }

            if (string.IsNullOrEmpty(passwordHash))
            {
                throw new ArgumentException(@"Value cannot be null or empty.", nameof(passwordHash));
            }


            UserName = userName;
            AccountId = accountId;
            _PasswordHash = passwordHash;
            IsLoggedIn = true;
        }

        public bool CheckPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(password));
            }

            return BCrypt.Net.BCrypt.Verify(password, _PasswordHash);
        }


        internal void OnLoggedOut(LogoutEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            IsLoggedIn = false;
            LoggedOut?.Invoke(this, args);
        }
    }
}
