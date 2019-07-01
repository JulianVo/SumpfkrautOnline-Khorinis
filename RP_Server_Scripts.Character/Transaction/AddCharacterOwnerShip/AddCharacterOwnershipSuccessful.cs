using RP_Server_Scripts.Authentication;

namespace RP_Server_Scripts.Character.Transaction.AddCharacterOwnerShip
{
    public sealed class AddCharacterOwnershipSuccessful : AddCharacterOwnershipResult
    {
        public AddCharacterOwnershipSuccessful(Account account, Character character) : base(true, account, character)
        {
        }
    }
}