
namespace RP_Server_Scripts.Character
{
    public sealed class NonHumanCharacter : Character
    {
        public NonHumanCharacter(int characterId) : base(characterId)
        {
            Name = "Monster";
        }
    }
}