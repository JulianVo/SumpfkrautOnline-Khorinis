using System;
using RP_Server_Scripts.Authentication;

namespace RP_Server_Scripts.Character
{
    public sealed class CharacterCreatedArgs
    {
        public CharacterCreatedArgs(Account creator, Character character)
        {
            Creator = creator ?? throw new ArgumentNullException(nameof(creator));
            Character = character ?? throw new ArgumentNullException(nameof(character));
        }

        public Account Creator { get; }
        public Character Character { get; }
    }
}
