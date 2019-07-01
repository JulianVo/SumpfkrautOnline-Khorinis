using System;
using GUC.Scripts.Sumpfkraut.VobSystem.Definitions;

namespace GUC.Scripts.Character
{
    public sealed class HumanCharacter : Character
    {
        public HumanCharacterVisuals CharacterVisuals { get; }

        public HumanCharacter(int characterId, string name, HumanCharacterVisuals characterVisuals, NPCDef template) : base(characterId, name, template)
        {
            CharacterVisuals = characterVisuals ?? throw new ArgumentNullException(nameof(characterVisuals));
        }
    }
}