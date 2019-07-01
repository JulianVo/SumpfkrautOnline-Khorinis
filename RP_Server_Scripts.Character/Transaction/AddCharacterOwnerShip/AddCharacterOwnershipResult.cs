using System;
using RP_Server_Scripts.Authentication;

namespace RP_Server_Scripts.Character.Transaction.AddCharacterOwnerShip
{
    public abstract class AddCharacterOwnershipResult
    {
        protected AddCharacterOwnershipResult(bool successful, Account account, Character character)
        {
            Successful = successful;
            Account = account ?? throw new ArgumentNullException(nameof(account));
            Character = character ?? throw new ArgumentNullException(nameof(character));
        }

        public bool Successful { get; }
        public Account Account { get; }
        public Character Character { get; }
    }
}
