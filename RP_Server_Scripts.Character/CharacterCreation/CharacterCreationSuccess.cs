using System;

namespace RP_Server_Scripts.Character
{
    public sealed class CharacterCreationSuccess : CharacterCreationResult
    {
        public CharacterCreationSuccess(Character character) : base(true)
        {
            Character = character ?? throw new ArgumentNullException(nameof(character));
        }

        public Character Character { get; }
    }
}