namespace RP_Server_Scripts.Database.Account
{
    public interface IAuthenticationContextFactory
    {
        AuthenticationContext CreateContext();
    }
}
