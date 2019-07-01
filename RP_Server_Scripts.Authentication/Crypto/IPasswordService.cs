namespace RP_Server_Scripts.Authentication
{
    public interface IPasswordService
    {
        string CreatePasswordHash(string password);
        bool VerifyPassword(string password, string hash);
    }
}