using RP_Shared_Script;

namespace RP_Server_Scripts.Character
{
    public sealed class CharacterCreationFailed : CharacterCreationResult
    {
        public CharacterCreationFailed(CharacterCreationFailure reason) : base(false)
        {
            Reason = reason;
        }

        public CharacterCreationFailure Reason { get; }
    }
}