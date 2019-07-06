namespace RP_Shared_Script
{
    public enum JoinGameFailedReason:byte
    {
        None,
        CharacterInUse,
        Timeout,
        MessageHandlingError,
        InvalidCharacterId
    }
}
