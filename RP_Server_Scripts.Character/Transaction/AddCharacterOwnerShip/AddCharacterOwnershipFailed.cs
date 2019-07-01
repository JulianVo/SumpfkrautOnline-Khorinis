using System;
using System.ComponentModel;
using RP_Server_Scripts.Authentication;

namespace RP_Server_Scripts.Character.Transaction.AddCharacterOwnerShip
{
    public sealed class AddCharacterOwnershipFailed : AddCharacterOwnershipResult
    {
        public AddCharacterOwnershipFailure Reason { get; }

        public AddCharacterOwnershipFailed(Account account, Character character, AddCharacterOwnershipFailure reason) : base(false, account, character)
        {
            if (!Enum.IsDefined(typeof(AddCharacterOwnershipFailure), reason))
            {
                throw new InvalidEnumArgumentException(nameof(reason), (int) reason,
                    typeof(AddCharacterOwnershipFailure));
            }

            Reason = reason;
        }
    }
}