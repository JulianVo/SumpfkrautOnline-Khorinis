using System;
using System.Linq;
using System.Threading.Tasks;
using RP_Server_Scripts.Database;
using RP_Server_Scripts.Database.Character;
using RP_Server_Scripts.Logging;

namespace RP_Server_Scripts.Character.Transaction
{
    internal sealed class CheckCharacterExistsTransaction
    {
        private readonly CharacterService _Service;
        private readonly ICharacterManagementContextFactory _ContextFactory;
        private readonly ILogger _Log;

        internal CheckCharacterExistsTransaction(CharacterService service, ILoggerFactory loggerFactory, ICharacterManagementContextFactory contextFactory)
        {
            _Service = service ?? throw new ArgumentNullException(nameof(service));
            _ContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _Log = loggerFactory?.GetLogger(GetType()) ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public Task<bool> CheckCharacterExistsAsync(string characterName)
        {
            if (string.IsNullOrWhiteSpace(characterName))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(characterName));
            }

            //run the database request on the thread pool.
            var task = new Task<bool>(() =>
            {
                //Before accessing the database, check if a character with the given name is already loaded.
                if (_Service.IsCharacterLoaded(characterName))
                {
                    return true;
                }

                //Access the database and search for a character with the given name.
                try
                {
                    using (CharacterManagementContext db = _ContextFactory.Create())
                    {
                        return db.Characters.Any(c =>
                            c.CharacterName.Equals(characterName, StringComparison.InvariantCultureIgnoreCase));
                    }
                }
                catch (Exception e)
                {
                    string msg =
                        $"Something went wrong while checking for the existence of a character with the name '{characterName}'";
                    _Log.Error(msg);

                    throw new DatabaseRequestException(msg, e);
                }
            });
            task.Start();
            return task;
        }
    }
}
