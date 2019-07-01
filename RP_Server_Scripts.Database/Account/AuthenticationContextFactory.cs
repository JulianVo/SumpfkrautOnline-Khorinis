namespace RP_Server_Scripts.Database.Account
{
    internal class AuthenticationContextFactory : IAuthenticationContextFactory
    {
        public AuthenticationContextFactory()
        {
            using (var context = new AuthenticationContext())
            {
                context.Database.CreateIfNotExists();
            }
        }

        public AuthenticationContext CreateContext()
        {
            return  new AuthenticationContext();
        }
    }
}