using System;
using System.Linq;
using System.Threading.Tasks;
using RP_Server_Scripts.Authentication;
using RP_Server_Scripts.Database;
using RP_Server_Scripts.Database.Character;
using RP_Server_Scripts.Logging;

namespace RP_Server_Scripts.Character.Transaction
{
    internal sealed class GetCharacterOwnershipsCountTransaction
    {
        private readonly ICharacterManagementContextFactory _ContextFactory;
        private readonly ILogger _Log;

        public GetCharacterOwnershipsCountTransaction(ICharacterManagementContextFactory contextFactory, ILoggerFactory loggerFactory)
        {
            _ContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _Log = loggerFactory?.GetLogger(GetType()) ?? throw new ArgumentNullException(nameof(loggerFactory));
        }


        public Task<int> GetCharacterOwnershipsCountAsync(Account account)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }
            var task = new Task<int>(() =>
            {
                try
                {
                    using (var db = _ContextFactory.Create())
                    {
                        return db.CharacterOwnerships.Count(os => os.OwnerId == account.AccountId);
                    }
                }
                catch (Exception e)
                {
                    string msg =
                        $"Something went wrong while requesting the amount of character ownerships for account '{account.UserName}' from the database";
                    _Log.Error(msg);
                    throw new DatabaseRequestException(msg, e);
                }
            });

            //Start the task on the thread pool and return it.
            task.Start();
            return task;
        }
    }
}
