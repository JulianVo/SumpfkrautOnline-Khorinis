using System;
using RP_Server_Scripts.Authentication;

namespace RP_Server_Scripts.Character
{
    public abstract class ActiveCharacterResult
    {
        protected ActiveCharacterResult(bool isSuccessful, Account account)
        {
            IsSuccessful = isSuccessful;
            Account = account ?? throw new ArgumentNullException(nameof(account));
        }

        public bool IsSuccessful { get; }
        public Account Account { get; }
    }
}
