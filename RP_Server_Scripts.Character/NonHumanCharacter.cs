
namespace RP_Server_Scripts.Character
{
    public sealed class NonHumanCharacter : Character
    {
        public NonHumanCharacter(int characterId,CharacterService characterService) : base(characterId, characterService)
        {
            Name = "Monster";
        }
    }
}