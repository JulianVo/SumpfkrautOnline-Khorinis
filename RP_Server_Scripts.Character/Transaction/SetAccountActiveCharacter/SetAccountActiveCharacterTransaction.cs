using System;
using System.Linq;
using System.Threading.Tasks;
using RP_Server_Scripts.Authentication;
using RP_Server_Scripts.Database;
using RP_Server_Scripts.Database.Character;
using RP_Server_Scripts.Logging;

namespace RP_Server_Scripts.Character.Transaction
{
    internal sealed class SetAccountActiveCharacterTransaction
    {
        private readonly ICharacterManagementContextFactory _ContextFactory;
        private readonly ILogger _Log;

        internal SetAccountActiveCharacterTransaction(ICharacterManagementContextFactory contextFactory, ILoggerFactory loggerFactory)
        {
            _ContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _Log = loggerFactory?.GetLogger(GetType()) ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public Task SetAccountActiveCharacterAsync(Account account, Character character)
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var db = _ContextFactory.Create())
                    {
                        //Find out if we have a entry for the last active character in the database.
                        AccountLastUsedCharacterEntity activeCharEntity = db.LastUsedCharacters.FirstOrDefault(lastChar =>
                            lastChar.Account.AccountId == account.AccountId);

                        //An entity does exist. Simply update the character key
                        if (activeCharEntity != null)
                        {
                            //The ids do match, so lets just return because no further action is required.
                            if (activeCharEntity.CharacterId == character.CharacterId)
                            {
                                return;
                            }

                            activeCharEntity.CharacterId = character.CharacterId;
                        }

                        //No last used character entity found, create a new one.
                        else
                        {
                            db.LastUsedCharacters.Add(new AccountLastUsedCharacterEntity
                            {
                                AccountId = account.AccountId,
                                CharacterId = character.CharacterId
                            });
                        }

                        //We have made changes, lets write them back to the database.
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    string msg =
                        $"Something went wrong while setting the active character(last used character) of account '{account.UserName}' to character '{character.Name}'";
                    _Log.Error(msg);
                    throw new DatabaseRequestException(msg,e);
                }
            });
        }
    }
}
