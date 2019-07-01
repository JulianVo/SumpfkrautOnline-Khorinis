using RP_Server_Scripts.Authentication;

namespace RP_Server_Scripts.Character
{
    public sealed class NoActiveCharacterFound : ActiveCharacterResult
    {
        public NoActiveCharacterFound(Account account) : base(false, account)
        {
        }
    }
}