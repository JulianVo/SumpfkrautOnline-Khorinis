using System.Data.Entity;

namespace RP_Server_Scripts.Database.Account
{
    public sealed class AuthenticationContext : DbContext
    {
        internal AuthenticationContext():base("name=GothicRpDB_ConnectionString")
        {
        }

        public DbSet<AccountEntity> Accounts { get; set; }
    }
}
