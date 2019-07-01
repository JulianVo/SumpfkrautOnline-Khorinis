namespace RP_Server_Scripts.Character
{
    public sealed class HumanCharacter : Character
    {
        internal HumanCharacter(int characterId) : base(characterId)
        {
        }

        public HumanCharacterVisuals HumanVisuals { get; internal set; }
    }
}