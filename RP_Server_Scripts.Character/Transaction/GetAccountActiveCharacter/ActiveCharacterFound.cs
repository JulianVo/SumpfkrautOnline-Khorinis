using System;
using RP_Server_Scripts.Authentication;

namespace RP_Server_Scripts.Character
{
    public sealed class ActiveCharacterFound : ActiveCharacterResult
    {
        public ActiveCharacterFound(Account account, int characterId) : base(true, account)
        {
            if (characterId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(characterId));
            }

            CharacterId = characterId;
        }

        public int CharacterId { get; }
    }
}