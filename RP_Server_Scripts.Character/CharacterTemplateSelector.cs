using GUC.Scripts.ReusedClasses;
using RP_Shared_Script;

namespace RP_Server_Scripts.Character
{
    internal sealed class CharacterTemplateSelector : ICharacterTemplateSelector
    {
        public string GetTemplate(CharCreationInfo charInfo)
        {
            return charInfo.BodyMesh == HumBodyMeshs.HUM_BODY_NAKED0 ? "maleplayer" : "femaleplayer";
        }
    }
}
