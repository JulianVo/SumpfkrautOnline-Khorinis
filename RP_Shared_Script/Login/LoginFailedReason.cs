namespace RP_Shared_Script.Login
{
    public enum LoginFailedReason:byte
    {
        None=0,
        InvalidLoginData=1,
        Banned=2,
        UserNameAlreadyInUse=3,
        Timeout=4,
    }
}