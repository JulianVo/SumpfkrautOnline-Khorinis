using System;

namespace RP_Server_Scripts.Database.Account
{
    internal class AccountCreationResult
    {
        public AccountCreationResult(bool successful, AccountEntity accountDto)
        {
            Successful = successful;
            AccountDto = accountDto;
            if (successful && accountDto == null)
            {
                throw new ArgumentException($"if argument '{nameof(successful)}' is true '{nameof(accountDto)}' must not be null.", nameof(accountDto));
            }
        }

        public bool Successful { get; }

        public AccountEntity AccountDto { get; }
    }
}
