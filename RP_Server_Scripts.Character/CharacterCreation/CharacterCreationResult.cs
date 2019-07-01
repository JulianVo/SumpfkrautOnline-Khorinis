namespace RP_Server_Scripts.Character
{
    public abstract class CharacterCreationResult
    {
        protected CharacterCreationResult(bool success)
        {
            Success = success;
        }

        public bool Success { get; }
    }
}
