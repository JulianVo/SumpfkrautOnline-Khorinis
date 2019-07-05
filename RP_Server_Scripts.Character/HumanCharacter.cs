namespace RP_Server_Scripts.Character
{
    public sealed class HumanCharacter : Character
    {
        internal HumanCharacter(int characterId, CharacterService characterService) : base(characterId, characterService)
        {
        }

        public HumanCharacterVisuals HumanVisuals { get; internal set; }
    }
}