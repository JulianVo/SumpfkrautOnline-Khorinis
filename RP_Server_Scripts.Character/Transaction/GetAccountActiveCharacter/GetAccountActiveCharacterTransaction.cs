using System;
using System.Linq;
using System.Threading.Tasks;
using RP_Server_Scripts.Authentication;
using RP_Server_Scripts.Database;
using RP_Server_Scripts.Database.Character;
using RP_Server_Scripts.Logging;

namespace RP_Server_Scripts.Character.Transaction
{
    internal sealed class GetAccountActiveCharacterTransaction
    {
        private readonly ICharacterManagementContextFactory _ContextFactory;
        private readonly ILogger _Log;

        public GetAccountActiveCharacterTransaction(ICharacterManagementContextFactory contextFactory, ILoggerFactory loggerFactory)
        {
            _ContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _Log = loggerFactory?.GetLogger(GetType()) ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public Task<ActiveCharacterResult> GetAccountActiveCharacterAsync(Account account)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            Task<ActiveCharacterResult> task = new Task<ActiveCharacterResult>(() =>
            {
                try
                {
                    using (var db = _ContextFactory.Create())
                    {
                        AccountLastUsedCharacterEntity lastUsedEntity = db.LastUsedCharacters.FirstOrDefault(lc => lc.AccountId == account.AccountId);
                        if (lastUsedEntity != null)
                        {
                            if (!db.CharacterOwnerships.Any(os =>
                                os.OwnerId == account.AccountId && os.CharacterId == lastUsedEntity.CharacterId))
                            {
                                //We found a last used character but it is not owned by the given account(ownership was removed since setting the active character). Remove the database entity.
                                db.LastUsedCharacters.Remove(lastUsedEntity);
                                return new NoActiveCharacterFound(account);
                            }

                            //Last used character was found and it is still owned by the given account.
                            return new ActiveCharacterFound(account, lastUsedEntity.CharacterId);

                        }

                        // No last used character found.
                        return new NoActiveCharacterFound(account);
                    }
                }
                catch (Exception e)
                {
                    string msg =
                        $"Something went wrong while requesting the last used character of the account '{account.UserName}' from the database";
                    _Log.Error(msg);
                    throw new DatabaseRequestException(msg, e);
                }
            });
            task.Start();
            return task;
        }
    }
}
