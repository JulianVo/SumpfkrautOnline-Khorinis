using GUC.Scripts.Sumpfkraut.VobSystem.Definitions;

namespace GUC.Scripts.Character
{
    public sealed class NonHumanCharacter : Character
    {
        public NonHumanCharacter(int characterId, string name, NPCDef template) : base(characterId, name, template)
        {
        }
    }
}