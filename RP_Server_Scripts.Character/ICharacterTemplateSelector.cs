using GUC.Scripts.ReusedClasses;

namespace RP_Server_Scripts.Character
{
    public interface ICharacterTemplateSelector
    {
        string GetTemplate(CharCreationInfo charInfo);
    }
}