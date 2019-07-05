using System;
using RP_Server_Scripts.VobSystem.Instances;

namespace RP_Server_Scripts.Character
{
    public sealed class CharacterMapping
    {
        public CharacterMapping(Character character, NpcInst characterNpc)
        {
            Character = character ?? throw new ArgumentNullException(nameof(character));
            CharacterNpc = characterNpc ?? throw new ArgumentNullException(nameof(characterNpc));
        }

        public Character Character { get; }

        public NpcInst CharacterNpc { get; }
    }
}
