using System;
using RP_Server_Scripts.Authentication;

namespace RP_Server_Scripts.Character
{
    public sealed class CharacterCreatedArgs
    {
        public CharacterCreatedArgs(Character character)
        {
            Character = character ?? throw new ArgumentNullException(nameof(character));
        }


        public Character Character { get; }
    }
}
