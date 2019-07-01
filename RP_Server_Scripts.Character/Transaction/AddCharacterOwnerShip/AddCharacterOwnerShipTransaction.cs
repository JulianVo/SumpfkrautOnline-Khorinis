using System;
using System.Linq;
using System.Threading.Tasks;
using RP_Server_Scripts.Authentication;
using RP_Server_Scripts.Character.Transaction.AddCharacterOwnerShip;
using RP_Server_Scripts.Database;
using RP_Server_Scripts.Database.Character;
using RP_Server_Scripts.Logging;
using RP_Server_Scripts.RP;

namespace RP_Server_Scripts.Character.Transaction
{
    internal sealed class AddCharacterOwnerShipTransaction
    {
        private readonly ICharacterManagementContextFactory _ContextFactory;
        private readonly RpConfig _RpConfig;
        private readonly ILogger _Log;

        public AddCharacterOwnerShipTransaction(ICharacterManagementContextFactory contextFactory, ILoggerFactory loggerFactory, RpConfig rpConfig)
        {
            _ContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _RpConfig = rpConfig ?? throw new ArgumentNullException(nameof(rpConfig));
            _Log = loggerFactory?.GetLogger(GetType()) ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        internal Task<AddCharacterOwnershipResult> AddCharacterOwnershipAsync(Account owner, Character character, DateTime expirationDate,
            bool deleteOnExpiration)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (character == null)
            {
                throw new ArgumentNullException(nameof(character));
            }

            var task = new Task<AddCharacterOwnershipResult>(() =>
             {
                 try
                 {
                     using (CharacterManagementContext db = _ContextFactory.Create())
                     {

                         //Limit the characters that can be created 
                         if (db.CharacterOwnerships.Count(os => os.OwnerId == owner.AccountId) >=
                              _RpConfig.MaxCharacterPerAccount)
                         {
                             return new AddCharacterOwnershipFailed(owner, character, AddCharacterOwnershipFailure.CharacterLimitReached);
                         }

                         if (!db.CharacterOwnerships.Any(os =>
                             os.Owner.AccountId == owner.AccountId && os.Character.CharacterId == character.CharacterId))
                         {
                             db.CharacterOwnerships.Add(new CharacterOwnershipEntity()
                             {
                                 OwnerId = owner.AccountId,
                                 CharacterId = character.CharacterId,
                                 DeleteCharacterOnExpiration = deleteOnExpiration,
                                 ExpiryDate = expirationDate,
                                 CreationDate = DateTime.Now
                             });
                             db.SaveChanges();
                         }
                         return new AddCharacterOwnershipSuccessful(owner, character);
                     }
                 }
                 catch (Exception e)
                 {
                     string msg =
                         $"Something went wrong while adding the ownership of the character with the name '{character.Name}' to the account with the name {owner.UserName}";
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
